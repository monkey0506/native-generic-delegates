using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace NativeGenericDelegatesGenerator
{
    internal readonly struct MethodSymbolWithContext
    {
        public readonly GeneratorSyntaxContext Context;
        public readonly bool IsAction;
        public readonly IMethodSymbol MethodSymbol;

        public static ImmutableArray<MethodSymbolWithContext> GetSymbols(ImmutableArray<GeneratorSyntaxContext> contextArray, CancellationToken cancellationToken)
        {
            List<MethodSymbolWithContext> symbolsWithContext = new();
            foreach (var context in contextArray)
            {
                cancellationToken.ThrowIfCancellationRequested();
                IMethodSymbol? methodSymbol = (IMethodSymbol?)context.SemanticModel.GetSymbolInfo(((InvocationExpressionSyntax)context.Node).Expression, cancellationToken).Symbol;
                switch (methodSymbol?.Name)
                {
                    case Constants.FromActionIdentifer:
                    case Constants.FromFuncIdentifier:
                    case Constants.FromFunctionPointerIdentifier:
                        break;
                    default:
                        continue;
                }
                INamedTypeSymbol? interfaceSymbol = methodSymbol.ContainingType;
                bool isAction = interfaceSymbol?.Name == Constants.INativeActionIdentifier;
                // this may be overkill (and there may be a simpler way to validate this as one of the interfaces we define), but this should cover any erroneous matches
                if ((interfaceSymbol is null) || (interfaceSymbol.ContainingNamespace is null) || (interfaceSymbol.ContainingNamespace.ContainingNamespace is null) ||
                    (!interfaceSymbol.ContainingNamespace.ContainingNamespace.IsGlobalNamespace) || (interfaceSymbol.ContainingNamespace.Name != Constants.RootNamespace) ||
                    (interfaceSymbol.TypeKind != TypeKind.Interface) || !interfaceSymbol.IsGenericType || (interfaceSymbol.Arity == 0) ||
                    (!isAction && (interfaceSymbol.Name != Constants.INativeFuncIdentifier)) || (methodSymbol.Parameters.Length != (isAction ? 3 : 4)))
                {
                    continue;
                }
                if (methodSymbol.IsGenericMethod)
                {
                    if (methodSymbol.Arity != interfaceSymbol.Arity)
                    {
                        continue;
                    }
                    if (methodSymbol.TypeArguments.Where(x => x is not INamedTypeSymbol namedTypeArgument || namedTypeArgument.IsGenericType).Any())
                    {
                        continue;
                    }
                }
                if (interfaceSymbol.TypeArguments.Where(x => x is not INamedTypeSymbol namedTypeArgument || namedTypeArgument.IsGenericType).Any())
                {
                    continue;
                }
                symbolsWithContext.Add(new(methodSymbol, context, isAction));
            }
            return symbolsWithContext.ToImmutableArray();
        }

        public MethodSymbolWithContext(IMethodSymbol methodSymbol, GeneratorSyntaxContext context, bool isAction)
        {
            Context = context;
            IsAction = isAction;
            MethodSymbol = methodSymbol;
        }
    }
}
