using System.IO;
using System.Runtime.InteropServices;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal static partial class Constants
    {
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

        public static readonly CallingConvention DefaultCallingConvention = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            CallingConvention.StdCall :
            CallingConvention.Cdecl;

        public const string FromFunctionPointerIdentifier = "FromFunctionPointer";

        public static readonly string GeneratorAssemblyName = typeof(Constants).Assembly.GetName().Name;
        public static readonly string GeneratorClassAssemblyQualifiedName = $"{GeneratorAssemblyName}.{nameof(Generator)}";
        public static readonly string GeneratedFilesOutputPath =
            Path.Combine(GeneratorAssemblyName, GeneratorClassAssemblyQualifiedName);

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

        public const string RootNamespace = "Monkeymoto.Generators.NativeGenericDelegates";
        public const string DeclarationsSourceFileName = $"{RootNamespace}.Declarations.g.cs";
        public static readonly string DeclarationsSourceFileNamePath;
        public const string SourceFileName = $"{RootNamespace}.g.cs";

        static Constants()
        {
            if (!GeneratedFilesOutputPath.EndsWith($"{Path.DirectorySeparatorChar}") &&
                !GeneratedFilesOutputPath.EndsWith($"{Path.AltDirectorySeparatorChar}"))
            {
                GeneratedFilesOutputPath += Path.DirectorySeparatorChar;
            }
            DeclarationsSourceFileNamePath = Path.Combine(GeneratedFilesOutputPath, DeclarationsSourceFileName);
        }

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
    }
}
