using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class OpenGenericInterceptors
    {
        public sealed class Builder
        {
            private readonly Dictionary<Key, ImmutableHashSet<string>.Builder> attributes = [];
            private readonly Dictionary<Key, ImmutableList<ImplementationClass>.Builder> implementationClasses = [];

            public void Add
            (
                ImplementationClass implementationClass,
                IReadOnlyList<MethodReference> methodReferences
            )
            {
                var attributesHashSet = attributes.GetOrCreate
                (
                    new Key(implementationClass),
                    ImmutableHashSet.CreateBuilder<string>
                );
                var implementationClassesList = implementationClasses.GetOrCreate
                (
                    new Key(implementationClass),
                    ImmutableList.CreateBuilder<ImplementationClass>
                );
                attributesHashSet!.UnionWith(methodReferences.Select(static x => x.InterceptorAttributeSourceText));
                implementationClassesList!.Add(implementationClass);
            }

            public OpenGenericInterceptors ToCollection()
            {
                var attributesBuilder = ImmutableDictionary.CreateBuilder<Key, ImmutableHashSet<string>>();
                var implementationClassesBuilder =
                    ImmutableDictionary.CreateBuilder<Key, ImmutableList<ImplementationClass>>();
                foreach (var kv in attributes)
                {
                    attributesBuilder.Add(new(kv.Key, kv.Value.ToImmutable()));
                }
                foreach (var kv in implementationClasses)
                {
                    implementationClassesBuilder.Add(new(kv.Key, kv.Value.ToImmutable()));
                }
                return new(attributesBuilder.ToImmutable(), implementationClassesBuilder.ToImmutable());
            }
        }
    }
}
