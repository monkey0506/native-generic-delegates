using System.Text;

namespace Monkeymoto.NativeGenericDelegates
{
    internal static class Declarations
    {
        private static void BuildBaseInterfaceDefinition(StringBuilder sb, bool isAction, int argumentCount)
        {
            string? qualifiedTypeParameters;
            string? returnType;
            string? type;
            string? typeParameters;
            string? antiConstraints;
            bool hasGenericMethods = true;
            if (isAction)
            {
                returnType = "void";
                type = Constants.CategoryAction;
                antiConstraints = Constants.Actions.AntiConstraints[argumentCount];
                if (argumentCount != 0)
                {
                    qualifiedTypeParameters = $"<{Constants.Actions.QualifiedTypeParameters[argumentCount]}>";
                    typeParameters = $"<{Constants.Actions.TypeParameters[argumentCount]}";
                }
                else
                {
                    qualifiedTypeParameters = string.Empty;
                    typeParameters = string.Empty;
                    hasGenericMethods = false;
                }
            }
            else
            {
                qualifiedTypeParameters = $"<{Constants.Funcs.QualifiedTypeParameters[argumentCount]}>";
                returnType = "TResult";
                type = Constants.CategoryFunc;
                typeParameters = $"<{Constants.Funcs.TypeParameters[argumentCount]}";
                antiConstraints = Constants.Funcs.AntiConstraints[argumentCount];
            }
            string genericType = $"{type}{typeParameters}";
            if (typeParameters.Length != 0)
            {
                genericType = $"{genericType}>";
                typeParameters = $"{typeParameters}>";
            }
            string fullType = $"INative{type}{typeParameters}";
            string parameters = Constants.Parameters[argumentCount];
            string typeAsArgument = type.ToLower();
            string callingConvention = $",{Constants.NewLineIndent3}CallingConvention callingConvention = CallingConvention.Winapi";
            string genericMethods = hasGenericMethods ?
     $@"
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified managed delegate, with a specified
        /// marshalling descriptor.
        /// </summary>
        /// <param name=""{typeAsArgument}"">The managed delegate to create a wrapper around.</param>
        /// <param name=""callingConvention"">The unmanaged calling convention.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        /// <typeparam name=""TMarshaller"">The marshalling descriptor.</typeparam>
        public static {fullType} From{type}<TMarshaller>
        (
            {genericType} {typeAsArgument}{callingConvention}
        )
            where TMarshaller : IMarshaller<TMarshaller>, new()
        {{
            throw new NotImplementedException();
        }}
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified unmanaged function pointer, with a specified
        /// marshalling descriptor.
        /// </summary>
        /// <param name=""functionPtr"">A pointer to the unmanaged function to create a wrapper around.</param>
        /// <param name=""callingConvention"">The unmanaged calling convention.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        /// <typeparam name=""TMarshaller"">The marshalling descriptor.</typeparam>
        public static {fullType} FromFunctionPointer<TMarshaller>
        (
            nint functionPtr{callingConvention}
        )
            where TMarshaller : IMarshaller<TMarshaller>, new()
        {{
            throw new NotImplementedException();
        }}" :
                string.Empty;
            _ = sb.Append
            (
$@"    /// <summary>
    /// Represents a native generic delegate.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A ""native generic delegate"" is a managed or unmanaged method which can be invoked from both managed and
    /// unmanaged code. Methods are provided to invoke the native generic delegate directly from managed code, and to
    /// obtain a function pointer which can be used to invoke from unmanaged code.
    /// </para><para>
    /// The type parameters match those of <see cref=""{genericType.Replace('<', '{').Replace('>', '}')}""/>.
    /// </para>
    /// </remarks>
    internal interface INative{type}{qualifiedTypeParameters}{antiConstraints}
    {{
        /// <summary>
        /// Gets the calling convention used when the method is invoked from unmanaged code.
        /// </summary>
        public CallingConvention CallingConvention {{ get; }}
        protected object? Target {{ get; }}
        protected MethodInfo Method {{ get; }}
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified managed delegate.
        /// </summary>
        /// <param name=""action"">The managed delegate to create a wrapper around.</param>
        /// <param name=""callingConvention"">The unmanaged calling convention.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        public static {fullType} From{type}
        (
            {genericType} {typeAsArgument}{callingConvention}
        )
        {{
            throw new NotImplementedException();
        }}
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified unmanaged function pointer.
        /// </summary>
        /// <param name=""functionPtr"">A pointer to the unmanaged function to create a wrapper around.</param>
        /// <param name=""callingConvention"">The unmanaged calling convention.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        public static {fullType} FromFunctionPointer
        (
            nint functionPtr{callingConvention}
        )
        {{
            throw new NotImplementedException();
        }}{genericMethods}

        /// <summary>
        /// Gets a function pointer that can be used to invoke the native generic delegate from unmanaged code.
        /// </summary>
        public nint GetFunctionPointer();
        /// <summary>
        /// Invokes the native generic delegate from managed code.
        /// </summary>
        public {returnType} Invoke({parameters});
        
        /// <summary>
        /// Creates a managed delegate from the native generic delegate.
        /// </summary>
        public {genericType} To{type}() =>
            ({genericType})Delegate.CreateDelegate
            (
                typeof({genericType}),
                Target,
                Method
            );
    }}
"
            );
        }

