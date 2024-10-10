using System.Linq;

namespace Monkeymoto.NativeGenericDelegates
{
    internal static partial class Constants
    {
        internal static class Actions
        {
            public static readonly string[] AntiConstraints = Constants.AntiConstraints;

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
                "",
                $"{RootNamespace}.IUnmanagedAction`2",
                $"{RootNamespace}.IUnmanagedAction`4",
                $"{RootNamespace}.IUnmanagedAction`6",
                $"{RootNamespace}.IUnmanagedAction`8",
                $"{RootNamespace}.IUnmanagedAction`10",
                $"{RootNamespace}.IUnmanagedAction`12",
                $"{RootNamespace}.IUnmanagedAction`14",
                $"{RootNamespace}.IUnmanagedAction`16",
                $"{RootNamespace}.IUnmanagedAction`18",
                $"{RootNamespace}.IUnmanagedAction`20",
                $"{RootNamespace}.IUnmanagedAction`22",
                $"{RootNamespace}.IUnmanagedAction`24",
                $"{RootNamespace}.IUnmanagedAction`26",
                $"{RootNamespace}.IUnmanagedAction`28",
                $"{RootNamespace}.IUnmanagedAction`30",
                $"{RootNamespace}.IUnmanagedAction`32"
            ];

            public static readonly string[] QualifiedTypeParameters = Constants.QualifiedTypeParameters;
            public static readonly string[] TypeParameters = Constants.TypeParameters;

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
