// NativeGenericDelegates
// https://github.com/monkey0506/native-generic-delegates
// Copyright(C) 2022-2023 Michael Rittenhouse monkey0506@gmail.com

// This C# project and all associated project files are hereby committed to the
// public domain pursuant to the WTFPL http://www.wtfpl.net/about/ without
// warranty of any kind, express or implied, including but not limited to
// fitness for a particular purpose. Attribution is appreciated, but not
// required.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NativeGenericDelegatesGenerator
{
    /// <summary>
    /// Represents an <see cref="IMethodSymbol"/> (see <see cref="MethodSymbolWithContext"/>) with custom marshaling behavior.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type represents a method symbol and custom marshaling behaviors that may or may not be unique, but is suitable for
    /// equality comparisons and hashing operations (e.g., use in a <see cref="HashSet{T}"/>). Equality and hashing combine an
    /// <see cref="ISymbol"/> (either <see cref="MethodSymbol"/> or the <see cref="INamedTypeSymbol"/> representing the
    /// containing interface) with the custom marshaling behavior. The <see cref="ISymbol"/> is always compared using <see
    /// cref="SymbolEqualityComparer.Default"/>. If <see cref="MethodSymbol"/> represents a generic overload of
    /// <c>FromFunctionPointer</c> (e.g., <c>INativeAction&lt;string&gt;.FromFunctionPointer&lt;nint&gt;</c>), then equality and
    /// hashing is performed using <see cref="MethodSymbol"/>; otherwise, the <see cref="INamedTypeSymbol"/> is used.
    /// </para>
    /// </remarks>
    internal readonly struct MethodSymbolWithMarshalInfo : IEquatable<MethodSymbolWithMarshalInfo>
    {
        private readonly int Hash;
        public readonly ImmutableArray<string?>? MarshalParamsAs;
        public readonly string? MarshalReturnAs;
        public readonly IMethodSymbol MethodSymbol;

        public MethodSymbolWithMarshalInfo
        (
            IMethodSymbol methodSymbol,
            string? marshalReturnAs,
            ImmutableArray<string?>? marshalParamsAs
        )
        {
            Hash = 1009;
            int factor = 9176;
            foreach (string? s in marshalParamsAs ?? ImmutableArray<string?>.Empty)
            {
                Hash = (Hash * factor) + (s ?? "").GetHashCode();
            }
            Hash = (Hash * factor) + (MarshalReturnAs ?? "").GetHashCode();
            Hash = (Hash * factor) + SymbolEqualityComparer.Default.GetHashCode
            (
                methodSymbol.IsGenericMethod ? methodSymbol : methodSymbol.ContainingType
            );
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
