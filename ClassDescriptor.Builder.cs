using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly partial struct ClassDescriptor
    {
        internal readonly ref struct Builder
        {
            public readonly ArgumentInfo ArgumentInfo;
            public readonly string ClassName;
            public readonly string FirstArgument;
            public readonly string FirstParameter;
            public readonly string Identifier;
            public readonly INamedTypeSymbol InterfaceSymbol;
            public readonly string InterfaceTypeArgumentsSourceText;
            public readonly int InvokeParameterCount;
            public readonly bool IsAction;
            public readonly bool IsFromFunctionPointer;
            public readonly IMethodSymbol Method;
            public readonly IReadOnlyList<MethodReference> References;

            public Builder
            (
                IMethodSymbol method,
                in ArgumentInfo argumentInfo,
                int invokeParameterCount,
                bool isAction,
                bool isFromFunctionPointer,
                IReadOnlyList<MethodReference> references
            )
            {
                ArgumentInfo = argumentInfo;
                Identifier = isAction ? "Action" : "Func";
                ClassName = $"Native{Identifier}_{Guid.NewGuid():N}";
                InterfaceSymbol = method.ContainingType;
                InterfaceTypeArgumentsSourceText = GetInterfaceTypeArguments(InterfaceSymbol);
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
                        FirstArgument = Identifier.ToLower();
                        FirstParameter = $"{Identifier}{InterfaceTypeArgumentsSourceText} {FirstArgument}";
                        break;
                }
            }

            public ClassDescriptor ToDescriptor() => new(in this);

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
