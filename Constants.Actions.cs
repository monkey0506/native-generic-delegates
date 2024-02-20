using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal static partial class Constants
    {
        internal static class Actions
        {
            public static string FromDelegateIdentifier = "FromAction";

            public static readonly string[] TypeParameters = Shared.TypeParameters;
            public static readonly string[] QualifiedTypeParameters = Shared.QualifiedTypeParameters;
            public static readonly string[] UnmanagedTypeConstraints = Shared.UnmanagedTypeConstraints;
            public static readonly string[] UnmanagedTypeParameters = Shared.UnmanagedTypeParameters;

            public static readonly string[] Interfaces =
            [
                "INativeAction",
                .. QualifiedTypeParameters.Skip(1).Select(x => $"INativeAction<{x}>")
            ];

            public static readonly string[] MetadataNames =
            [
                $"{RootNamespace}.INativeAction",
                $"{RootNamespace}.INativeAction`1",
                $"{RootNamespace}.INativeAction`2",
                $"{RootNamespace}.INativeAction`3",
                $"{RootNamespace}.INativeAction`4",
                $"{RootNamespace}.INativeAction`5",
                $"{RootNamespace}.INativeAction`6",
                $"{RootNamespace}.INativeAction`7",
                $"{RootNamespace}.INativeAction`8",
                $"{RootNamespace}.INativeAction`9",
                $"{RootNamespace}.INativeAction`10",
                $"{RootNamespace}.INativeAction`11",
                $"{RootNamespace}.INativeAction`12",
                $"{RootNamespace}.INativeAction`13",
                $"{RootNamespace}.INativeAction`14",
                $"{RootNamespace}.INativeAction`15",
                $"{RootNamespace}.INativeAction`16",
            ];
        }
    }
}
