using Microsoft.CodeAnalysis;
using Monkeymoto.GeneratorUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

        public static IncrementalValueProvider<InterfaceOrMethodReferenceCollection> GetInterfaceOrMethodReferences
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
                references.UnionWith(symbols.SelectMany(x => tree.GetBranchesBySymbol(x, cancellationToken)));
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
