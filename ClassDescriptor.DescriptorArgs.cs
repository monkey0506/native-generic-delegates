using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly partial struct ClassDescriptor
    {
        internal readonly ref struct DescriptorArgs
        {
            public readonly ArgumentInfo ArgumentInfo;
            public readonly string ClassName;
            public readonly string FirstArgument;
            public readonly string FirstParameter;
            public readonly string InterfaceFullName;
            public readonly string InterfaceName;
            public readonly INamedTypeSymbol InterfaceSymbol;
            public readonly int InvokeParameterCount;
            public readonly bool IsAction;
            public readonly bool IsFromFunctionPointer;
            public readonly IMethodSymbol Method;
            public readonly IReadOnlyList<MethodReference> References;

            public DescriptorArgs
            (
                IMethodSymbol method,
                in ArgumentInfo argumentInfo,
                int invokeParameterCount,
                bool isAction,
                bool isFromFunctionPointer,
                IReadOnlyList<MethodReference> references
            )
            {
                string identifier = isAction ? "Action" : "Func";
                string interfaceTypeArguments = GetInterfaceTypeArguments(method.ContainingType);
                ArgumentInfo = argumentInfo;
                ClassName = $"Native{identifier}_{Guid.NewGuid():N}";
                InterfaceName = $"INative{identifier}";
                InterfaceSymbol = method.ContainingType;
                InterfaceFullName = $"{InterfaceName}{interfaceTypeArguments}";
                InvokeParameterCount = invokeParameterCount;
                IsAction = isAction;
                IsFromFunctionPointer = isFromFunctionPointer;
                Method = method;
                References = references;
                switch (isFromFunctionPointer)
                {
                    case true:
                        FirstArgument = "functionPtr";
                        FirstParameter = "nint functionPtr";
                        break;
                    default:
                        FirstArgument = identifier.ToLower();
                        FirstParameter = $"{identifier}{interfaceTypeArguments} {FirstArgument}";
                        break;
                }
            }

            private static string GetInterfaceTypeArguments(INamedTypeSymbol interfaceSymbol)
            {
                if (interfaceSymbol.Arity == 0)
                {
                    return "";
                }
                return $"<{string.Join(", ", interfaceSymbol.TypeArguments.Select(x => x.ToDisplayString()))}>";
            }
        }
    }
}
