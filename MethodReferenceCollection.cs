using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct MethodReferenceCollection : IEquatable<MethodReferenceCollection>, IEnumerable<MethodReference>
    {
        private readonly int hash;
        private readonly ImmutableHashSet<MethodReference> references;

        public static bool operator ==(MethodReferenceCollection left, MethodReferenceCollection right) => left.Equals(right);
        public static bool operator !=(MethodReferenceCollection left, MethodReferenceCollection right) => !(left == right);

        public static IncrementalValueProvider<(MethodReferenceCollection, IReadOnlyList<Diagnostic>)> GetReferencesOrDiagnostics
        (
            IncrementalValueProvider<InterfaceOrMethodReferenceCollection> referencesProvider
        )
        {
            return referencesProvider.Select(static (references, cancellationToken) =>
            {
                HashSet<MethodReference> methodReferences = [];
                List<Diagnostic> diagnostics = [];
                foreach (var reference in references)
                {
                    var methodReference = MethodReference.GetReference(reference, diagnostics, cancellationToken);
                    if (methodReference is not null)
                    {
                        _ = methodReferences.Add(methodReference.Value);
                    }
                }
                return
                (
                    new MethodReferenceCollection(methodReferences.ToImmutableHashSet()),
                    (IReadOnlyList<Diagnostic>)diagnostics.AsReadOnly()
                );
            });
        }

        private MethodReferenceCollection(ImmutableHashSet<MethodReference> references)
        {
            this.references = references;
            hash = Hash.Combine(references);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodReferenceCollection other && Equals(other);
        }

        public bool Equals(MethodReferenceCollection other)
        {
            return references.SetEquals(other.references);
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public IEnumerator<MethodReference> GetEnumerator()
        {
            return references.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
