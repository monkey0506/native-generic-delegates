using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct ClassDefinition
    {
        public readonly string SourceText;

        public ClassDefinition
        (
            IMethodSymbol method,
            ArgumentInfo argumentInfo,
            int invokeParameterCount,
            bool isAction,
            bool isFromFunctionPointer,
            IReadOnlyList<MethodReference> references
        )
        {
            var interfaceType = method.ContainingType;
            string? identifier;
            string? returnKeyword;
            string? returnType;
            string? interceptorMarshalReturnAsParam;
            switch (isAction)
            {
                case true:
                    identifier = "Action";
                    returnKeyword = "";
                    returnType = "void";
                    interceptorMarshalReturnAsParam = "";
                    break;
                default:
                    identifier = "Func";
                    returnKeyword = "return ";
                    returnType = interfaceType.TypeArguments.Last().ToDisplayString();
                    interceptorMarshalReturnAsParam =
$@"MarshalAsAttribute marshalReturnAs,
            ";
                    break;
            }
            var suffix = Guid.NewGuid().ToString("N");
            var name = $"Native{identifier}_{suffix}";
            var interfaceTypeArguments = GetInterfaceTypeArguments(interfaceType);
            var interfaceName = $"INative{identifier}{interfaceTypeArguments}";
            var unmanagedCallingConvention = GetUnmanagedCallingConvention(argumentInfo.CallingConvention);
            var interceptorMarshalParamsAsParam = invokeParameterCount == 0 ?
                "" :
$@"MarshalAsAttribute[] marshalParamsAs,
            ";
            var returnMarshalAsAttribute = GetReturnMarshalAsAttribute(argumentInfo.MarshalInfo.MarshalReturnAs);
            var interceptorAttributes = GetInterceptorAttributes(references);
            var interceptorTypeArguments = GetInterceptorTypeArguments(interfaceType.Arity, method.IsGenericMethod);
            var parameters = GetParameters
            (
                argumentInfo.MarshalInfo.MarshalParamsAs,
                invokeParameterCount,
                interfaceType.TypeArguments
            );
            string? constructor;
            string? interceptorParameters;
            switch (isFromFunctionPointer)
            {
                case true:
                    constructor = GetFromFunctionPointerConstructor(name);
                    interceptorParameters = GetInterceptorParameters
                    (
                        "nint functionPtr",
                        interceptorMarshalReturnAsParam,
                        interceptorMarshalParamsAsParam
                    );
                    break;
                default:
                    var arg = identifier.ToLower();
                    var argType = $"{identifier}{interfaceTypeArguments}";
                    var param = $"{argType} {arg}";
                    constructor = GetFromDelegateConstructor(name, param, arg);
                    interceptorParameters = GetInterceptorParameters
                    (
                        param,
                        interceptorMarshalReturnAsParam,
                        interceptorMarshalParamsAsParam
                    );
                    break;
            }
            SourceText =
$@"    file unsafe sealed class {name} : {interfaceName}
    {{
        private readonly Handler handler;
        private readonly nint functionPtr;

        [UnmanagedFunctionPointer(CallingConvention.{argumentInfo.CallingConvention})]
        {returnMarshalAsAttribute}public delegate {returnType} Handler{parameters};

        {constructor}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public nint GetFunctionPointer()
        {{
            return (nint)functionPtr;
        }}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnmanagedCallConv(CallConvs = new[] {{ typeof(CallConv{unmanagedCallingConvention}) }})]
        {returnMarshalAsAttribute}public {returnType} Invoke{parameters}
        {{
            {returnKeyword}handler({Constants.Arguments[invokeParameterCount]});
        }}

        {interceptorAttributes}[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static {interfaceName} {method.Name}{interceptorTypeArguments}
        (
            {interceptorParameters}
        )
        {{
            return new {name}({(isFromFunctionPointer ? "functionPtr" : identifier.ToLower())});
        }}
    }}
";
        }

        private static string GetFromDelegateConstructor(string name, string param, string arg)
        {
            return
$@"internal {name}({param})
        {{
            ArgumentNullException.ThrowIfNull({arg});
            handler = (Handler)Delegate.CreateDelegate(typeof(Handler), {arg}.Target, {arg}.Method);
            functionPtr = Marshal.GetFunctionPointerForDelegate(handler);
        }}";
        }

        private static string GetFromFunctionPointerConstructor(string name)
        {
            return
$@"internal {name}(nint functionPtr)
        {{
            if (functionPtr == nint.Zero)
            {{
                throw new ArgumentNullException(nameof(functionPtr));
            }}
            handler = Marshal.GetDelegateForFunctionPointer<Handler>(functionPtr);
            this.functionPtr = functionPtr;
        }}";
        }

        private static string GetInterceptorAttributes(IReadOnlyList<MethodReference> references)
        {
            var sb = new StringBuilder();
            foreach (var reference in references)
            {
                _ = sb.Append
                (
$@"[InterceptsLocation(@""{reference.FilePath}"", {reference.Line}, {reference.Character})]
        "
                );
            }
            return sb.ToString();
        }

        private static string GetInterceptorParameters(string param, string marshalReturnAsParam, string marshalParamsAsParam)
        {
            return
$@"{param},
            {marshalReturnAsParam}{marshalParamsAsParam}CallingConvention callingConvention";
        }

        private static string GetInterceptorTypeArguments(int interfaceArity, bool isGenericMethod)
        {
            if (interfaceArity == 0)
            {
                return "";
            }
            if (isGenericMethod)
            {
                interfaceArity *= 2;
            }
            return $"<{string.Join(", ", Enumerable.Range(1, interfaceArity).Select(x => $"X{x}"))}>";
        }

        private static string GetInterfaceTypeArguments(INamedTypeSymbol interfaceType)
        {
            if (interfaceType.Arity == 0)
            {
                return "";
            }
            var sb = new StringBuilder("<");
            for (int i = 0; i < interfaceType.Arity; ++i)
            {
                if (i != 0)
                {
                    _ = sb.Append(", ");
                }
                _ = sb.Append(interfaceType.TypeArguments[i].ToDisplayString());
            }
            return sb.Append('>').ToString();
        }

        private static string GetParameters
        (
            IReadOnlyList<string?>? marshalParamsAs,
            int parameterCount,
            ImmutableArray<ITypeSymbol> typeArguments
        )
        {
            if (parameterCount == 0)
            {
                return "()";
            }
            var sb = new StringBuilder
            (
$@"
        (
            "
            );
            for (int i = 0, j = 1; i < parameterCount; ++i, ++j)
            {
                if (marshalParamsAs?[i] is not null)
                {
                    _ = sb.Append
                    (
$@"[MarshalAs({marshalParamsAs[i]})]
            "
                    );
                }
                _ = sb.Append($@"{typeArguments[i].ToDisplayString()} t{(parameterCount == 1 ? "" : j.ToString())}");
                if (j < parameterCount)
                {
                    _ = sb.Append
                    (
$@",
            "
                    );
                }
                else
                {
                    _ = sb.AppendLine();
                }
            }
            _ = sb.Append
            (
$@"        )"
            );
            return sb.ToString();
        }

        private static string GetReturnMarshalAsAttribute(string? marshalReturnAs)
        {
            return marshalReturnAs is null ?
                "" :
$@"[return: MarshalAs({marshalReturnAs})]
        ";
        }

        private static string GetUnmanagedCallingConvention(CallingConvention callingConvention)
        {
            return callingConvention switch
            {
                CallingConvention.Cdecl => "Cdecl",
                CallingConvention.StdCall => "Stdcall",
                CallingConvention.ThisCall => "Thiscall",
                _ => throw new NotSupportedException()
            };
        }
    }
}
