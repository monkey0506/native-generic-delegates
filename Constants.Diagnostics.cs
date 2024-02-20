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
        }
    }
}
