using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Monkeymoto.GeneratorUtils;
using System;
using System.Linq;
using System.Threading;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed class MethodReference : IEquatable<MethodReference>
    {
        private readonly int hashCode;

        public int Character { get; }
        public string FilePath { get; }
        public string InterceptorAttributeSourceText { get; }
        public InterfaceDescriptor Interface { get; }
        public int InvocationArgumentCount { get; }
        public bool IsInterfaceOrMethodOpenGeneric { get; }
        public int Line { get; }
        public MarshalInfo MarshalInfo { get; }
        public MethodDescriptor Method { get; }

        public static bool operator ==(MethodReference? left, MethodReference? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(MethodReference? left, MethodReference? right) => !(left == right);

        public static MethodReference? GetReference
        (
            GenericSymbolReference interfaceOrMethodReference,
            CancellationToken cancellationToken
        )
        {
            INamedTypeSymbol? interfaceSymbol;
            IMethodSymbol? methodSymbol;
            InvocationExpressionSyntax? invocationExpression;
            var node = interfaceOrMethodReference.Node;
            if (node is InvocationExpressionSyntax genericMethodInvocationExpression)
            {
                methodSymbol = (IMethodSymbol)interfaceOrMethodReference.Symbol;
                interfaceSymbol = methodSymbol.ContainingType;
                invocationExpression = genericMethodInvocationExpression;
            }
            else if (node.Parent?.Parent is InvocationExpressionSyntax methodInvocationExpression)
            {
                var methodNameSyntax = ((MemberAccessExpressionSyntax)node.Parent).Name;
                interfaceSymbol = (INamedTypeSymbol)interfaceOrMethodReference.Symbol;
                methodSymbol = interfaceSymbol.GetMembers(methodNameSyntax.Identifier.ToString()).Cast<IMethodSymbol>()
                    .First(x => x.Arity == methodNameSyntax.Arity);
                invocationExpression = methodInvocationExpression;
            }
            else
            {
                return null;
            }
            var semanticModel = interfaceOrMethodReference.SemanticModel!;
            int invocationArgumentCount = 0;
            var invocation =
                semanticModel.GetOperation(invocationExpression, cancellationToken) as IInvocationOperation;
            if (invocation is not null)
            {
                invocationArgumentCount = invocation.Arguments.Length -
                    invocation.Arguments.Where(static x => x.ArgumentKind != ArgumentKind.Explicit).Count();
            }
            var interfaceDescriptor = new InterfaceDescriptor(interfaceSymbol);
            var methodDescriptor = new MethodDescriptor
            (
                interfaceDescriptor,
                methodSymbol!
            );
            var marshalInfo = MarshalInfo.GetMarshalInfo
            (
                methodSymbol!,
                interfaceDescriptor,
                invocationExpression,
                interfaceOrMethodReference.SemanticModel!,
                cancellationToken
            );
            return new MethodReference
            (
                interfaceDescriptor,
                methodDescriptor,
                invocationExpression,
                marshalInfo,
                !interfaceOrMethodReference.IsSyntaxReferenceClosedTypeOrMethod,
                invocationArgumentCount
            );
        }

        private MethodReference
        (
            InterfaceDescriptor interfaceDescriptor,
            MethodDescriptor methodDescriptor,
            InvocationExpressionSyntax invocationExpression,
            MarshalInfo marshalInfo,
            bool isInterfaceOrMethodOpenGeneric,
            int invocationArgumentCount
        )
        {
            var methodNode = ((MemberAccessExpressionSyntax)invocationExpression.Expression).Name;
            var linePosition = methodNode.GetLocation().GetLineSpan().Span.Start;
            Character = linePosition.Character + 1;
            FilePath = invocationExpression.SyntaxTree.FilePath;
            Interface = interfaceDescriptor;
            IsInterfaceOrMethodOpenGeneric = isInterfaceOrMethodOpenGeneric;
            Line = linePosition.Line + 1;
            MarshalInfo = marshalInfo;
            Method = methodDescriptor;
            InterceptorAttributeSourceText = $"[InterceptsLocation(@\"{FilePath}\", {Line}, {Character})]";
            InvocationArgumentCount = invocationArgumentCount;
            hashCode = Hash.Combine(Character, FilePath, Line);
        }

        public override bool Equals(object? obj) => obj is MethodReference other && Equals(other);
        public bool Equals(MethodReference? other) => (other is not null) && (Character == other.Character) &&
             (FilePath == other.FilePath) && (Line == other.Line);
        public override int GetHashCode() => hashCode;
    }
}
