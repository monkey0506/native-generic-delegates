using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed class ClosedGenericInterceptor : IEquatable<ClosedGenericInterceptor>
    {
        private readonly int hashCode;

        public ImplementationClass ImplementationClass { get; }
        public ImmutableArray<InterceptedMethodReference> InterceptedMethodReferences { get; }
        public MethodDescriptor InterceptsMethod { get; }
        public string SourceText { get; }

        public static bool operator ==(ClosedGenericInterceptor? left, ClosedGenericInterceptor? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(ClosedGenericInterceptor? left, ClosedGenericInterceptor? right) =>
            !(left == right);

        public ClosedGenericInterceptor
        (
            MethodDescriptor interceptsMethod,
            ImplementationClass implementationClass,
            IReadOnlyList<MethodReference> methodReferences
        )
        {
            ImplementationClass = implementationClass;
            InterceptsMethod = interceptsMethod;
            InterceptedMethodReferences = [.. methodReferences.Select(x => new InterceptedMethodReference(x, this))];
            SourceText = GetSourceText();
            hashCode = SourceText.GetHashCode();
        }

        public override bool Equals(object? obj) => obj is ClosedGenericInterceptor other && Equals(other);
        public bool Equals(ClosedGenericInterceptor? other) => (other is not null) && (SourceText == other.SourceText);
        public override int GetHashCode() => hashCode;

        private string GetSourceText()
        {
            var sb = new StringBuilder($"{Constants.NewLineIndent2}");
            foreach (var reference in InterceptedMethodReferences.Select(x => x.MethodReference))
            {
                _ = sb.Append($"{Constants.NewLineIndent2}").Append(reference.InterceptorAttributeSourceText);
            }
            var method = InterceptsMethod;
            var typeParameters = GetTypeParameters(method.ContainingInterface.Arity, method.Arity);
            _ = sb.Append
            (
     $@"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static {method.ContainingInterface.FullName} {method.Name}{typeParameters}
        (
            {method.Parameters}
        )
        {{
            return new {ImplementationClass.ClassName}({method.FirstParameterName});
        }}"
            );
            return sb.ToString();
        }
        internal static string GetTypeParameters(int interfaceArity, int methodArity)
        {
            if (interfaceArity == 0)
            {
                return string.Empty;
            }
            int arity = interfaceArity + methodArity;
            return arity == 1 ?
                "<X>" :
                $"<{string.Join(", ", Enumerable.Range(1, arity).Select(x => $"X{x}"))}>";
        }
    }
}
