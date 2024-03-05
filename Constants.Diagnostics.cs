using Microsoft.CodeAnalysis;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal static partial class Constants
    {
        internal static class Diagnostics
        {
            private const string NGD1001_ID = "NGD1001";
            private const string NGD1001_Title = "Invalid argument";
            private const string NGD1001_MessageFormat =
                "MarshalAsAttribute{0} argument '{1}' must be null or use object creation syntax";
            private const string NGD1001_Category = "Usage";
            private const DiagnosticSeverity NGD1001_DefaultSeverity = DiagnosticSeverity.Error;
            private const bool NGD1001_IsEnabledByDefault = true;

            /// <summary>
            /// MarshalAsAttribute argument must be null or use object creation syntax.
            /// </summary>
            public static readonly DiagnosticDescriptor NGD1001 = new
            (
                NGD1001_ID,
                NGD1001_Title,
                NGD1001_MessageFormat,
                NGD1001_Category,
                NGD1001_DefaultSeverity,
                NGD1001_IsEnabledByDefault
            );

            private const string NGD1002_ID = "NGD1002";
            private const string NGD1002_Title = "Invalid CallingConvention argument";
            private const string NGD1002_MessageFormat =
                "CallingConvention argument '{0}' must be a System.Runtime.InteropServices.CallingConvention literal value";
            private const string NGD1002_Category = "Usage";
            private const DiagnosticSeverity NGD1002_DefaultSeverity = DiagnosticSeverity.Error;
            private const bool NGD1002_IsEnabledByDefault = true;

            /// <summary>
            /// CallingConvention argument must be a <see cref="System.Runtime.InteropServices.CallingConvention"/> literal
            /// value.
            /// </summary>
            public static readonly DiagnosticDescriptor NGD1002 = new
            (
                NGD1002_ID,
                NGD1002_Title,
                NGD1002_MessageFormat,
                NGD1002_Category,
                NGD1002_DefaultSeverity,
                NGD1002_IsEnabledByDefault
            );
        }
    }
}
