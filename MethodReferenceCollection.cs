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

        public IReadOnlyList<Diagnostic> Diagnostics { get; }

        public static bool operator ==(MethodReferenceCollection left, MethodReferenceCollection right) =>
            left.Equals(right);
        public static bool operator !=(MethodReferenceCollection left, MethodReferenceCollection right) =>
            !(left == right);

        public static IncrementalValueProvider<MethodReferenceCollection> GetMethodReferences
        (
            IncrementalValueProvider<InterfaceOrMethodReferenceCollection> interfaceOrMethodReferencesProvider
        ) => interfaceOrMethodReferencesProvider.Select(static (interfaceOrMethodReferences, cancellationToken) =>
        {
            var builder = ImmutableHashSet.CreateBuilder<MethodReference>();
            var diagnostics = new List<Diagnostic>();
            foreach (var interfaceOrMethodReference in interfaceOrMethodReferences)
            {
                var methodReference =
                    MethodReference.GetReference(interfaceOrMethodReference, diagnostics, cancellationToken);
                if (methodReference is not null)
                {
                    _ = builder.Add(methodReference);
                }
            }
            return new MethodReferenceCollection
            (
                builder.ToImmutable(),
                diagnostics.AsReadOnly()
            );
        });

        private MethodReferenceCollection
        (
            ImmutableHashSet<MethodReference> references,
            IReadOnlyList<Diagnostic> diagnostics
        )
        {
            this.references = references;
            Diagnostics = diagnostics;
            hashCode = Hash.Combine(references);
        }

        public override bool Equals(object? obj) => obj is MethodReferenceCollection other && Equals(other);
        public bool Equals(MethodReferenceCollection other) => references.SetEquals(other.references);
        public IEnumerator<MethodReference> GetEnumerator() => references.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public override int GetHashCode() => hashCode;
    }
}
