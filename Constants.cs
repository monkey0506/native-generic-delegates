using System;
using System.IO;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal static partial class Constants
    {
        public const string RootNamespace = "Monkeymoto.Generators.NativeGenericDelegates";
        public const string DeclarationsSourceFileName = $"{RootNamespace}.Declarations.g.cs";
        public static readonly string DeclarationsSourceFileNamePath;
        public const string FromFunctionPointerIdentifier = "FromFunctionPointer";
        public static readonly string GeneratorAssemblyName = typeof(Constants).Assembly.GetName().Name;
        public static readonly string GeneratorClassAssemblyQualifiedName = $"{GeneratorAssemblyName}.{nameof(Generator)}";
        public static readonly string GeneratedFilesOutputPath =
            Path.Combine(GeneratorAssemblyName, GeneratorClassAssemblyQualifiedName);
        public const string SourceFileName = $"{RootNamespace}.g.cs";

        /// <summary>
        /// Returns the total number of interfaces per kind (Action or Func).
        /// </summary>
        public const int InterfaceSymbolCountPerKind = 17;
        private const int GenericSymbolCountPerInterface = 0;
        private const int GenericSymbolCountPerKind = InterfaceSymbolCountPerKind * GenericSymbolCountPerInterface;
        /// <summary>
        /// Returns the total number of interfaces (INativeAction and INativeFunc types) and generic methods that those
        /// interfaces contain.
        /// </summary>
        public const int InterfaceAndGenericMethodSymbolCount =
            ((InterfaceSymbolCountPerKind + GenericSymbolCountPerKind) * 2) - 1;

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
            Shared.Arguments_T0,
            Shared.Arguments_T1,
            Shared.Arguments_T1_T2,
            Shared.Arguments_T1_T3,
            Shared.Arguments_T1_T4,
            Shared.Arguments_T1_T5,
            Shared.Arguments_T1_T6,
            Shared.Arguments_T1_T7,
            Shared.Arguments_T1_T8,
            Shared.Arguments_T1_T9,
            Shared.Arguments_T1_T10,
            Shared.Arguments_T1_T11,
            Shared.Arguments_T1_T12,
            Shared.Arguments_T1_T13,
            Shared.Arguments_T1_T14,
            Shared.Arguments_T1_T15,
            Shared.Arguments_T1_T16,
        ];

        public static readonly string[] Parameters =
        [
            Shared.Parameters_T0,
            Shared.Parameters_T1,
            Shared.Parameters_T1_T2,
            Shared.Parameters_T1_T3,
            Shared.Parameters_T1_T4,
            Shared.Parameters_T1_T5,
            Shared.Parameters_T1_T6,
            Shared.Parameters_T1_T7,
            Shared.Parameters_T1_T8,
            Shared.Parameters_T1_T9,
            Shared.Parameters_T1_T10,
            Shared.Parameters_T1_T11,
            Shared.Parameters_T1_T12,
            Shared.Parameters_T1_T13,
            Shared.Parameters_T1_T14,
            Shared.Parameters_T1_T15,
            Shared.Parameters_T1_T16
        ];

        /// <summary>
        /// Returns a newline for a source text string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <a href="https://github.com/dotnet/roslyn/issues/51437">How should source generators emit newlines? #51437</a>.
        /// In short, both '\n' and <see cref="Environment.NewLine"/> perform inconsistently when generating source files.
        /// </para>
        /// </remarks>
        public const string NewLine =
@"
";
    }
}
