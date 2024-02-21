using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct MethodReferenceCollection : IEnumerable<MethodReference>, IEquatable<MethodReferenceCollection>
    {
        private readonly int hash;

        public readonly ImmutableHashSet<MethodReference> References;

        public static bool operator ==(MethodReferenceCollection left, MethodReferenceCollection right) => left.Equals(right);
        public static bool operator !=(MethodReferenceCollection left, MethodReferenceCollection right) => !(left == right);

        public static IncrementalValueProvider<(MethodReferenceCollection, IReadOnlyList<Diagnostic>)> GetReferencesOrDiagnostics
        (
            IncrementalValueProvider<InterfaceOrMethodReferenceCollection> references
        )
        {
            return references.Select(static (references, cancellationToken) =>
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
                return (new MethodReferenceCollection(methodReferences), (IReadOnlyList<Diagnostic>)diagnostics.AsReadOnly());
            });
        }

        private MethodReferenceCollection(HashSet<MethodReference> references)
        {
            References = references.ToImmutableHashSet();
            hash = Hash.Combine(References);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodReferenceCollection other && Equals(other);
        }

        public bool Equals(MethodReferenceCollection other)
        {
            return References.SetEquals(other.References);
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public IEnumerator<MethodReference> GetEnumerator()
        {
            return References.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
