using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal static partial class Constants
    {
        private static class Shared
        {
            public const string Arguments_T0 = "";
            public const string Arguments_T1 = "t";
            public const string Arguments_T1_T2 = $"{Arguments_T1}1, t2";
            public const string Arguments_T1_T3 = $"{Arguments_T1_T2}, t3";
            public const string Arguments_T1_T4 = $"{Arguments_T1_T3}, t4";
            public const string Arguments_T1_T5 = $"{Arguments_T1_T4}, t5";
            public const string Arguments_T1_T6 = $"{Arguments_T1_T5}, t6";
            public const string Arguments_T1_T7 = $"{Arguments_T1_T6}, t7";
            public const string Arguments_T1_T8 = $"{Arguments_T1_T7}, t8";
            public const string Arguments_T1_T9 = $"{Arguments_T1_T8}, t9";
            public const string Arguments_T1_T10 = $"{Arguments_T1_T9}, t10";
            public const string Arguments_T1_T11 = $"{Arguments_T1_T10}, t11";
            public const string Arguments_T1_T12 = $"{Arguments_T1_T11}, t12";
            public const string Arguments_T1_T13 = $"{Arguments_T1_T12}, t13";
            public const string Arguments_T1_T14 = $"{Arguments_T1_T13}, t14";
            public const string Arguments_T1_T15 = $"{Arguments_T1_T14}, t15";
            public const string Arguments_T1_T16 = $"{Arguments_T1_T15}, t16";

            public const string Parameters_T0 = "";
            public const string Parameters_T1 = "T t";
            public const string Parameters_T1_T2 = "T1 t1, T2 t2";
            public const string Parameters_T1_T3 = $"{Parameters_T1_T2}, T3 t3";
            public const string Parameters_T1_T4 = $"{Parameters_T1_T3}, T4 t4";
            public const string Parameters_T1_T5 = $"{Parameters_T1_T4}, T5 t5";
            public const string Parameters_T1_T6 = $"{Parameters_T1_T5}, T6 t6";
            public const string Parameters_T1_T7 = $"{Parameters_T1_T6}, T7 t7";
            public const string Parameters_T1_T8 = $"{Parameters_T1_T7}, T8 t8";
            public const string Parameters_T1_T9 = $"{Parameters_T1_T8}, T9 t9";
            public const string Parameters_T1_T10 = $"{Parameters_T1_T9}, T10 t10";
            public const string Parameters_T1_T11 = $"{Parameters_T1_T10}, T11 t11";
            public const string Parameters_T1_T12 = $"{Parameters_T1_T11}, T12 t12";
            public const string Parameters_T1_T13 = $"{Parameters_T1_T12}, T13 t13";
            public const string Parameters_T1_T14 = $"{Parameters_T1_T13}, T14 t14";
            public const string Parameters_T1_T15 = $"{Parameters_T1_T14}, T15 t15";
            public const string Parameters_T1_T16 = $"{Parameters_T1_T15}, T16 t16";

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
