using System.Linq;

namespace Monkeymoto.NativeGenericDelegates
{
    internal static partial class Constants
    {
        internal static class Funcs
        {
            public static readonly string[] AntiConstraints =
            [
                $"{NewLineIndent2}where TResult : allows ref struct",
                .. Constants.AntiConstraints.Skip(1)
                    .Select(static x => $"{x}{NewLineIndent2}where TResult : allows ref struct")
            ];

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
                $"{RootNamespace}.IUnmanagedFunc`2",
                $"{RootNamespace}.IUnmanagedFunc`4",
                $"{RootNamespace}.IUnmanagedFunc`6",
                $"{RootNamespace}.IUnmanagedFunc`8",
                $"{RootNamespace}.IUnmanagedFunc`10",
                $"{RootNamespace}.IUnmanagedFunc`12",
                $"{RootNamespace}.IUnmanagedFunc`14",
                $"{RootNamespace}.IUnmanagedFunc`16",
                $"{RootNamespace}.IUnmanagedFunc`18",
                $"{RootNamespace}.IUnmanagedFunc`20",
                $"{RootNamespace}.IUnmanagedFunc`22",
                $"{RootNamespace}.IUnmanagedFunc`24",
                $"{RootNamespace}.IUnmanagedFunc`26",
                $"{RootNamespace}.IUnmanagedFunc`28",
                $"{RootNamespace}.IUnmanagedFunc`30",
                $"{RootNamespace}.IUnmanagedFunc`32",
                $"{RootNamespace}.IUnmanagedFunc`34"
            ];

            public static readonly string[] QualifiedTypeParameters =
            [
                "out TResult",
                .. Constants.QualifiedTypeParameters.Skip(1).Select(x => $"{x}, out TResult")
            ];

            public static readonly string[] TypeParameters =
            [
                "TResult",
                .. Constants.TypeParameters.Skip(1).Select(x => $"{x}, TResult")
            ];

            public static readonly string[] UnmanagedConstraints =
            [
                .. AntiConstraints.Select(static x =>
                {
                    var unmanaged = x.Replace(": allows", ": unmanaged, allows").Replace('T', 'U');
                    return $"{x}{unmanaged}";
                })
            ];
        }
    }
}
