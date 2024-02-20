using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct MethodImplementation : IEquatable<MethodImplementation>
    {
        private readonly int hash;

        public readonly ArgumentInfo ArgumentInfo;
        public readonly ClassDefinition ClassDefinition;
        public readonly int InvokeParameterCount;
        public readonly bool IsAction;
        public readonly bool IsFromFunctionPointer;
        public readonly IMethodSymbol Method;
        public readonly IReadOnlyList<MethodReference> References;
        public readonly string SourceText;

        public static bool operator ==(MethodImplementation left, MethodImplementation right) => left.Equals(right);
        public static bool operator !=(MethodImplementation left, MethodImplementation right) => !(left == right);

        public MethodImplementation
        (
            ArgumentInfo argumentInfo,
            int invokeParameterCount,
            bool isAction,
            bool isFromFunctionPointer,
            IMethodSymbol method,
            IReadOnlyList<MethodReference> references
        )
        {
            ArgumentInfo = argumentInfo;
            ClassDefinition = new(method, argumentInfo, invokeParameterCount, isAction, isFromFunctionPointer, references);
            InvokeParameterCount = invokeParameterCount;
            IsAction = isAction;
            IsFromFunctionPointer = isFromFunctionPointer;
            Method = method;
            References = references;
            SourceText = ClassDefinition.SourceText;
            hash = Hash.Combine(ArgumentInfo, Method, References);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodImplementation other && Equals(other);
        }

        public bool Equals(MethodImplementation other)
        {
            return ArgumentInfo == other.ArgumentInfo && SymbolEqualityComparer.Default.Equals(Method, other.Method) &&
                References.SequenceEqual(other.References);
        }

        public override int GetHashCode()
        {
            return hash;
        }
    }
}
