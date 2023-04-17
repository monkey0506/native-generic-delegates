using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System;

namespace NativeGenericDelegatesGenerator
{
    internal readonly struct MethodSymbolWithMarshalInfo : IEquatable<MethodSymbolWithMarshalInfo>
    {
        public readonly ImmutableArray<string?>? MarshalParamsAs;
        public readonly string? MarshalReturnAs;
        public readonly IMethodSymbol MethodSymbol;

        public MethodSymbolWithMarshalInfo(IMethodSymbol methodSymbol, string? marshalReturnAs, ImmutableArray<string?>? marshalParamsAs)
        {
            MarshalParamsAs = marshalParamsAs;
            MarshalReturnAs = marshalReturnAs;
            MethodSymbol = methodSymbol;
        }

        public override bool Equals(object obj)
        {
            return obj is MethodSymbolWithMarshalInfo other && Equals(other);
        }

        public bool Equals(MethodSymbolWithMarshalInfo other)
        {
            return GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            int hash = 1009;
            int factor = 9176;
            foreach (string? s in MarshalParamsAs ?? ImmutableArray<string?>.Empty)
            {
                hash = (hash * factor) + (s ?? "").GetHashCode();
            }
            hash = (hash * factor) + (MarshalReturnAs ?? "").GetHashCode();
            hash = (hash * factor) + SymbolEqualityComparer.Default.GetHashCode(MethodSymbol.IsGenericMethod ? MethodSymbol : MethodSymbol.ContainingType);
            return hash;
        }
    }
}
