using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal static partial class Constants
    {
        internal static class Funcs
        {
            public static string FromDelegateIdentifier = "FromFunc";

            public static readonly string[] QualifiedTypeParameters =
            [
                "out TResult",
                .. Actions.QualifiedTypeParameters.Skip(1).Select(x => $"{x}, out TResult")
            ];

            public static readonly string[] TypeParameters =
            [
                "TResult",
                .. Shared.TypeParameters.Skip(1).Select(x => $"{x}, TResult")
            ];

            public static readonly string[] Interfaces = [.. QualifiedTypeParameters.Select(x => $"INativeFunc<{x}>")];

            public static readonly string[] MetadataNames =
            [
                $"{RootNamespace}.INativeFunc`1",
                $"{RootNamespace}.INativeFunc`2",
                $"{RootNamespace}.INativeFunc`3",
                $"{RootNamespace}.INativeFunc`4",
                $"{RootNamespace}.INativeFunc`5",
                $"{RootNamespace}.INativeFunc`6",
                $"{RootNamespace}.INativeFunc`7",
                $"{RootNamespace}.INativeFunc`8",
                $"{RootNamespace}.INativeFunc`9",
                $"{RootNamespace}.INativeFunc`10",
                $"{RootNamespace}.INativeFunc`11",
                $"{RootNamespace}.INativeFunc`12",
                $"{RootNamespace}.INativeFunc`13",
                $"{RootNamespace}.INativeFunc`14",
                $"{RootNamespace}.INativeFunc`15",
                $"{RootNamespace}.INativeFunc`16",
                $"{RootNamespace}.INativeFunc`17",
            ];
        }
    }
}
