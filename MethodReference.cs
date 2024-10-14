using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Monkeymoto.GeneratorUtils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed class MethodReference : IEquatable<MethodReference>
    {
        private readonly int hashCode;

        public InterfaceDescriptor Interface { get; }
        public int InvocationArgumentCount { get; }
        public bool IsInterfaceOrMethodOpenGeneric { get; }
        public InterceptedLocation Location { get; }
        public MarshalInfo MarshalInfo { get; }
        public MethodDescriptor Method { get; }

        public static bool operator ==(MethodReference? left, MethodReference? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(MethodReference? left, MethodReference? right) => !(left == right);

        public static IReadOnlyList<MethodReference>? GetReferences
        (
            GenericSymbolReference interfaceReference,
            Func<IMethodSymbol, InvocationExpressionSyntax, IReadOnlyCollection<GenericSymbolReference>>
                getGenericMethodReferences,
            CancellationToken cancellationToken
        )
        {
            var node = interfaceReference.Node;
            var invocationExpression = node.Parent?.Parent as InvocationExpressionSyntax;
            if (invocationExpression is null)
            {
                return null;
            }
            var semanticModel = interfaceReference.SemanticModel!;
            var operation = semanticModel.GetOperation(invocationExpression, cancellationToken);
            if (operation is not IInvocationOperation invocation)
            {
                return null;
            }
            var methodSymbol = invocation.TargetMethod;
            var marshallers = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            var isOpenGenericMethod = false;
            if (methodSymbol.IsGenericMethod)
            {
                if (methodSymbol.TypeArguments.First() is INamedTypeSymbol namedMarshaller)
                {
                    marshallers.Add(namedMarshaller);
                }
                else
                {
                    isOpenGenericMethod = true;
                    foreach
                    (
                        var marshaller in getGenericMethodReferences(methodSymbol, invocationExpression)
                            .Select(static x => (INamedTypeSymbol)x.TypeArguments.First())
                    )
                    {
                        marshallers.Add(marshaller);
                    }
                }
            }
            var invocationArgumentCount = invocation.Arguments.Length -
                invocation.Arguments.Where(static x => x.ArgumentKind != ArgumentKind.Explicit).Count();
            var interfaceSymbol = (INamedTypeSymbol)interfaceReference.Symbol;
            var interfaceDescriptor = new InterfaceDescriptor(interfaceSymbol);
            var methodDescriptor = new MethodDescriptor(interfaceDescriptor, methodSymbol!);
            var methodReferences = ImmutableList.CreateBuilder<MethodReference>();

            MethodReference GetReference(INamedTypeSymbol? marshaller)
            {
                var marshalInfo = MarshalInfo.GetMarshalInfo
                (
                    marshaller,
                    interfaceDescriptor,
                    invocationExpression,
                    semanticModel,
                    cancellationToken
                );
                return new MethodReference
                (
                    interfaceDescriptor,
                    methodDescriptor,
                    invocationExpression,
                    marshalInfo,
                    !interfaceReference.IsSyntaxReferenceClosedTypeOrMethod || isOpenGenericMethod,
                    invocationArgumentCount
                );
            }

            if (marshallers.Count == 0)
            {
                methodReferences.Add(GetReference(null));
            }
            else
            {
                foreach (var marshaller in marshallers)
                {
                    methodReferences.Add(GetReference(marshaller));
                }
            }
            return methodReferences.ToImmutable();
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
            Interface = interfaceDescriptor;
            InvocationArgumentCount = invocationArgumentCount;
            IsInterfaceOrMethodOpenGeneric = isInterfaceOrMethodOpenGeneric;
            Location = new InterceptedLocation(invocationExpression);
            MarshalInfo = marshalInfo;
            Method = methodDescriptor;
            hashCode = Hash.Combine(Location, Method, InvocationArgumentCount, MarshalInfo);
        }

        public override bool Equals(object? obj) => obj is MethodReference other && Equals(other);
        public bool Equals(MethodReference? other) => (other is not null) && (Location == other.Location) &&
            (Method == other.Method) && (InvocationArgumentCount == other.InvocationArgumentCount) &&
            (MarshalInfo == other.MarshalInfo);
        public override int GetHashCode() => hashCode;
    }
}
