using Microsoft.CodeAnalysis;
using Monkeymoto.GeneratorUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct InterfaceOrMethodReferenceCollection :
        IEquatable<InterfaceOrMethodReferenceCollection>,
        IEnumerable<GenericSymbolReference>
    {
        private readonly int hashCode;
        private readonly ImmutableHashSet<GenericSymbolReference> references;

        public static bool operator ==(InterfaceOrMethodReferenceCollection left, InterfaceOrMethodReferenceCollection right) =>
            left.Equals(right);
        public static bool operator !=(InterfaceOrMethodReferenceCollection left, InterfaceOrMethodReferenceCollection right) =>
            !(left == right);

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
                HashSet<GenericSymbolReference> references = [];
                foreach (var symbol in symbols)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    references.UnionWith(tree.GetBranchesBySymbol(symbol, cancellationToken));
                }
                return new InterfaceOrMethodReferenceCollection(references.ToImmutableHashSet());
            });
        }

        private InterfaceOrMethodReferenceCollection(ImmutableHashSet<GenericSymbolReference> references)
        {
            this.references = references;
            hashCode = Hash.Combine(references);
        }

        public override bool Equals(object obj)
        {
            return obj is InterfaceOrMethodReferenceCollection other && Equals(other);
        }

        public bool Equals(InterfaceOrMethodReferenceCollection other)
        {
            return references.SetEquals(other.references);
        }

        public IEnumerator<GenericSymbolReference> GetEnumerator()
        {
            return references.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
