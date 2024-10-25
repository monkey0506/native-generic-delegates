using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Monkeymoto.GeneratorUtils;
using System;
using System.Linq;
using System.Threading;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed class InterfaceReference : IEquatable<InterfaceReference>
    {
        private readonly int hashCode;

        public InterfaceDescriptor Interface { get; }
        public int InvocationArgumentCount { get; }
        public bool IsInterfaceOrMethodOpenGeneric { get; }
        public MethodDescriptor Method { get; }
        public IInvocationOperation MethodInvocation { get; }

        public static bool operator ==(InterfaceReference? left, InterfaceReference? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(InterfaceReference? left, InterfaceReference? right) =>
            !(left == right);

        public static InterfaceReference? GetReference
        (
            GenericSymbolReference reference,
            CancellationToken cancellationToken
        )
        {
            if ((reference.Node.Parent?.Parent is not InvocationExpressionSyntax invocationExpression) ||
                (reference.Symbol is not INamedTypeSymbol interfaceSymbol) ||
                (reference.SemanticModel.GetOperation(invocationExpression, cancellationToken) is not
                    IInvocationOperation methodInvocation))
            {
                return null;
            }
            return new InterfaceReference
            (
                interfaceSymbol,
                methodInvocation,
                !reference.IsSyntaxReferenceClosedTypeOrMethod
            );
        }

        public static InterfaceReference? GetReference(IInvocationOperation invocation)
        {
            if ((invocation is null) || invocation.TargetMethod.IsGenericMethod ||
                (invocation.TargetMethod.ContainingType is not INamedTypeSymbol interfaceSymbol) ||
                interfaceSymbol.IsGenericType)
            {
                return null;
            }
            return new InterfaceReference(interfaceSymbol, invocation, false);
        }

        private InterfaceReference
        (
            INamedTypeSymbol interfaceSymbol,
            IInvocationOperation methodInvocation,
            bool isInterfaceOrMethodOpenGeneric
        )
        {
            Interface = new InterfaceDescriptor(interfaceSymbol);
            InvocationArgumentCount = methodInvocation.Arguments
                .Where(static x => x.ArgumentKind == ArgumentKind.Explicit)
                .Count();
            IsInterfaceOrMethodOpenGeneric = isInterfaceOrMethodOpenGeneric;
            Method = new MethodDescriptor(Interface, methodInvocation.TargetMethod);
            MethodInvocation = methodInvocation;
            hashCode = Hash.Combine(Interface, MethodInvocation.Syntax);
        }

        public override bool Equals(object? obj) => obj is InterfaceReference other && Equals(other);
        public bool Equals(InterfaceReference? other) => (other is not null) && (Interface == other.Interface) &&
            MethodInvocation.Syntax.IsEquivalentTo(other.MethodInvocation.Syntax);
        public override int GetHashCode() => hashCode;
    }
}
