using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis;
using System.Threading;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct ArgumentInfo : IEquatable<ArgumentInfo>
    {
        private readonly int hashCode;

        public readonly CallingConvention CallingConvention;
        public readonly MarshalInfo MarshalInfo;

        public static bool operator ==(ArgumentInfo left, ArgumentInfo right) => left.Equals(right);
        public static bool operator !=(ArgumentInfo left, ArgumentInfo right) => !(left == right);

        public ArgumentInfo
        (
            InvocationExpressionSyntax invocationExpressionSyntax,
            SemanticModel semanticModel,
            int marshallableParamsCount,
            List<Diagnostic> diagnostics,
            CancellationToken cancellationToken
        )
        {
            IArgumentOperation? callingConventionArgument = null;
            IArgumentOperation? marshalParamsAsArgument = null;
            IArgumentOperation? marshalReturnAsArgument = null;
            foreach (var argumentNode in invocationExpressionSyntax.ArgumentList.Arguments)
            {
                var argument = (IArgumentOperation)semanticModel.GetOperation(argumentNode, cancellationToken)!;
                switch (argument.Parameter!.Name)
                {
                    case "callingConvention":
                        callingConventionArgument = argument;
                        break;
                    case "marshalParamsAs":
                        marshalParamsAsArgument = argument;
                        break;
                    case "marshalReturnAs":
                        marshalReturnAsArgument = argument;
                        break;
                    default:
                        break;
                }
            }
            CallingConvention callingConvention = CallingConvention.Winapi;
            if (callingConventionArgument is not null)
            {
                var argument = (callingConventionArgument.Value as IFieldReferenceOperation)?.Field.ToDisplayString();
                if (argument is not null)
                {
                    _ = Enum.TryParse(argument.Substring(argument.LastIndexOf('.') + 1), false, out callingConvention);
                    // TODO: diagnostic - must use System.Runtime.InteropServices.CallingConvention literal value
                }
                // TODO: diagnostic - must use System.Runtime.InteropServices.CallingConvention literal value
            }
            CallingConvention = callingConvention == CallingConvention.Winapi ?
                Constants.DefaultCallingConvention :
                callingConvention;
            MarshalInfo = new
            (
                marshalReturnAsArgument,
                marshalParamsAsArgument,
                marshallableParamsCount,
                diagnostics,
                cancellationToken
            );
            hashCode = Hash.Combine(CallingConvention, MarshalInfo);
        }

        public override bool Equals(object obj)
        {
            return obj is ArgumentInfo other && Equals(other);
        }

        public bool Equals(ArgumentInfo other)
        {
            return CallingConvention == other.CallingConvention && MarshalInfo == other.MarshalInfo;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
