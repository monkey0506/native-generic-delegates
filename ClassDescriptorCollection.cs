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
                    var classDescriptor = new ClassDescriptor
                    (
                        kv.Key.Method,
                        kv.Key.ArgumentInfo,
                        kv.Key.InvokeParameterCount,
                        kv.Key.IsAction,
                        kv.Key.IsFromFunctionPointer,
                        kv.Value.AsReadOnly()
                    );
                    list.Add(classDescriptor);
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
            Dictionary<int, List<InterceptorDescriptor>> openInterceptors = [];
            foreach (var openDescriptor in descriptors.Where(x => x.Interceptor.OpenReferenceAttributes.Any()))
            {
                var list = openInterceptors.GetOrCreate(openDescriptor.Interceptor.MethodHash);
                list!.Add(openDescriptor.Interceptor);
            }
            foreach (var kv in openInterceptors)
            {
                _ = sb.AppendLine().AppendLine(GetOpenInterceptorSourceText(kv.Value.AsReadOnly()));
            }
            return sb.ToString();
        }

        private static string GetOpenInterceptorSourceText(IReadOnlyList<InterceptorDescriptor> openInterceptors)
        {
            var first = openInterceptors.First();
            var interfaceName = first.InterfaceName;
            var methodName = first.InterceptsMethod;
            var typeParameters = first.TypeParameters;
            var parameters = first.Parameters;
            var methodHash = first.MethodHash;
            var sb = new StringBuilder("    ").AppendLine
            (
 $@"file static class NativeGenericDelegates_{(methodHash < 0 ? $"S{-methodHash}" : $"U{methodHash}")}
    {{"
            );
            HashSet<string> attributeSet = [];
            foreach (var descriptor in openInterceptors)
            {
                foreach (var attribute in descriptor.OpenReferenceAttributes)
                {
                    _ = attributeSet.Add(attribute);
                }
            }
            foreach (var attribute in attributeSet)
            {
                _ = sb.Append("        ").Append(attribute).AppendLine();
            }
            _ = sb.Append("        ").AppendLine
            (
     $@"[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static {interfaceName} {methodName}{typeParameters}
        (
            {parameters}
        )
        {{"
            );
            for (int i = 0; i < openInterceptors.Count; ++i)
            {
                var typeArguments = openInterceptors[i].TypeArguments;
                if (typeArguments.Count == 1)
                {
                    _ = sb.Append($"            if (typeof(X) == typeof({typeArguments[0]}))");
                }
                else
                {
                    _ = sb.Append($"            if{Constants.NewLine}            ({Constants.NewLine}");
                    for (int j = 0, k = 1; j < typeArguments.Count; ++j, ++k)
                    {
                        _ = sb.Append($"                (typeof(X{k}) == typeof({typeArguments[j]}))");
                        if (k != typeArguments.Count)
                        {
                            _ = sb.AppendLine(" &&");
                        }
                    }
                    _ = sb.Append("            )");
                }
                _ = sb.AppendLine().Append("            ").AppendLine
                (
         $@"{{
                return ({interfaceName})(object)(new {openInterceptors[i].ClassName}({openInterceptors[i].FirstArgument}));
            }}"
                );
            }
            _ = sb.Append
            (
                $"            throw new NotImplementedException();{Constants.NewLine}        }}{Constants.NewLine}    }}"
            );
            return sb.ToString();
        }
    }
}
