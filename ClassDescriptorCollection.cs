using Microsoft.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                return x.ArgumentInfo == y.ArgumentInfo &&
                    SymbolEqualityComparer.Default.Equals(x.InterfaceSymbol, y.InterfaceSymbol) &&
                    SymbolEqualityComparer.Default.Equals(x.Method, y.Method);
            }

            public int GetHashCode(MethodReference obj)
            {
                return Hash.Combine(obj.ArgumentInfo, obj.Method);
            }
        }

        public static IncrementalValueProvider<ClassDescriptorCollection> GetDescriptors
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

        public string GetOpenInterceptorsSourceText()
        {
            var sb = new StringBuilder();
            Dictionary<int, List<ClassDescriptor>> openDescriptors = [];
            foreach (var openDescriptor in descriptors.Where(x => x.Interceptor.OpenReferenceAttributes.Any()))
            {
                var list = openDescriptors.GetOrCreate(openDescriptor.Interceptor.MethodHash);
                list!.Add(openDescriptor);
            }
            foreach (var kv in openDescriptors)
            {
                _ = sb.AppendLine().AppendLine(GetOpenInterceptorSourceText(kv.Value.AsReadOnly()));
            }
            return sb.ToString();
        }

        private static string GetOpenInterceptorSourceText
        (
            IReadOnlyList<ClassDescriptor> openDescriptors
        )
        {
            var first = openDescriptors.First();
            var interfaceOriginalName = first.Interface.OriginalName.Replace('T', 'X');
            var methodName = first.Interceptor.InterceptsMethod;
            var typeParameters = first.Interceptor.TypeParameters;
            var parameters = first.Interceptor.Parameters;
            var methodHash = first.Interceptor.MethodHash;
            var sb = new StringBuilder
            (
$@"    file static class NativeGenericDelegates_{(methodHash < 0 ? $"S{-methodHash}" : $"U{methodHash}")}
    {{
        "
            );
            HashSet<string> attributeSet = [];
            foreach (var descriptor in openDescriptors)
            {
                foreach (var attribute in descriptor.Interceptor.OpenReferenceAttributes)
                {
                    _ = attributeSet.Add(attribute);
                }
            }
            foreach (var attribute in attributeSet)
            {
                _ = sb.Append(attribute).Append
                (
$@"
        "
                );
            }
            _ = sb.Append
            (
$@"[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static {interfaceOriginalName} {methodName}{typeParameters}
        (
            {parameters}
        )
        {{
"
            );
            int x = -1;
            foreach (var typeArguments in openDescriptors.Select(x => x.Interceptor.TypeArguments))
            {
                ++x;
                if (typeArguments.Count == 1)
                {
                    _ = sb.Append
                    (
$@"            if (typeof(X) == typeof({typeArguments[0]}))"
                    );
                }
                else
                {
                    _ = sb.Append
                    (
$@"            if
            (
"
                    );
                    for (int i = 0, j = 1; i < typeArguments.Count; ++i, ++j)
                    {
                        _ = sb.Append
                        (
$@"                (typeof(X{j}) == typeof({typeArguments[i]}))"
                        );
                        if (j != typeArguments.Count)
                        {
                            _ = sb.AppendLine(" &&");
                        }
                    }
                    _ = sb.Append
                    (
$@"            )"
                    );
                }
                _ = sb.Append
                (
$@"
            {{
                return ({interfaceOriginalName})(object)(new {openDescriptors[x].Name}({openDescriptors[x].Interceptor.FirstArgument}));
            }}
"
                );
            }
            _ = sb.Append
            (
$@"            throw new NotImplementedException();
        }}
    }}"
            );
            return sb.ToString();
        }
    }
}
