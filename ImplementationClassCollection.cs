using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class ImplementationClassCollection :
        IEquatable<ImplementationClassCollection>,
        IEnumerable<ImplementationClass>
    {
        private readonly int hashCode;
        private readonly ImmutableList<ImplementationClass> implementationClasses;
        private readonly OpenGenericInterceptors openGenericInterceptors;

        public static bool operator ==(ImplementationClassCollection left, ImplementationClassCollection right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(ImplementationClassCollection left, ImplementationClassCollection right) =>
            !(left == right);

        public static IncrementalValueProvider<ImplementationClassCollection> GetImplementationClasses
        (
            IncrementalValueProvider<MethodReferenceCollection> methodReferencesProvider
        ) => methodReferencesProvider.Select((methodReferences, cancellationToken) =>
        {
            var dictionary = new Dictionary<Key, List<MethodReference>>();
            foreach (var methodReference in methodReferences)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var list = dictionary.GetOrCreate(methodReference);
                list!.Add(methodReference);
            }
            var openGenericInterceptorsBuilder = new OpenGenericInterceptors.Builder();
            return new ImplementationClassCollection
            (
                openGenericInterceptorsBuilder,
                [
                    .. dictionary.Select
                    (
                        x => new ImplementationClass
                        (
                            openGenericInterceptorsBuilder,
                            x.Key.MethodReference.Method,
                            x.Key.MethodReference.IsInterfaceOrMethodOpenGeneric,
                            x.Key.MethodReference.Marshalling,
                            x.Value.AsReadOnly()
                        )
                    )
                ]
            );
        });

        private ImplementationClassCollection
        (
            OpenGenericInterceptors.Builder openGenericInterceptorsBuilder,
            ImmutableList<ImplementationClass> implementationClasses
        )
        {
            this.implementationClasses = implementationClasses;
            openGenericInterceptors = openGenericInterceptorsBuilder.ToCollection();
            hashCode = Hash.Combine(this.implementationClasses, openGenericInterceptors);
        }

        public override bool Equals(object? obj) => obj is ImplementationClassCollection other && Equals(other);
        public bool Equals(ImplementationClassCollection? other) => (other is not null) &&
            implementationClasses.SequenceEqual(other.implementationClasses) &&
            (openGenericInterceptors == other.openGenericInterceptors);
        public IEnumerator<ImplementationClass> GetEnumerator() => implementationClasses.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public override int GetHashCode() => hashCode;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetOpenGenericInterceptorsSourceText() => openGenericInterceptors.SourceText;
    }
}
