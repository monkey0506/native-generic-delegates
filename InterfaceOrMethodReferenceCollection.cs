using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Monkeymoto.GeneratorUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Monkeymoto.NativeGenericDelegates
{
    internal readonly struct InterfaceOrMethodReferenceCollection :
        IEquatable<InterfaceOrMethodReferenceCollection>,
        IEnumerable<GenericSymbolReference>
    {
        private readonly int hashCode;
        private readonly ImmutableHashSet<GenericSymbolReference> references;

        public static bool operator ==
        (
            InterfaceOrMethodReferenceCollection left,
            InterfaceOrMethodReferenceCollection right
        ) => left.Equals(right);

        public static bool operator !=
        (
            InterfaceOrMethodReferenceCollection left,
            InterfaceOrMethodReferenceCollection right
        ) => !(left == right);

        private static void AddMethodReferences
        (
            ImmutableHashSet<GenericSymbolReference>.Builder references,
            GenericSymbolReferenceTree tree,
            IMethodSymbol methodSymbol,
            CancellationToken cancellationToken
        )
        {
            var methodReferences = tree.GetBranchesBySymbol(methodSymbol, cancellationToken);
            if (!methodReferences.Any())
            {
                return;
            }
            var roots = tree.GetBranchesBySymbol(methodSymbol.ContainingType, cancellationToken);
            if (!roots.Any())
            {
                return;
            }
            var methodSymbolsToConstruct = new HashSet<IMethodSymbol>(SymbolEqualityComparer.Default);
            foreach (var root in roots.Select(static x => (INamedTypeSymbol)x.Symbol))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var methodSymbolToConstruct = root.GetMembers(methodSymbol.Name)
                    .Where
                    (
                        x => SymbolEqualityComparer.Default.Equals
                        (
                            x.OriginalDefinition,
                            methodSymbol.OriginalDefinition
                        )
                    )
                    .Cast<IMethodSymbol>()
                    .Single();
                _ = methodSymbolsToConstruct.Add(methodSymbolToConstruct);
            }
            var constructedMethodSymbols = new HashSet<IMethodSymbol>(SymbolEqualityComparer.Default);
            constructedMethodSymbols.UnionWith(methodReferences.SelectMany
            (
                x => methodSymbolsToConstruct.Select
                (
                    y => y.Construct([.. ((IMethodSymbol)x.Symbol).TypeArguments])
                )
            ));
            foreach (var methodReference in methodReferences)
            {
                cancellationToken.ThrowIfCancellationRequested();
                switch (methodReference.Node)
                {
                    case IdentifierNameSyntax identifierName:
                        references.UnionWith
                        (
                            constructedMethodSymbols.Select
                            (
                                x => new GenericSymbolReference
                                (
                                    x,
                                    methodReference.SemanticModel,
                                    identifierName
                                )
                            )
                        );
                        break;
                    case InvocationExpressionSyntax invocationExpression:
                        references.UnionWith
                        (
                            constructedMethodSymbols.Select
                            (
                                x => new GenericSymbolReference
                                (
                                    x,
                                    methodReference.SemanticModel,
                                    invocationExpression
                                )
                            )
                        );
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
        }

        public static IncrementalValueProvider<InterfaceOrMethodReferenceCollection> GetReferences
        (
            IncrementalGeneratorInitializationContext context,
            IncrementalValueProvider<InterfaceOrMethodSymbolCollection> symbolsProvider
        )
        {
            var treeProvider = GenericSymbolReferenceTree.FromIncrementalGeneratorInitializationContext(context);
            return symbolsProvider.Combine(treeProvider).Select(static (x, cancellationToken) =>
            {
                var symbols = x.Left;
                using var tree = x.Right; // Dispose tree after we extract the symbol references we need
                var references = ImmutableHashSet.CreateBuilder<GenericSymbolReference>();
                foreach (var symbol in symbols)
                {
                    switch (symbol)
                    {
                        case INamedTypeSymbol:
                            references.UnionWith(tree.GetBranchesBySymbol(symbol, cancellationToken));
                            break;
                        case IMethodSymbol methodSymbol:
                            AddMethodReferences(references, tree, methodSymbol, cancellationToken);
                            break;
                        default:
                            throw new UnreachableException();
                    }
                }
                return new InterfaceOrMethodReferenceCollection(references.ToImmutable());
            });
        }

        private InterfaceOrMethodReferenceCollection(ImmutableHashSet<GenericSymbolReference> references)
        {
            this.references = references;
            hashCode = Hash.Combine(references);
        }

        public override bool Equals(object? obj) => obj is InterfaceOrMethodReferenceCollection other && Equals(other);
        public bool Equals(InterfaceOrMethodReferenceCollection other) => references.SetEquals(other.references);
        public IEnumerator<GenericSymbolReference> GetEnumerator() => references.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public override int GetHashCode() => hashCode;
    }
}
