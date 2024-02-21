using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly partial struct MarshalInfo : IEquatable<MarshalInfo>
    {
        private readonly int hash;

        public readonly string? MarshalReturnAs;
        public readonly IReadOnlyList<string?>? MarshalParamsAs;

        public static bool operator ==(MarshalInfo left, MarshalInfo right) => left.Equals(right);
        public static bool operator !=(MarshalInfo left, MarshalInfo right) => !(left == right);

        public MarshalInfo
        (
            IArgumentOperation? marshalReturnAsArgument,
            IArgumentOperation? marshalParamsAsArgument,
            int marshallableParamsCount,
            List<Diagnostic> diagnostics,
            CancellationToken cancellationToken
        )
        {
            MarshalParamsAs = Parser.GetMarshalParamsAs
            (
                marshalParamsAsArgument,
                marshallableParamsCount,
                diagnostics,
                cancellationToken
            );
            MarshalReturnAs = Parser.GetMarshalReturnAs(marshalReturnAsArgument, diagnostics, cancellationToken);
            hash = Hash.Combine(MarshalParamsAs, MarshalReturnAs);
        }

        public override bool Equals(object obj)
        {
            return obj is MarshalInfo other && Equals(other);
        }

        public bool Equals(MarshalInfo other)
        {
            if (MarshalReturnAs != other.MarshalReturnAs)
            {
                return false;
            }
            if (MarshalParamsAs is null)
            {
                return other.MarshalParamsAs is null;
            }
            if (other.MarshalParamsAs is null || (MarshalParamsAs.Count != other.MarshalParamsAs.Count))
            {
                return false;
            }
            return MarshalParamsAs.SequenceEqual(other.MarshalParamsAs);
        }

        public override int GetHashCode()
        {
            return hash;
        }
    }
}