        private static void BuildUnmanagedInterfaceDefinition(StringBuilder sb, bool isAction, int argumentCount)
        {
            string? qualifiedTypeParameters;
            string? category;
            string? managedTypeParameters;
            string? unmanagedTypeParameters;
            string? fullTypeParameterList;
            string? constraints;
            bool hasGenericMethods = true;
            if (isAction)
            {
                category = Constants.CategoryAction;
                constraints = Constants.Actions.UnmanagedConstraints[argumentCount];
                if (argumentCount != 0)
                {
                    qualifiedTypeParameters = Constants.Actions.QualifiedTypeParameters[argumentCount];
                    managedTypeParameters = Constants.Actions.TypeParameters[argumentCount];
                    qualifiedTypeParameters =
                        $"<{qualifiedTypeParameters}, {managedTypeParameters.Replace('T', 'U')}>";
                }
                else
                {
                    qualifiedTypeParameters = string.Empty;
                    managedTypeParameters = string.Empty;
                    hasGenericMethods = false;
                }
            }
            else
            {
                category = Constants.CategoryFunc;
                constraints = Constants.Funcs.UnmanagedConstraints[argumentCount];
                qualifiedTypeParameters = Constants.Funcs.QualifiedTypeParameters[argumentCount];
                managedTypeParameters = Constants.Funcs.TypeParameters[argumentCount];
                qualifiedTypeParameters = $"<{qualifiedTypeParameters}, {managedTypeParameters.Replace('T', 'U')}>";
            }
            if (managedTypeParameters.Length != 0)
            {
                unmanagedTypeParameters = managedTypeParameters.Replace('T', 'U');
                fullTypeParameterList = $"<{managedTypeParameters}, {unmanagedTypeParameters}>";
                managedTypeParameters = $"<{managedTypeParameters}>";
                var voidReturn = isAction ? ", void" : string.Empty;
                unmanagedTypeParameters = $"<{unmanagedTypeParameters}{voidReturn}>";
            }
            else
            {
                fullTypeParameterList = string.Empty;
                unmanagedTypeParameters = "<void>";
            }
            string systemDelegate = $"{category}{managedTypeParameters}";
            string baseType = $"INative{category}{managedTypeParameters}";
            string fullType = $"IUnmanaged{category}{fullTypeParameterList}";
            string systemDelegateArgument = category.ToLower();
            string callingConvention =
                $",{Constants.NewLineIndent3}CallingConvention callingConvention = CallingConvention.Winapi";
            string genericMethods = hasGenericMethods ?
     $@"
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified managed delegate, with a specified
        /// marshalling descriptor.
        /// </summary>
        /// <param name=""{systemDelegateArgument}"">The managed delegate to create a wrapper around.</param>
        /// <param name=""callingConvention"">The unmanaged calling convention.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        /// <typeparam name=""TMarshaller"">The marshalling descriptor.</typeparam>
        public new static {fullType} From{category}<TMarshaller>
        (
            {systemDelegate} {systemDelegateArgument}{callingConvention}
        )
            where TMarshaller : IMarshaller<TMarshaller>, new()
        {{
            throw new NotImplementedException();
        }}
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified unmanaged function pointer, with a specified
        /// marshalling descriptor.
        /// </summary>
        /// <param name=""functionPtr"">A pointer to the unmanaged function to create a wrapper around.</param>
        /// <param name=""callingConvention"">The unmanaged calling convention.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        /// <typeparam name=""TMarshaller"">The marshalling descriptor.</typeparam>
        public new static {fullType} FromFunctionPointer<TMarshaller>
        (
            nint functionPtr{callingConvention}
        )
            where TMarshaller : IMarshaller<TMarshaller>, new()
        {{
            throw new NotImplementedException();
        }}
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified unmanaged function pointer, with a specified
        /// marshalling descriptor.
        /// </summary>
        /// <param name=""functionPtr"">A pointer to the unmanaged function to create a wrapper around.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        /// <typeparam name=""TMarshaller"">The marshalling descriptor.</typeparam>
        public static {fullType} FromFunctionPointer<TMarshaller>
        (
            delegate* unmanaged[Cdecl]{unmanagedTypeParameters} functionPtr
        )
            where TMarshaller : IMarshaller<TMarshaller>, new()
        {{
            throw new NotImplementedException();
        }}
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified unmanaged function pointer, with a specified
        /// marshalling descriptor.
        /// </summary>
        /// <param name=""functionPtr"">A pointer to the unmanaged function to create a wrapper around.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        /// <typeparam name=""TMarshaller"">The marshalling descriptor.</typeparam>
        public static {fullType} FromFunctionPointer<TMarshaller>
        (
            delegate* unmanaged[Stdcall]{unmanagedTypeParameters} functionPtr
        )
            where TMarshaller : IMarshaller<TMarshaller>, new()
        {{
            throw new NotImplementedException();
        }}
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified unmanaged function pointer, with a specified
        /// marshalling descriptor.
        /// </summary>
        /// <param name=""functionPtr"">A pointer to the unmanaged function to create a wrapper around.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        /// <typeparam name=""TMarshaller"">The marshalling descriptor.</typeparam>
        public static {fullType} FromFunctionPointer<TMarshaller>
        (
            delegate* unmanaged[Thiscall]{unmanagedTypeParameters} functionPtr
        )
            where TMarshaller : IMarshaller<TMarshaller>, new()
        {{
            throw new NotImplementedException();
        }}" :
                string.Empty;
            _ = sb.Append
            (
$@"#if UNSAFE
    /// <summary>
    /// Represents a native generic delegate which supports <see langword=""unsafe""/> function pointers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Using this type requires the <c>/unsafe</c> compiler switch <b>and</b> the constant <c>UNSAFE</c> to be
    /// defined.
    /// </para><para>
    /// A ""native generic delegate"" is a managed or unmanaged method which can be invoked from both managed and
    /// unmanaged code. Methods are provided to invoke the native generic delegate directly from managed code, and to
    /// obtain a function pointer which can be used to invoke from unmanaged code. Properties are also provided which
    /// give access to <see langword=""unsafe""/> function pointers, which can be invoked directly from managed or
    /// unmanaged code.
    /// </para><para>
    /// The type parameters prefixed with <c>T-</c> match those of
    /// <see cref=""{systemDelegate.Replace('<', '{').Replace('>', '}')}""/>. The type parameters prefixed with
    /// <c>U-</c> correspond to the unmanaged method's arguments and/or return type, and are constrained by the
    /// <see langword=""unmanaged""/> type constraint.
    /// </para>
    /// </remarks>
    internal unsafe interface IUnmanaged{category}{qualifiedTypeParameters} : {baseType}{constraints}
    {{
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified managed delegate.
        /// </summary>
        /// <param name=""{systemDelegateArgument}"">The managed delegate to create a wrapper around.</param>
        /// <param name=""callingConvention"">The unmanaged calling convention.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        public new static {fullType} From{category}
        (
            {systemDelegate} {systemDelegateArgument}{callingConvention}
        )
        {{
            throw new NotImplementedException();
        }}
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified unmanaged function pointer.
        /// </summary>
        /// <param name=""functionPtr"">A pointer to the unmanaged function to create a wrapper around.</param>
        /// <param name=""callingConvention"">The unmanaged calling convention.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        public new static {fullType} FromFunctionPointer
        (
            nint functionPtr{callingConvention}
        )
        {{
            throw new NotImplementedException();
        }}
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified unmanaged function pointer.
        /// </summary>
        /// <param name=""functionPtr"">A pointer to the unmanaged function to create a wrapper around.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        public static {fullType} FromFunctionPointer
        (
            delegate* unmanaged[Cdecl]{unmanagedTypeParameters} functionPtr
        )
        {{
            throw new NotImplementedException();
        }}
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified unmanaged function pointer.
        /// </summary>
        /// <param name=""functionPtr"">A pointer to the unmanaged function to create a wrapper around.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        public static {fullType} FromFunctionPointer
        (
            delegate* unmanaged[Stdcall]{unmanagedTypeParameters} functionPtr
        )
        {{
            throw new NotImplementedException();
        }}
        
        /// <summary>
        /// Creates a native generic delegate wrapper around the specified unmanaged function pointer.
        /// </summary>
        /// <param name=""functionPtr"">A pointer to the unmanaged function to create a wrapper around.</param>
        /// <returns>The created native generic delegate wrapper.</returns>
        public static {fullType} FromFunctionPointer
        (
            delegate* unmanaged[Thiscall]{unmanagedTypeParameters} functionPtr
        )
        {{
            throw new NotImplementedException();
        }}{genericMethods}
        
        /// <summary>
        /// Gets the unmanaged function pointer with <c>Cdecl</c> calling convention that this native generic delegate
        /// represents, or <see langword=""null""/>.
        /// </summary>
        public delegate* unmanaged[Cdecl]{unmanagedTypeParameters} AsCdeclPtr {{ get; }}
        /// <summary>
        /// Gets the unmanaged function pointer with <c>Stdcall</c> calling convention that this native generic delegate
        /// represents, or <see langword=""null""/>.
        /// </summary>
        public delegate* unmanaged[Stdcall]{unmanagedTypeParameters} AsStdCallPtr {{ get; }}
        /// <summary>
        /// Gets the unmanaged function pointer with <c>Thiscall</c> calling convention that this native generic delegate
        /// represents, or <see langword=""null""/>.
        /// </summary>
        public delegate* unmanaged[Thiscall]{unmanagedTypeParameters} AsThisCallPtr {{ get; }}
    }}
#endif // UNSAFE
"
            );
        }

