using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct MethodImplementationCollection : IEquatable<MethodImplementationCollection>
    {
        private readonly int hash;

        public readonly IReadOnlyList<MethodImplementation> Implementations;

        public static bool operator ==(MethodImplementationCollection left, MethodImplementationCollection right) =>
            left.Equals(right);
        public static bool operator !=(MethodImplementationCollection left, MethodImplementationCollection right) =>
            !(left == right);

        private sealed class MethodReferenceComparer : IEqualityComparer<MethodReference>
        {
            public static readonly MethodReferenceComparer Instance = new();

            private MethodReferenceComparer() { }

            public bool Equals(MethodReference x, MethodReference y)
            {
                return x.ArgumentInfo == y.ArgumentInfo && SymbolEqualityComparer.Default.Equals(x.Method, y.Method);
            }

            public int GetHashCode(MethodReference obj)
            {
                return Hash.Combine(obj.ArgumentInfo, obj.Method);
            }
        }

        public static IncrementalValueProvider<MethodImplementationCollection> GetImplementations
        (
            IncrementalValueProvider<MethodReferenceCollection> methodReferences
        )
        {
            return methodReferences.Select(static (methodReferences, cancellationToken) =>
            {
                var dictionary = new Dictionary<MethodReference, List<MethodReference>>(MethodReferenceComparer.Instance);
                foreach (var methodReference in methodReferences)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var methodList = dictionary.GetOrCreate(methodReference);
                    methodList!.Add(methodReference);
                }
                var list = new List<MethodImplementation>(dictionary.Keys.Count);
                foreach (var kv in dictionary)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    list.Add
                    (
                        new
                        (
                            kv.Key.ArgumentInfo,
                            kv.Key.InvokeParameterCount,
                            kv.Key.IsAction,
                            kv.Key.IsFromFunctionPointer,
                            kv.Key.Method,
                            kv.Value.AsReadOnly()
                        )
                    );
                }
                return new MethodImplementationCollection(list.AsReadOnly());
            });
        }

        private MethodImplementationCollection(IReadOnlyList<MethodImplementation> implementations)
        {
            Implementations = implementations;
            hash = Hash.Combine(Implementations);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodImplementationCollection other && Equals(other);
        }

        public bool Equals(MethodImplementationCollection other)
        {
            return Implementations.SequenceEqual(other.Implementations);
        }

        public override int GetHashCode()
        {
            return hash;
        }
    }
}
