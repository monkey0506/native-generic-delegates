using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace NativeGenericDelegates
{
    /// <summary>
    /// Factory methods for building a runtime native generic delegate.
    /// </summary>
    internal static class DelegateFactory
    {
        private const MethodAttributes ctorAttrs =
            MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public;
        private const MethodImplAttributes implAttrs = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
        private const MethodAttributes invokeAttrs =
            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
        private static readonly Type[] ctorSig = new[] { typeof(object), typeof(nint) };
        private static readonly AssemblyName assemblyName = new("RuntimeNativeGenericDelegates");
        private static readonly AssemblyBuilder assemblyBuilder =
            AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
        private static readonly ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name!);
        private static readonly ConstructorInfo unmanagedFuncPtrCtorInfo =
            typeof(UnmanagedFunctionPointerAttribute).GetConstructor(new[] { typeof(CallingConvention) })!;
        private static readonly CustomAttributeBuilder cdeclAttrBuilder =
            new(unmanagedFuncPtrCtorInfo, new object[] { CallingConvention.Cdecl });
        private static readonly CustomAttributeBuilder stdCallAttrBuilder =
            new(unmanagedFuncPtrCtorInfo, new object[] { CallingConvention.StdCall });
        private static readonly CustomAttributeBuilder thisCallAttrBuilder =
            new(unmanagedFuncPtrCtorInfo, new object[] { CallingConvention.ThisCall });
        private static readonly CustomAttributeBuilder winapiAttrBuilder =
            new(unmanagedFuncPtrCtorInfo, new object[] { CallingConvention.Winapi });
        // cache defined runtime delegate types based on signature and marshaling behavior
        private static readonly Dictionary<NativeGenericDelegateInfo, Type> delegates = new();

        /// <summary>
        /// Defines a runtime delegate type.
        /// </summary>
        /// <remarks>
        /// This runtime delegate type will implement exactly one of the <see cref="INativeAction">INativeAction</see> or
        /// <see cref="INativeFunc{TResult}">INativeFunc</see> family of interfaces that has a matching signature.
        /// </remarks>
        /// <param name="info">The native generic delegate info describing the runtime delegate.</param>
        /// <returns>The <see cref="Type">Type</see> of the runtime delegate.</returns>
        private static Type DefineDelegateType(NativeGenericDelegateInfo info)
        {
            Type returnType = info.ReturnType;
            MarshalAsAttribute? marshalReturnAs = info.ReturnParameter?.MarshalAs;
            Type[] parameters = info.Parameters.Select(p => p.ParameterType).ToArray();
            MarshalAsAttribute?[] marshalParamAs = info.Parameters.Select(p => p.MarshalAs).ToArray();
            // generate a unique name for the delegate type
            string name = $"NativeGenericDelegate{Guid.NewGuid()}";
            // define the type and constructor
            TypeBuilder typeBuilder =
                moduleBuilder.DefineType(name, TypeAttributes.Public | TypeAttributes.Sealed, typeof(MulticastDelegate));
            typeBuilder.DefineConstructor(ctorAttrs, CallingConventions.Standard, ctorSig).SetImplementationFlags(implAttrs);
            // implement the INativeAction/INativeFunc interface with the matching signature
            typeBuilder.AddInterfaceImplementation(GetInterfaceType(returnType, parameters));
            // set the unmanaged calling convention
            typeBuilder.SetCustomAttribute(info.CallingConvention switch
            {
                CallingConvention.Cdecl => cdeclAttrBuilder,
                CallingConvention.StdCall => stdCallAttrBuilder,
                CallingConvention.ThisCall => thisCallAttrBuilder,
                CallingConvention.Winapi => winapiAttrBuilder,
                // CallingConvention.FastCall or unexpected values are unsupported
                _ => throw new NotSupportedException($"Calling convention {info.CallingConvention} is not supported.")
            });
            // define the Invoke method
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Invoke", invokeAttrs, returnType, parameters);
            methodBuilder.SetImplementationFlags(implAttrs);
            // define custom marshaling behavior for Invoke parameters and return parameter
            ParameterBuilder parameterBuilder = methodBuilder.DefineParameter(0, ParameterAttributes.Retval, "_ret");
            if (marshalReturnAs is not null)
            {
                // set the return value marshal behavior
                parameterBuilder.SetCustomAttribute(marshalReturnAs.ToCustomAttributeBuilder());
            }
            for (int i = 0; i < parameters.Length; ++i)
            {
                parameterBuilder = methodBuilder.DefineParameter(i + 1, ParameterAttributes.None, $"_{i}");
                if (marshalParamAs[i] is not null)
                {
                    // set the parameter marshal behavior
                    parameterBuilder.SetCustomAttribute(marshalParamAs[i]!.ToCustomAttributeBuilder());
                }
            }
            // create the type and cache it so matching signatures (with matching marshaling) won't unnecessarily create new
            // delegate types
            Type type = typeBuilder.CreateType();
            delegates[info] = type;
            return type;
        }

        /// <summary>
        /// Gets an instance of the runtime delegate type described by the given info.
        /// </summary>
        /// <param name="info">The native generic delegate info describing the runtime delegate.</param>
        /// <param name="target">The new delegate target.</param>
        /// <param name="method">The new delegate method.</param>
        /// <returns>The delegate instance.</returns>
        internal static Delegate GetDelegate(NativeGenericDelegateInfo info, object? target, MethodInfo method)
        {
            return Delegate.CreateDelegate(GetDelegateType(info), target, method);
        }

        /// <summary>
        /// Gets an instance of the runtime delegate type described by the given info.
        /// </summary>
        /// <param name="info">The native generic delegate info describing the runtime delegate.</param>
        /// <param name="functionPtr">The unmanaged function pointer to wrap with the new delegate.</param>
        /// <returns>The delegate instance.</returns>
        internal static Delegate GetDelegate(NativeGenericDelegateInfo info, nint functionPtr)
        {
            return Marshal.GetDelegateForFunctionPointer(functionPtr, GetDelegateType(info));
        }

        /// <summary>
        /// Gets the runtime delegate <see cref="Type">Type</see> described by the given info.
        /// </summary>
        /// <param name="info">The native generic delegate info describing the runtime delegate.</param>
        /// <returns>The runtime delegate type.</returns>
        private static Type GetDelegateType(NativeGenericDelegateInfo info)
        {
            if (delegates.TryGetValue(info, out Type? type))
            {
                return type;
            }
            return DefineDelegateType(info);
        }

        /// <summary>
        /// Gets the <see cref="INativeAction">INativeAction</see> or <see cref="INativeFunc{TResult}">INativeFunc</see>
        /// interface type with the given return type and parameter types.
        /// </summary>
        /// <returns>The interface type.</returns>
        private static Type GetInterfaceType(Type returnType, Type[] parameters)
        {
            bool isAction = returnType == typeof(void);
            int parameterCount = parameters.Length;
            if (parameterCount == 0)
            {
                return isAction ? typeof(INativeAction) : typeof(INativeFunc<>).MakeGenericType(returnType);
            }
            if (!isAction)
            {
                parameters = parameters.Append(returnType).ToArray();
            }
            return (parameterCount switch
            {
                /////////////////////////////////// * \\\\\\\\\\|////////// * \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                1 => isAction ? typeof(INativeAction<>) : typeof(INativeFunc<,>), /////////////////////////////////
                2 => isAction ? typeof(INativeAction<,>) : typeof(INativeFunc<,,>), //\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                3 => isAction ? typeof(INativeAction<,,>) : typeof(INativeFunc<,,,>), /////////////////////////////
                4 => isAction ? typeof(INativeAction<,,,>) : typeof(INativeFunc<,,,,>), //\\\\\\\\\\\\\\\\\\\\\\\\\
                5 => isAction ? typeof(INativeAction<,,,,>) : typeof(INativeFunc<,,,,,>), /////////////////////////
                6 => isAction ? typeof(INativeAction<,,,,,>) : typeof(INativeFunc<,,,,,,>), //\\\\\\\\\\\\\\\\\\\\\
                7 => isAction ? typeof(INativeAction<,,,,,,>) : typeof(INativeFunc<,,,,,,,>), /////////////////////
                8 => isAction ? typeof(INativeAction<,,,,,,,>) : typeof(INativeFunc<,,,,,,,,>), //\\\\\\\\\\\\\\\\\
                9 => isAction ? typeof(INativeAction<,,,,,,,,>) : typeof(INativeFunc<,,,,,,,,,>), /////////////////
                10 => isAction ? typeof(INativeAction<,,,,,,,,,>) : typeof(INativeFunc<,,,,,,,,,,>), //\\\\\\\\\\\\
                11 => isAction ? typeof(INativeAction<,,,,,,,,,,>) : typeof(INativeFunc<,,,,,,,,,,,>), ////////////
                12 => isAction ? typeof(INativeAction<,,,,,,,,,,,>) : typeof(INativeFunc<,,,,,,,,,,,,>), //\\\\\\\\
                13 => isAction ? typeof(INativeAction<,,,,,,,,,,,,>) : typeof(INativeFunc<,,,,,,,,,,,,,>), ////////
                14 => isAction ? typeof(INativeAction<,,,,,,,,,,,,,>) : typeof(INativeFunc<,,,,,,,,,,,,,,>), //\\\\
                15 => isAction ? typeof(INativeAction<,,,,,,,,,,,,,,>) : typeof(INativeFunc<,,,,,,,,,,,,,,,>), ////
                16 => isAction ? typeof(INativeAction<,,,,,,,,,,,,,,,>) : typeof(INativeFunc<,,,,,,,,,,,,,,,,>), //
                /////////////////////////////////////      \|_\|      ///////////\\\\\\\\\\\      \\__\\       ////
                _ => throw new NotSupportedException("The maximum number of supported delegate parameters is 16.")
            }).MakeGenericType(parameters);
        }

        /// <summary>
        /// Converts a <see cref="MarshalAsAttribute">MarshalAsAttribute</see> to a <see
        /// cref="CustomAttributeBuilder">CustomAttributeBuilder</see>.
        /// </summary>
        /// <param name="_this">The attribute to convert.</param>
        /// <returns>The custom attribute builder.</returns>
        /// <exception cref="UnreachableException">
        /// An unexpected attribute field was encountered.
        /// </exception>
        private static CustomAttributeBuilder ToCustomAttributeBuilder(this MarshalAsAttribute _this)
        {
            FieldInfo[] fieldInfos = typeof(MarshalAsAttribute).GetFields(BindingFlags.Public | BindingFlags.Instance).Where
                (f =>
                {
                    return f.Name switch
                    {
                        nameof(MarshalAsAttribute.MarshalCookie) => _this.MarshalCookie is not null,
                        nameof(MarshalAsAttribute.MarshalType) => _this.MarshalType is not null,
                        nameof(MarshalAsAttribute.MarshalTypeRef) => _this.MarshalTypeRef is not null,
                        nameof(MarshalAsAttribute.SafeArrayUserDefinedSubType) => _this.SafeArrayUserDefinedSubType is not null,
                        _ => true
                    };
                }).ToArray();
            object?[] fieldValues = fieldInfos.Select
                (f =>
                {
                    return f.Name switch
                    {
                        nameof(MarshalAsAttribute.ArraySubType) => (object)(int)_this.ArraySubType,
                        nameof(MarshalAsAttribute.IidParameterIndex) => _this.IidParameterIndex,
                        nameof(MarshalAsAttribute.MarshalCookie) => _this.MarshalCookie,
                        nameof(MarshalAsAttribute.MarshalType) => _this.MarshalType,
                        nameof(MarshalAsAttribute.MarshalTypeRef) => _this.MarshalTypeRef,
                        nameof(MarshalAsAttribute.SafeArraySubType) => (int)_this.SafeArraySubType,
                        nameof(MarshalAsAttribute.SafeArrayUserDefinedSubType) => _this.SafeArrayUserDefinedSubType,
                        nameof(MarshalAsAttribute.SizeConst) => _this.SizeConst,
                        nameof(MarshalAsAttribute.SizeParamIndex) => _this.SizeParamIndex,
                        _ => throw new UnreachableException()
                    };
                }).ToArray();
            return new CustomAttributeBuilder
                (
                    typeof(MarshalAsAttribute).GetConstructor(new[] { typeof(UnmanagedType) })!,
                    new object[] { _this.Value },
                    fieldInfos,
                    fieldValues
                );
        }
    }
}
