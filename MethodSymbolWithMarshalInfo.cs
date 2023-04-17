using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System;

namespace NativeGenericDelegatesGenerator
{
    internal readonly struct MethodSymbolWithMarshalInfo : IEquatable<MethodSymbolWithMarshalInfo>
    {
        private readonly int Hash;
        public readonly ImmutableArray<string?>? MarshalParamsAs;
        public readonly string? MarshalReturnAs;
        public readonly IMethodSymbol MethodSymbol;

        public MethodSymbolWithMarshalInfo(IMethodSymbol methodSymbol, string? marshalReturnAs, ImmutableArray<string?>? marshalParamsAs)
        {
            Hash = 1009;
            int factor = 9176;
            foreach (string? s in marshalParamsAs ?? ImmutableArray<string?>.Empty)
            {
                Hash = (Hash * factor) + (s ?? "").GetHashCode();
            }
            Hash = (Hash * factor) + (MarshalReturnAs ?? "").GetHashCode();
            Hash = (Hash * factor) + SymbolEqualityComparer.Default.GetHashCode(methodSymbol.IsGenericMethod ? methodSymbol : methodSymbol.ContainingType);
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
            return Hash == other.Hash;
        }

        public override int GetHashCode()
        {
            return Hash;
        }
    }
}
