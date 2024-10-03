using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class DelegateMarshalling : IEquatable<DelegateMarshalling>
    {
        private readonly int hashCode;

        public CallingConvention CallingConvention { get; }
        public IReadOnlyList<string?>? MarshalParamsAs { get; }
        public string? MarshalReturnAs { get; }

        public static bool operator ==(DelegateMarshalling? left, DelegateMarshalling? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(DelegateMarshalling? left, DelegateMarshalling? right) => !(left == right);

        public DelegateMarshalling
        (
            InvocationExpressionSyntax invocationExpression,
            SemanticModel semanticModel,
            InterfaceDescriptor interfaceDescriptor,
            IList<Diagnostic> diagnostics,
            CancellationToken cancellationToken
        )
        {
            IArgumentOperation? callingConventionArgument = null;
            IArgumentOperation? marshalReturnAsArgument = null;
            IArgumentOperation? marshalParamsAsArgument = null;
            foreach (var argumentNode in invocationExpression.ArgumentList.Arguments)
            {
                var argumentOp = (IArgumentOperation)semanticModel.GetOperation(argumentNode, cancellationToken)!;
                switch (argumentOp.Parameter!.Name)
                {
                    case "callingConvention":
                        callingConventionArgument = argumentOp;
                        break;
                    case "marshalReturnAs":
                        marshalReturnAsArgument = argumentOp;
                        break;
                    case "marshalParamsAs":
                        marshalParamsAsArgument = argumentOp;
                        break;
                    default:
                        break;
                }
            }
            CallingConvention callingConvention = CallingConvention.Winapi;
            if (callingConventionArgument is not null)
            {
                bool isValid = false;
                var field = (callingConventionArgument.Value as IFieldReferenceOperation)?.Field;
                if (field is not null && SymbolEqualityComparer.Default.Equals(field.ContainingType, field.Type) &&
                    Enum.TryParse(field?.Name, false, out callingConvention))
                {
                    isValid = true;
                }
                if (!isValid)
                {
                    callingConvention = CallingConvention.Winapi;
                    diagnostics.Add
                    (
                        Diagnostic.Create
                        (
                            Diagnostics.NGD1002_InvalidCallingConventionArgument,
                            callingConventionArgument.Syntax.GetLocation()
                        )
                    );
                }
            }
            CallingConvention = callingConvention;
            MarshalParamsAs = Parser.GetMarshalParamsAs
            (
                marshalParamsAsArgument,
                interfaceDescriptor.InvokeParameterCount,
                diagnostics,
                cancellationToken
            );
            MarshalReturnAs = Parser.GetMarshalReturnAs(marshalReturnAsArgument, diagnostics, cancellationToken);
            hashCode = Hash.Combine(CallingConvention, MarshalParamsAs, MarshalReturnAs);
        }

        public override bool Equals(object? obj) => obj is DelegateMarshalling other && Equals(other);
        public bool Equals(DelegateMarshalling? other) =>
            (other is not null) && (CallingConvention == other.CallingConvention) &&
            MarshalParamsAs.SequenceEqual(other.MarshalParamsAs) && (MarshalReturnAs == other.MarshalReturnAs);
        public override int GetHashCode() => hashCode;
    }
}
