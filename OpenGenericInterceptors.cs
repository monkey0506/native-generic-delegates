using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class OpenGenericInterceptors : IEquatable<OpenGenericInterceptors>
    {
        private readonly ImmutableDictionary<Key, ImmutableHashSet<string>> attributes;
        private readonly int hashCode;
        private readonly ImmutableDictionary<Key, ImmutableList<ImplementationClass>> implementationClasses;

        public string SourceText { get; }

        public static bool operator ==(OpenGenericInterceptors? left, OpenGenericInterceptors? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(OpenGenericInterceptors? left, OpenGenericInterceptors? right) =>
            !(left == right);

        private OpenGenericInterceptors
        (
            ImmutableDictionary<Key, ImmutableHashSet<string>> attributes,
            ImmutableDictionary<Key, ImmutableList<ImplementationClass>> implementationClasses
        )
        {
            this.attributes = attributes;
            this.implementationClasses = implementationClasses;
            SourceText = GetSourceText();
            hashCode = SourceText.GetHashCode();
        }

        public override bool Equals(object? obj) => obj is OpenGenericInterceptors other && Equals(other);
        public bool Equals(OpenGenericInterceptors? other) => (other is not null) &&
            (SourceText == other.SourceText);
        public override int GetHashCode() => hashCode;

        private string GetSourceText()
        {
            var sb = new StringBuilder();
            foreach (var kv in implementationClasses)
            {
                Debug.Assert(attributes.ContainsKey(kv.Key));
                var first = kv.Value.First();
                var interfaceTypeParameters =
                    ClosedGenericInterceptor.GetTypeParameters(first.Method.ContainingInterface.Arity, 0);
                var interfaceName = $"{first.Method.ContainingInterface.Name}{interfaceTypeParameters}";
                var methodHash = kv.Key.GetHashCode();
                var methodName = first.Method.Name;
                var parameters = first.Method.Parameters;
                var typeParameters = ClosedGenericInterceptor.GetTypeParameters
                (
                    first.Method.ContainingInterface.Arity,
                    first.Method.Arity
                );
                _ = sb.AppendLine().Append("    ").AppendLine
                (
 $@"file static class NativeGenericDelegates_{(methodHash < 0 ? $"${-methodHash}" : $"U{methodHash}")}
    {{"
                );
                foreach (var attribute in attributes[kv.Key])
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
                for (int i = 0; i < kv.Value.Count; ++i)
                {
                    var typeArguments = kv.Value[i].Method.ContainingInterface.TypeArguments;
                    if (typeArguments.Count == 1)
                    {
                        _ = sb.Append($"            if (typeof(X) == typeof({typeArguments[0]}))");
                    }
                    else
                    {
                        _ = sb.Append($"            if{Constants.NewLineIndent3}({Constants.NewLine}");
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
                return ({interfaceName})(object)(new {kv.Value[i].ClassName}({kv.Value[i].Method.FirstParameterName}));
            }}"
                    );
                }
                _ = sb.Append
                (
                    $"            throw new NotImplementedException();{Constants.NewLineIndent2}}}" +
                    $"{Constants.NewLineIndent1}}}{Constants.NewLine}"
                );
            }
            return sb.ToString();
        }
    }
}
