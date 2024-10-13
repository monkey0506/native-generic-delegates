using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Monkeymoto.GeneratorUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Monkeymoto.NativeGenericDelegates
{
    internal readonly struct InterfaceReferenceCollection :
        IEquatable<InterfaceReferenceCollection>,
        IEnumerable<GenericSymbolReference>
    {
        private readonly int hashCode;
        private readonly ImmutableHashSet<GenericSymbolReference> interfaceReferences;
        private readonly ImmutableHashSet<GenericSymbolReference> methodReferences;

        public static bool operator ==(InterfaceReferenceCollection left, InterfaceReferenceCollection right) =>
            left.Equals(right);
        public static bool operator !=(InterfaceReferenceCollection left, InterfaceReferenceCollection right) =>
            !(left == right);

        public static IncrementalValueProvider<InterfaceReferenceCollection> GetReferences
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
                var interfaceReferences = ImmutableHashSet.CreateBuilder<GenericSymbolReference>();
                var methodReferences = ImmutableHashSet.CreateBuilder<GenericSymbolReference>();
                foreach (var symbol in symbols)
                {
                    switch (symbol)
                    {
                        case INamedTypeSymbol:
                            interfaceReferences.UnionWith(tree.GetBranchesBySymbol(symbol, cancellationToken));
                            break;
                        case IMethodSymbol methodSymbol:
                            methodReferences.UnionWith(tree.GetBranchesBySymbol(symbol, cancellationToken));
                            break;
                        default:
                            throw new UnreachableException();
                    }
                }
                return new InterfaceReferenceCollection
                (
                    interfaceReferences.ToImmutable(),
                    methodReferences.ToImmutable()
                );
            });
        }

        private InterfaceReferenceCollection
        (
            ImmutableHashSet<GenericSymbolReference> interfaceReferences,
            ImmutableHashSet<GenericSymbolReference> methodReferences
        )
        {
            this.interfaceReferences = interfaceReferences;
            this.methodReferences = methodReferences;
            hashCode = Hash.Combine(interfaceReferences, methodReferences);
        }

        public override bool Equals(object? obj) => obj is InterfaceReferenceCollection other && Equals(other);
        public bool Equals(InterfaceReferenceCollection other) =>
            interfaceReferences.SetEquals(other.interfaceReferences) &&
            methodReferences.SetEquals(other.methodReferences);
        public IEnumerator<GenericSymbolReference> GetEnumerator() => interfaceReferences.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public override int GetHashCode() => hashCode;

        public IReadOnlyCollection<GenericSymbolReference> GetGenericMethodReferences
        (
            IMethodSymbol methodSymbol,
            InvocationExpressionSyntax invocationExpression
        )
        {
            methodSymbol = methodSymbol.OriginalDefinition;
            return methodReferences.Where
            (
                x => SymbolEqualityComparer.Default.Equals(x.Symbol, methodSymbol) &&
                    x.Node.IsEquivalentTo(invocationExpression)
            ).ToImmutableList();
        }
    }
}
