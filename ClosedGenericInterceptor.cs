using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
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
                _ = sb.Append(reference.InterceptorAttributeSourceText).Append($"{Constants.NewLineIndent2}");
            }
            var method = InterceptsMethod;
            var typeParameters = Constants.InterceptorTypeParameters[method.ContainingInterface.Arity];
            typeParameters = typeParameters.Length != 0 ?
                $"<{typeParameters}>" :
                typeParameters;
            _ = sb.Append
            (
     $@"[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static {method.ContainingInterface.FullName} {method.Name}{typeParameters}
        (
            {method.Parameters}
        ){Constants.InterceptorAntiConstraints[method.ContainingInterface.Arity]}
        {{"
            );
            if (ImplementationClass.Marshalling.StaticCallingConvention is not null)
            {
                _ = sb.Append
                (
     $@"
            return new {ImplementationClass.ClassName}({method.FirstParameterName});
        }}"
                );
            }
            else
            {
                _ = sb.Append
                (
     $@"
            return callingConvention switch
            {{
                CallingConvention.Cdecl => ({method.ContainingInterface.FullName})new {ImplementationClass.ClassName}_{nameof(CallingConvention.Cdecl)}({method.FirstParameterName}),
                CallingConvention.StdCall => new {ImplementationClass.ClassName}_{nameof(CallingConvention.StdCall)}({method.FirstParameterName}),
                CallingConvention.ThisCall => new {ImplementationClass.ClassName}_{nameof(CallingConvention.ThisCall)}({method.FirstParameterName}),
                CallingConvention.Winapi => new {ImplementationClass.ClassName}_{nameof(CallingConvention.Winapi)}({method.FirstParameterName}),
                _ => throw new NotImplementedException()
            }};
        }}"
                );
            }
            return sb.ToString();
        }
    }
}
