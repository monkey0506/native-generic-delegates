using System.Linq;
using System.Runtime.CompilerServices;

namespace Monkeymoto.NativeGenericDelegates
{
    internal static partial class Constants
    {
        internal static class Funcs
        {
            static Funcs()
            {
                RuntimeHelpers.RunClassConstructor(typeof(Constants).TypeHandle);
            }

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
        }
    }
}
