using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Monkeymoto.NativeGenericDelegates
{
    internal readonly struct MethodReferenceCollection :
        IEquatable<MethodReferenceCollection>,
        IEnumerable<MethodReference>
    {
        private readonly int hashCode;
        private readonly ImmutableHashSet<MethodReference> references;

        public static bool operator ==(MethodReferenceCollection left, MethodReferenceCollection right) =>
            left.Equals(right);
        public static bool operator !=(MethodReferenceCollection left, MethodReferenceCollection right) =>
            !(left == right);

        public static IncrementalValueProvider<MethodReferenceCollection> GetReferences
        (
            IncrementalValueProvider<InterfaceReferenceCollection> interfaceReferencesProvider
        ) => interfaceReferencesProvider.Select(static (interfaceReferences, cancellationToken) =>
        {
            var builder = ImmutableHashSet.CreateBuilder<MethodReference>();
            foreach (var interfaceReference in interfaceReferences)
            {
                var methodReferences = MethodReference.GetReferences
                (
                    interfaceReference,
                    interfaceReferences.GetGenericMethodReferences,
                    cancellationToken
                );
                if (methodReferences is not null)
                {
                    builder.UnionWith(methodReferences);
                }
            }
            return new MethodReferenceCollection(builder.ToImmutable());
        });

        private MethodReferenceCollection(ImmutableHashSet<MethodReference> references)
        {
            this.references = references;
            hashCode = Hash.Combine(references);
        }

        public override bool Equals(object? obj) => obj is MethodReferenceCollection other && Equals(other);
        public bool Equals(MethodReferenceCollection other) => references.SetEquals(other.references);
        public IEnumerator<MethodReference> GetEnumerator() => references.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public override int GetHashCode() => hashCode;
    }
}
