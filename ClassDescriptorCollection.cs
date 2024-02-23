using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct ClassDescriptorCollection : IEnumerable<ClassDescriptor>
    {
        private readonly IReadOnlyList<ClassDescriptor> descriptors;

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

        public static IncrementalValueProvider<ClassDescriptorCollection> GetImplementations
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
                var list = new List<ClassDescriptor>(dictionary.Keys.Count);
                foreach (var kv in dictionary)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var builder = new ClassDescriptor.Builder
                    (
                        kv.Key.Method,
                        kv.Key.ArgumentInfo,
                        kv.Key.InvokeParameterCount,
                        kv.Key.IsAction,
                        kv.Key.IsFromFunctionPointer,
                        kv.Value.AsReadOnly()
                    );
                    list.Add(builder.ToDescriptor());
                }
                return new ClassDescriptorCollection(list.AsReadOnly());
            });
        }

        private ClassDescriptorCollection(IReadOnlyList<ClassDescriptor> descriptors)
        {
            this.descriptors = descriptors;
        }

        public IEnumerator<ClassDescriptor> GetEnumerator()
        {
            return descriptors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
