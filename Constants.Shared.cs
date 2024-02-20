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

            private const string UnmanagedTypeConstraints_T0 = "";
            private const string UnmanagedTypeConstraints_T1 = $@"
                where U : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T2 = $@"
                where U1 : unmanaged
                where U2 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T3 = $@"{UnmanagedTypeConstraints_T1_T2}
                where U3 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T4 = $@"{UnmanagedTypeConstraints_T1_T3}
                where U4 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T5 = $@"{UnmanagedTypeConstraints_T1_T4}
                where U5 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T6 = $@"{UnmanagedTypeConstraints_T1_T5}
                where U6 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T7 = $@"{UnmanagedTypeConstraints_T1_T6}
                where U7 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T8 = $@"{UnmanagedTypeConstraints_T1_T7}
                where U8 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T9 = $@"{UnmanagedTypeConstraints_T1_T8}
                where U9 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T10 = $@"{UnmanagedTypeConstraints_T1_T9}
                where U10 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T11 = $@"{UnmanagedTypeConstraints_T1_T10}
                where U11 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T12 = $@"{UnmanagedTypeConstraints_T1_T11}
                where U12 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T13 = $@"{UnmanagedTypeConstraints_T1_T12}
                where U13 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T14 = $@"{UnmanagedTypeConstraints_T1_T13}
                where U14 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T15 = $@"{UnmanagedTypeConstraints_T1_T14}
                where U15 : unmanaged";
            private const string UnmanagedTypeConstraints_T1_T16 = $@"{UnmanagedTypeConstraints_T1_T15}
                where U16 : unmanaged";

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

            public static readonly string[] UnmanagedTypeConstraints =
            [
                UnmanagedTypeConstraints_T0,
                UnmanagedTypeConstraints_T1,
                UnmanagedTypeConstraints_T1_T2,
                UnmanagedTypeConstraints_T1_T3,
                UnmanagedTypeConstraints_T1_T4,
                UnmanagedTypeConstraints_T1_T5,
                UnmanagedTypeConstraints_T1_T6,
                UnmanagedTypeConstraints_T1_T7,
                UnmanagedTypeConstraints_T1_T8,
                UnmanagedTypeConstraints_T1_T9,
                UnmanagedTypeConstraints_T1_T10,
                UnmanagedTypeConstraints_T1_T11,
                UnmanagedTypeConstraints_T1_T12,
                UnmanagedTypeConstraints_T1_T13,
                UnmanagedTypeConstraints_T1_T14,
                UnmanagedTypeConstraints_T1_T15,
                UnmanagedTypeConstraints_T1_T16
            ];

            public static readonly string[] QualifiedTypeParameters = [.. TypeParameters.Select(x => x.Replace("T", "in T"))];
            public static readonly string[] UnmanagedTypeParameters = [.. TypeParameters.Select(x => x.Replace('T', 'U'))];
        }
    }
}