        public static string GetSourceText()
        {
            var source = new StringBuilder
            (
$@"// <auto-generated/>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable

namespace {Constants.RootNamespace}
{{
    /// <summary>
    /// Represents a compile-time descriptor used to generate marshalling behaviors for native generic delegates.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The values defined by these properties must be parsable at compile-time in order for appropriate source code to
    /// be generated for your native generic delegate wrappers. Any of these values which are specified must follow the
    /// same parsing rules as <see cref=""System.Attribute"">attribute</see> arguments, or they will be ignored.
    /// </para><para>
    /// For example, the <see cref=""IMarshaller{{TSelf}}.CallingConvention"">CallingConvention</see> property must be a
    /// literal reference to one of the enumerated values, or <see langword=""null""/>. The
    /// <see cref=""IMarshaller{{TSelf}}.MarshalMap"">MarshalMap</see>,
    /// <see cref=""IMarshaller{{TSelf}}.MarshalParamsAs"">MarshalParamsAs</see>, and
    /// <see cref=""IMarshaller{{TSelf}}.MarshalReturnAs"">MarshalReturnAs</see> properties must be <see langword=""null""/>
    /// or a <see langword=""new""/> expression with optional initializers.
    /// </para>
    /// </remarks>
    /// <typeparam name=""TSelf"">The user-defined type which provides these properties.</typeparam>
    internal interface IMarshaller<TSelf> where TSelf : IMarshaller<TSelf>
    {{
        /// <summary>
        /// Gets the unmanaged calling convention.
        /// </summary>
        protected static virtual CallingConvention? CallingConvention => null;
        /// <summary>
        /// Gets the <see cref=""Monkeymoto.NativeGenericDelegates.MarshalMap"">MarshalMap</see> descriptor.
        /// </summary>
        protected static virtual MarshalMap? MarshalMap => null;
        /// <summary>
        /// Gets the <see cref=""System.Runtime.InteropServices.MarshalAsAttribute"">MarshalAsAttribute</see> which will
        /// be applied to each parameter, or <see langword=""null""/> to omit the attribute. This property is ignored for
        /// native generic delegates which do not take any parameters.
        /// </summary>
        protected static virtual MarshalAsAttribute?[]? MarshalParamsAs => null;
        /// <summary>
        /// Gets the <see cref=""System.Runtime.InteropServices.MarshalAsAttribute"">MarshalAsAttribute</see> which will
        /// be applied to the return value, or <see langword=""null""/>. This property is ignored for native generic
        /// delegates which do not return a value.
        /// </summary>
        protected static virtual MarshalAsAttribute? MarshalReturnAs => null;
    }}
    "
            );
            for (int i = 0; i < 17; ++i)
            {
                BuildBaseInterfaceDefinition(source.AppendLine(), isAction: true, argumentCount: i);
                BuildBaseInterfaceDefinition(source.AppendLine(), isAction: false, argumentCount: i);
                BuildUnmanagedInterfaceDefinition(source.AppendLine(), isAction: true, argumentCount: i);
                BuildUnmanagedInterfaceDefinition(source.AppendLine(), isAction: false, argumentCount: i);
            }
            _ = source.AppendLine
            (
 $@"
    /// <summary>
    /// Represents a compile-time only mapping of <see cref=""System.Type"">types</see> to
    /// <see cref=""System.Runtime.InteropServices.MarshalAsAttribute"">MarshalAsAttribute</see>s.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type implements
    /// <see cref=""System.Collections.Generic.IEnumerable{{T}}"">IEnumerable</see>&lt;<see
    /// cref=""System.Collections.Generic.KeyValuePair{{TKey, Value}}"">KeyValuePair</see>&lt;<see
    /// cref=""System.Type"">Type</see>,
    /// <see cref=""System.Runtime.InteropServices.MarshalAsAttribute"">MarshalAsAttribute</see>&gt;&gt; as required for
    /// collection-initializer support. However, this type is not intended for runtime consumption. Attempting to
    /// enumerate this collection will cause a <see cref=""System.NotImplementedException"">NotImplementedException</see>
    /// to be thrown.
    /// </para>
    /// </remarks>
    internal sealed class MarshalMap : IEnumerable<KeyValuePair<Type, MarshalAsAttribute>>
    {{
        public MarshalMap() {{ }}
        public void Add(Type key, MarshalAsAttribute value) {{ }}
        IEnumerator<KeyValuePair<Type, MarshalAsAttribute>>
            IEnumerable<KeyValuePair<Type, MarshalAsAttribute>>.GetEnumerator() =>
                throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }}
}}

namespace System.Runtime.CompilerServices
{{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal sealed class InterceptsLocationAttribute(string filePath, int line, int character) : Attribute
    {{
        public string FilePath {{ get; }} = filePath;
        public int Line {{ get; }} = line;
        public int Character {{ get; }} = character;
    }}
}}

#nullable restore"
            );
            return source.ToString();
        }
    }
}
