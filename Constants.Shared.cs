using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal static partial class Constants
    {
        private static class Shared
        {
            private const string TypeParameters_T0 = "";
            private const string TypeParameters_T1 = "T";
            private const string TypeParameters_T1_T2 = $"{TypeParameters_T1}1, T2";
            private const string TypeParameters_T1_T3 = $"{TypeParameters_T1_T2}, T3";
            private const string TypeParameters_T1_T4 = $"{TypeParameters_T1_T3}, T4";
            private const string TypeParameters_T1_T5 = $"{TypeParameters_T1_T4}, T5";
            private const string TypeParameters_T1_T6 = $"{TypeParameters_T1_T5}, T6";
            private const string TypeParameters_T1_T7 = $"{TypeParameters_T1_T6}, T7";
            private const string TypeParameters_T1_T8 = $"{TypeParameters_T1_T7}, T8";
            private const string TypeParameters_T1_T9 = $"{TypeParameters_T1_T8}, T9";
            private const string TypeParameters_T1_T10 = $"{TypeParameters_T1_T9}, T10";
            private const string TypeParameters_T1_T11 = $"{TypeParameters_T1_T10}, T11";
            private const string TypeParameters_T1_T12 = $"{TypeParameters_T1_T11}, T12";
            private const string TypeParameters_T1_T13 = $"{TypeParameters_T1_T12}, T13";
            private const string TypeParameters_T1_T14 = $"{TypeParameters_T1_T13}, T14";
            private const string TypeParameters_T1_T15 = $"{TypeParameters_T1_T14}, T15";
            private const string TypeParameters_T1_T16 = $"{TypeParameters_T1_T15}, T16";

            public static readonly string[] TypeParameters =
            [
                TypeParameters_T0,
                TypeParameters_T1,
                TypeParameters_T1_T2,
                TypeParameters_T1_T3,
                TypeParameters_T1_T4,
                TypeParameters_T1_T5,
                TypeParameters_T1_T6,
                TypeParameters_T1_T7,
                TypeParameters_T1_T8,
                TypeParameters_T1_T9,
                TypeParameters_T1_T10,
                TypeParameters_T1_T11,
                TypeParameters_T1_T12,
                TypeParameters_T1_T13,
                TypeParameters_T1_T14,
                TypeParameters_T1_T15,
                TypeParameters_T1_T16
            ];

            public static readonly string[] QualifiedTypeParameters = [.. TypeParameters.Select(x => x.Replace("T", "in T"))];
        }
    }
}
