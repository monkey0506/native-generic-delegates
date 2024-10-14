using System.Text;

namespace Monkeymoto.NativeGenericDelegates
{
    internal static class PostInitialization
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
        
        public static {fullType} From{type}<TMarshaller>
        (
            {genericType} {typeAsArgument}{callingConvention}
        )
            where TMarshaller : IMarshaller<TMarshaller>, new()
        {{
            throw new NotImplementedException();
        }}
        
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
$@"    internal interface INative{type}{qualifiedTypeParameters}{antiConstraints}
    {{
        protected object? Target {{ get; }}
        protected MethodInfo Method {{ get; }}
        
        public static {fullType} From{type}
        (
            {genericType} {typeAsArgument}{callingConvention}
        )
        {{
            throw new NotImplementedException();
        }}
        
        public static {fullType} FromFunctionPointer
        (
            nint functionPtr{callingConvention}
        )
        {{
            throw new NotImplementedException();
        }}{genericMethods}

        public nint GetFunctionPointer();
        public {returnType} Invoke({parameters});
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

        public new static {fullType} From{category}<TMarshaller>
        (
            {systemDelegate} {systemDelegateArgument}{callingConvention}
        )
            where TMarshaller : IMarshaller<TMarshaller>, new()
        {{
            throw new NotImplementedException();
        }}

        public new static {fullType} FromFunctionPointer<TMarshaller>
        (
            nint functionPtr{callingConvention}
        )
            where TMarshaller : IMarshaller<TMarshaller>, new()
        {{
            throw new NotImplementedException();
        }}
        
        public static {fullType} FromFunctionPointer<TMarshaller>
        (
            delegate* unmanaged[Cdecl]{unmanagedTypeParameters} functionPtr
        )
            where TMarshaller : IMarshaller<TMarshaller>, new()
        {{
            throw new NotImplementedException();
        }}
        
        public static {fullType} FromFunctionPointer<TMarshaller>
        (
            delegate* unmanaged[Stdcall]{unmanagedTypeParameters} functionPtr
        )
            where TMarshaller : IMarshaller<TMarshaller>, new()
        {{
            throw new NotImplementedException();
        }}
        
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
    internal unsafe interface IUnmanaged{category}{qualifiedTypeParameters} : {baseType}{constraints}
    {{
        public new static {fullType} From{category}
        (
            {systemDelegate} {systemDelegateArgument}{callingConvention}
        )
        {{
            throw new NotImplementedException();
        }}
        
        public new static {fullType} FromFunctionPointer
        (
            nint functionPtr{callingConvention}
        )
        {{
            throw new NotImplementedException();
        }}
        
        public static {fullType} FromFunctionPointer
        (
            delegate* unmanaged[Cdecl]{unmanagedTypeParameters} functionPtr
        )
        {{
            throw new NotImplementedException();
        }}
        
        public static {fullType} FromFunctionPointer
        (
            delegate* unmanaged[Stdcall]{unmanagedTypeParameters} functionPtr
        )
        {{
            throw new NotImplementedException();
        }}
        
        public static {fullType} FromFunctionPointer
        (
            delegate* unmanaged[Thiscall]{unmanagedTypeParameters} functionPtr
        )
        {{
            throw new NotImplementedException();
        }}{genericMethods}
        
        public delegate* unmanaged[Cdecl]{unmanagedTypeParameters} AsCdeclPtr {{ get; }}
        public delegate* unmanaged[Stdcall]{unmanagedTypeParameters} AsStdCallPtr {{ get; }}
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
    internal interface IMarshaller<TSelf> where TSelf : IMarshaller<TSelf>
    {{
        protected static virtual CallingConvention? CallingConvention => null;
        protected static virtual MarshalMap? MarshalMap => null;
        protected static virtual MarshalAsAttribute?[]? MarshalParamsAs => null;
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
