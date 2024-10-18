using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace Monkeymoto.NativeGenericDelegates
{
    internal static partial class Constants
    {
        private static readonly string[] AntiConstraints =
        [
            AntiConstraint_T0,
            AntiConstraint_T1,
            AntiConstraint_T1_T2,
            AntiConstraint_T1_T3,
            AntiConstraint_T1_T4,
            AntiConstraint_T1_T5,
            AntiConstraint_T1_T6,
            AntiConstraint_T1_T7,
            AntiConstraint_T1_T8,
            AntiConstraint_T1_T9,
            AntiConstraint_T1_T10,
            AntiConstraint_T1_T11,
            AntiConstraint_T1_T12,
            AntiConstraint_T1_T13,
            AntiConstraint_T1_T14,
            AntiConstraint_T1_T15,
            AntiConstraint_T1_T16
        ];

        private const string AntiConstraint_T0 = "";
        private const string AntiConstraint_T1 = $"{NewLineIndent2}where T : allows ref struct";
        private const string AntiConstraint_T1_T2 =
            $"{NewLineIndent2}where T1 : allows ref struct{NewLineIndent2}where T2 : allows ref struct";
        private const string AntiConstraint_T1_T3 =
            $"{AntiConstraint_T1_T2}{NewLineIndent2}where T3 : allows ref struct";
        private const string AntiConstraint_T1_T4 =
            $"{AntiConstraint_T1_T3}{NewLineIndent2}where T4 : allows ref struct";
        private const string AntiConstraint_T1_T5 =
            $"{AntiConstraint_T1_T4}{NewLineIndent2}where T5 : allows ref struct";
        private const string AntiConstraint_T1_T6 =
            $"{AntiConstraint_T1_T5}{NewLineIndent2}where T6 : allows ref struct";
        private const string AntiConstraint_T1_T7 =
            $"{AntiConstraint_T1_T6}{NewLineIndent2}where T7 : allows ref struct";
        private const string AntiConstraint_T1_T8 =
            $"{AntiConstraint_T1_T7}{NewLineIndent2}where T8 : allows ref struct";
        private const string AntiConstraint_T1_T9 =
            $"{AntiConstraint_T1_T8}{NewLineIndent2}where T9 : allows ref struct";
        private const string AntiConstraint_T1_T10 =
            $"{AntiConstraint_T1_T9}{NewLineIndent2}where T10 : allows ref struct";
        private const string AntiConstraint_T1_T11 =
            $"{AntiConstraint_T1_T10}{NewLineIndent2}where T11 : allows ref struct";
        private const string AntiConstraint_T1_T12 =
            $"{AntiConstraint_T1_T11}{NewLineIndent2}where T12 : allows ref struct";
        private const string AntiConstraint_T1_T13 =
            $"{AntiConstraint_T1_T12}{NewLineIndent2}where T13 : allows ref struct";
        private const string AntiConstraint_T1_T14 =
            $"{AntiConstraint_T1_T13}{NewLineIndent2}where T14 : allows ref struct";
        private const string AntiConstraint_T1_T15 =
            $"{AntiConstraint_T1_T14}{NewLineIndent2}where T15 : allows ref struct";
        private const string AntiConstraint_T1_T16 =
            $"{AntiConstraint_T1_T15}{NewLineIndent2}where T16 : allows ref struct";

        public static readonly string[] Arguments =
        [
            Arguments_T0,
            Arguments_T1,
            Arguments_T1_T2,
            Arguments_T1_T3,
            Arguments_T1_T4,
            Arguments_T1_T5,
            Arguments_T1_T6,
            Arguments_T1_T7,
            Arguments_T1_T8,
            Arguments_T1_T9,
            Arguments_T1_T10,
            Arguments_T1_T11,
            Arguments_T1_T12,
            Arguments_T1_T13,
            Arguments_T1_T14,
            Arguments_T1_T15,
            Arguments_T1_T16,
        ];

        private const string Arguments_T0 = "";
        private const string Arguments_T1 = "t";
        private const string Arguments_T1_T2 = $"{Arguments_T1}1, t2";
        private const string Arguments_T1_T3 = $"{Arguments_T1_T2}, t3";
        private const string Arguments_T1_T4 = $"{Arguments_T1_T3}, t4";
        private const string Arguments_T1_T5 = $"{Arguments_T1_T4}, t5";
        private const string Arguments_T1_T6 = $"{Arguments_T1_T5}, t6";
        private const string Arguments_T1_T7 = $"{Arguments_T1_T6}, t7";
        private const string Arguments_T1_T8 = $"{Arguments_T1_T7}, t8";
        private const string Arguments_T1_T9 = $"{Arguments_T1_T8}, t9";
        private const string Arguments_T1_T10 = $"{Arguments_T1_T9}, t10";
        private const string Arguments_T1_T11 = $"{Arguments_T1_T10}, t11";
        private const string Arguments_T1_T12 = $"{Arguments_T1_T11}, t12";
        private const string Arguments_T1_T13 = $"{Arguments_T1_T12}, t13";
        private const string Arguments_T1_T14 = $"{Arguments_T1_T13}, t14";
        private const string Arguments_T1_T15 = $"{Arguments_T1_T14}, t15";
        private const string Arguments_T1_T16 = $"{Arguments_T1_T15}, t16";

        public const string CategoryAction = "Action";
        public const string CategoryFunc = "Func";
        public const string FromFunctionPointerIdentifier = "FromFunctionPointer";
        public const string RootNamespace = "Monkeymoto.NativeGenericDelegates";
        public const string DeclarationsSourceFileName = RootNamespace + ".Declarations.g.cs";

        public const string IMarshallerInterfaceName = "IMarshaller";
        public const string IMarshallerMetadataName = $"{RootNamespace}.{IMarshallerInterfaceName}`1";

        /// <summary>
        /// Returns a newline for a source text string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <a href="https://github.com/dotnet/roslyn/issues/51437">How should source generators emit newlines?
        /// #51437</a>. In short, both '\n' and <see cref="Environment.NewLine"/> perform inconsistently when
        /// generating source files.
        /// </para>
        /// </remarks>
        public const string NewLine =
@"
";

        public const string NewLineIndent1 = $"{NewLine}    ";
        public const string NewLineIndent2 = $"{NewLineIndent1}    ";
        public const string NewLineIndent3 = $"{NewLineIndent2}    ";
        public const string NewLineIndent4 = $"{NewLineIndent3}    ";
        public const string SourceFileName = RootNamespace + ".g.cs";

        public static readonly string[] Parameters =
        [
            Parameters_T0,
            Parameters_T1,
            Parameters_T1_T2,
            Parameters_T1_T3,
            Parameters_T1_T4,
            Parameters_T1_T5,
            Parameters_T1_T6,
            Parameters_T1_T7,
            Parameters_T1_T8,
            Parameters_T1_T9,
            Parameters_T1_T10,
            Parameters_T1_T11,
            Parameters_T1_T12,
            Parameters_T1_T13,
            Parameters_T1_T14,
            Parameters_T1_T15,
            Parameters_T1_T16
        ];

        private const string Parameters_T0 = "";
        private const string Parameters_T1 = "T t";
        private const string Parameters_T1_T2 = "T1 t1, T2 t2";
        private const string Parameters_T1_T3 = $"{Parameters_T1_T2}, T3 t3";
        private const string Parameters_T1_T4 = $"{Parameters_T1_T3}, T4 t4";
        private const string Parameters_T1_T5 = $"{Parameters_T1_T4}, T5 t5";
        private const string Parameters_T1_T6 = $"{Parameters_T1_T5}, T6 t6";
        private const string Parameters_T1_T7 = $"{Parameters_T1_T6}, T7 t7";
        private const string Parameters_T1_T8 = $"{Parameters_T1_T7}, T8 t8";
        private const string Parameters_T1_T9 = $"{Parameters_T1_T8}, T9 t9";
        private const string Parameters_T1_T10 = $"{Parameters_T1_T9}, T10 t10";
        private const string Parameters_T1_T11 = $"{Parameters_T1_T10}, T11 t11";
        private const string Parameters_T1_T12 = $"{Parameters_T1_T11}, T12 t12";
        private const string Parameters_T1_T13 = $"{Parameters_T1_T12}, T13 t13";
        private const string Parameters_T1_T14 = $"{Parameters_T1_T13}, T14 t14";
        private const string Parameters_T1_T15 = $"{Parameters_T1_T14}, T15 t15";
        private const string Parameters_T1_T16 = $"{Parameters_T1_T15}, T16 t16";

        private static readonly string[] TypeParameters =
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

        public static readonly string[] InterceptorTypeConstraints =
        [
            .. AntiConstraints.Select(static x => x.Replace("    where T", "        where X").Replace('T', 'X')),
            $"{AntiConstraint_T1_T16.Replace("    where T", "        where X").Replace('T', 'X')}{NewLineIndent3}" +
                "where X17 : allows ref struct"
        ];

        public static readonly string[] InterceptorUnmanagedTypeConstraints =
        [
            .. InterceptorTypeConstraints
                .Select
                (
                    static x =>
                        $"{x.Replace("X", "XT")}{x.Replace("X", "XU").Replace(": allows", ": unmanaged, allows")}"
                )
        ];

        public static readonly string[] InterceptorTypeParameters =
        [
            .. TypeParameters.Select(static x => x.Replace('T', 'X')),
            $"{TypeParameters_T1_T16.Replace('T', 'X')}, X17"
        ];

        public static readonly string[] InterceptorUnmanagedTypeParameters =
        [
            string.Empty,
            .. InterceptorTypeParameters.Skip(1).Select(static x => $"{x.Replace("X", "XT")}, {x.Replace("X", "XU")}")
        ];

        private static readonly string[] QualifiedTypeParameters =
            [.. TypeParameters.Select(x => x.Replace("T", "in T"))];
    }
}