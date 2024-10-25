using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Monkeymoto.NativeGenericDelegates.Tests
{
    public sealed unsafe partial class Tests
    {
        private static class Stubs
        {
            public static void Stub_Action() { }
            public static void Stub_Action<T>(T _1) { }
            public static void Stub_Action<T1, T2>(T1 _1, T2 _2) { }
            public static void Stub_Action<T1, T2, T3>(T1 _1, T2 _2, T3 _3) { }
            public static void Stub_Action<T1, T2, T3, T4>(T1 _1, T2 _2, T3 _3, T4 _4) { }
            public static void Stub_Action<T1, T2, T3, T4, T5>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5) { }
            public static void Stub_Action<T1, T2, T3, T4, T5, T6>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6) { }
            public static void Stub_Action<T1, T2, T3, T4, T5, T6, T7>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7) { }
            public static void Stub_Action<T1, T2, T3, T4, T5, T6, T7, T8>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8) { }
            public static void Stub_Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9) { }
            public static void Stub_Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10) { }
            public static void Stub_Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11) { }
            public static void Stub_Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12) { }
            public static void Stub_Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13) { }
            public static void Stub_Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14) { }
            public static void Stub_Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, T15 _15) { }
            public static void Stub_Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, T15 _15, T16 _16) { }
            public static TResult Stub_Func<TResult>() => default!;
            public static TResult Stub_Func<T, TResult>(T _1) => default!;
            public static TResult Stub_Func<T1, T2, TResult>(T1 _1, T2 _2) => default!;
            public static TResult Stub_Func<T1, T2, T3, TResult>(T1 _1, T2 _2, T3 _3) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, TResult>(T1 _1, T2 _2, T3 _3, T4 _4) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, T5, TResult>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, T5, T6, TResult>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, T15 _15) => default!;
            public static TResult Stub_Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6, T7 _7, T8 _8, T9 _9, T10 _10, T11 _11, T12 _12, T13 _13, T14 _14, T15 _15, T16 _16) => default!;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction0() { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction1(nint _1) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction2(nint _1, nint _2) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction3(nint _1, nint _2, nint _3) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction4(nint _1, nint _2, nint _3, nint _4) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction5(nint _1, nint _2, nint _3, nint _4, nint _5) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction6(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction7(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction8(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction9(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction10(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction11(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10, nint _11) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction12(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10, nint _11, nint _12) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction13(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10, nint _11, nint _12, nint _13) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction14(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10, nint _11, nint _12, nint _13, nint _14) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction15(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10, nint _11, nint _12, nint _13, nint _14, nint _15) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static void Stub_UnmanagedAction16(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10, nint _11, nint _12, nint _13, nint _14, nint _15, nint _16) { }
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc0() => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc1(nint _1) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc2(nint _1, nint _2) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc3(nint _1, nint _2, nint _3) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc4(nint _1, nint _2, nint _3, nint _4) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc5(nint _1, nint _2, nint _3, nint _4, nint _5) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc6(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc7(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc8(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc9(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc10(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc11(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10, nint _11) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc12(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10, nint _11, nint _12) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc13(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10, nint _11, nint _12, nint _13) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc14(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10, nint _11, nint _12, nint _13, nint _14) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc15(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10, nint _11, nint _12, nint _13, nint _14, nint _15) => default;
            [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
            public static nint Stub_UnmanagedFunc16(nint _1, nint _2, nint _3, nint _4, nint _5, nint _6, nint _7, nint _8, nint _9, nint _10, nint _11, nint _12, nint _13, nint _14, nint _15, nint _16) => default;

            public static nint ToPointer(delegate* unmanaged[Cdecl]<void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, void> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint> ptr) => (nint)ptr;
            public static nint ToPointer(delegate* unmanaged[Cdecl]<nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint, nint> ptr) => (nint)ptr;
        }
    }
}
