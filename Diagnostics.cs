using Microsoft.CodeAnalysis;

namespace Monkeymoto.NativeGenericDelegates
{
    internal static class Diagnostics
    {
        public static readonly DiagnosticDescriptor NGD1001_MarshalAsArgumentMustUseObjectCreationSyntaxDescriptor =
            new
            (
                "NGD1001",
                "NGD1001: Invalid MarshalAs argument",
                "MarshalAsAttribute{0} argument '{1}' must be null or use object creation syntax",
                "Usage",
                DiagnosticSeverity.Error,
                true
            );

        public static readonly DiagnosticDescriptor NGD1002_InvalidCallingConventionArgument = new
        (
            "NGD1002",
            "NGD1002: Invalid CallingConvention argument",
            "CallingConvention argument must be literal or static readonly field",
            "Usage",
            DiagnosticSeverity.Error,
            true
        );

        public static readonly DiagnosticDescriptor NGD1003_MarshalAsArgumentSpreadElementNotSupported = new
        (
            "NGD1003",
            "NGD1003: Spread element is not supported",
            "Spread element is not supported for MarshalAsAttribute[] argument `{0}`",
            "Usage",
            DiagnosticSeverity.Error,
            true
        );
    }
}
