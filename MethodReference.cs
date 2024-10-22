using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Monkeymoto.GeneratorUtils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
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

        private static MethodReference GetReference
        (
            InterfaceReference interfaceReference,
            CancellationToken cancellationToken,
            INamedTypeSymbol? marshaller = null
        )
        {
            var marshalInfo = MarshalInfo.GetMarshalInfo(interfaceReference, marshaller, cancellationToken);
            return new MethodReference(interfaceReference, marshalInfo);
        }

        public static IReadOnlyList<MethodReference>? GetReferences
        (
            InterfaceReference interfaceReference,
            Func<InterfaceReference, IReadOnlyCollection<GenericSymbolReference>> getGenericMethodReferences,
            CancellationToken cancellationToken
        )
        {
            var invocation = interfaceReference.MethodInvocation;
            var invocationExpression = (InvocationExpressionSyntax)invocation.Syntax;
            var methodSymbol = invocation.TargetMethod;
            var marshallers = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            if (methodSymbol.IsGenericMethod)
            {
                if (methodSymbol.TypeArguments[0] is INamedTypeSymbol namedMarshaller)
                {
                    marshallers.Add(namedMarshaller);
                }
                else
                {
                    foreach
                    (
                        var marshaller in getGenericMethodReferences(interfaceReference)
                            .Select(static x => (INamedTypeSymbol)x.TypeArguments[0])
                    )
                    {
                        marshallers.Add(marshaller);
                    }
                }
            }
            var interfaceDescriptor = interfaceReference.Interface;
            var methodDescriptor = interfaceReference.Method;
            var methodReferences = ImmutableList.CreateBuilder<MethodReference>();
            if (marshallers.Count == 0)
            {
                methodReferences.Add(GetReference(interfaceReference, cancellationToken));
            }
            else
            {
                foreach (var marshaller in marshallers)
                {
                    methodReferences.Add(GetReference(interfaceReference, cancellationToken, marshaller));
                }
            }
            return methodReferences.ToImmutable();
        }

        private MethodReference
        (
            InterfaceReference interfaceReference,
            MarshalInfo marshalInfo
        )
        {
            var invocationExpression = (InvocationExpressionSyntax)interfaceReference.MethodInvocation.Syntax;
            Interface = interfaceReference.Interface;
            InvocationArgumentCount = interfaceReference.InvocationArgumentCount;
            IsInterfaceOrMethodOpenGeneric = interfaceReference.IsInterfaceOrMethodOpenGeneric;
            Location = new InterceptedLocation(invocationExpression);
            MarshalInfo = marshalInfo;
            Method = interfaceReference.Method;
            hashCode = Hash.Combine(Location, Method, InvocationArgumentCount, MarshalInfo);
        }

        public override bool Equals(object? obj) => obj is MethodReference other && Equals(other);
        public bool Equals(MethodReference? other) => (other is not null) && (Location == other.Location) &&
            (Method == other.Method) && (InvocationArgumentCount == other.InvocationArgumentCount) &&
            (MarshalInfo == other.MarshalInfo);
        public override int GetHashCode() => hashCode;
    }
}
