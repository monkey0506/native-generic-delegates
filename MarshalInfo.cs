using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly partial struct MarshalInfo : IEquatable<MarshalInfo>
    {
        private readonly int hash;

        public readonly string? MarshalReturnAs;
        public readonly string MarshalReturnAsSourceText;
        public readonly IReadOnlyList<string?>? MarshalParamsAs;
        public readonly string MarshalParamsAsSourceText;

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
            switch (MarshalParamsAs)
            {
                case null:
                case IReadOnlyList<string?> { Count: 0 }:
                    MarshalParamsAsSourceText = "null";
                    break;
                default:
                    var sb = new StringBuilder
                    (
$@"new MarshalAsAttribute?[]
        {{
"
                    );
                    foreach (var marshalParamAs in MarshalParamsAs)
                    {
                        _ = sb.AppendLine($"            {Parser.GetSourceText(marshalParamAs)}");
                    }
                    _ = sb.Append("        }");
                    MarshalParamsAsSourceText = sb.ToString();
                    break;
            }
            MarshalReturnAs = Parser.GetMarshalReturnAs(marshalReturnAsArgument, diagnostics, cancellationToken);
            MarshalReturnAsSourceText = Parser.GetSourceText(MarshalReturnAs);
            hash = Hash.Combine(MarshalParamsAsSourceText, MarshalReturnAsSourceText);
        }

        public override bool Equals(object obj)
        {
            return obj is MarshalInfo other && Equals(other);
        }

        public bool Equals(MarshalInfo other)
        {
            return MarshalParamsAsSourceText == other.MarshalParamsAsSourceText &&
                MarshalReturnAsSourceText == other.MarshalReturnAsSourceText;
        }

        public override int GetHashCode()
        {
            return hash;
        }
    }
}
