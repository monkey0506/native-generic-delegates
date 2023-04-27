// NativeGenericDelegates
// https://github.com/monkey0506/native-generic-delegates
// Copyright(C) 2022-2023 Michael Rittenhouse monkey0506@gmail.com

// This C# project and all associated project files are hereby committed to the
// public domain pursuant to the WTFPL http://www.wtfpl.net/about/ without
// warranty of any kind, express or implied, including but not limited to
// fitness for a particular purpose. Attribution is appreciated, but not
// required.

using Microsoft.CodeAnalysis;
using System;

namespace NativeGenericDelegatesGenerator
{
    internal static class Constants
    {
        public const string ActionIdentifier = "Action";
        public const string FuncIdentifier = "Func";

        public const string FromActionIdentifer = "FromAction";
        public const string FromFuncIdentifier = "FromFunc";

        public const string FromFunctionPointerIdentifier = "FromFunctionPointer";

        public const string INativeActionIdentifier = "INativeAction";
        public const string INativeActionGenericIdentifier = "INativeAction<";
        public const string INativeFuncIdentifier = "INativeFunc";
        public const string INativeFuncGenericIdentifier = "INativeFunc<";

        public const string CallConvCdecl = "Cdecl";
        public const string CallConvStdCall = "StdCall";
        public const string CallConvThisCall = "ThisCall";

        public const string UnmanagedCallConvCdecl = "Cdecl";
        public const string UnmanagedCallConvStdcall = "Stdcall";
        public const string UnmanagedCallConvThiscall = "Thiscall";

        public const string RootNamespace = "NativeGenericDelegates";

        public const string DeclarationsSourceFileName = RootNamespace + ".Declarations.g.cs";
        public const string SourceFileName = RootNamespace + ".g.cs";

        public const string MarshalAsArgumentMustUseObjectCreationSyntaxID = "NGD1001";
        public static readonly DiagnosticDescriptor MarshalAsArgumentMustUseObjectCreationSyntaxDescriptor = new
        (
            MarshalAsArgumentMustUseObjectCreationSyntaxID,
            "Invalid MarshalAs argument",
            "MarshalAs argument must be null or use object creation syntax",
            "Usage",
            DiagnosticSeverity.Error,
            true
        );

        public static readonly string[] GenericActionTypeParameters = new[]
        {
            "",
            "T",
            "T1, T2",
            "T1, T2, T3",
            "T1, T2, T3, T4",
            "T1, T2, T3, T4, T5",
            "T1, T2, T3, T4, T5, T6",
            "T1, T2, T3, T4, T5, T6, T7",
            "T1, T2, T3, T4, T5, T6, T7, T8",
            "T1, T2, T3, T4, T5, T6, T7, T8, T9",
            "T1, T2, T3, T4, T5, T6, T7, T8, T9, T10",
            "T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11",
            "T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12",
            "T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13",
            "T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14",
            "T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15",
            "T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16"
        };

        public static readonly string[] GenericFuncTypeParameters = new[]
        {
            "TResult",
            GenericActionTypeParameters[1] + ", TResult",
            GenericActionTypeParameters[2] + ", TResult",
            GenericActionTypeParameters[3] + ", TResult",
            GenericActionTypeParameters[4] + ", TResult",
            GenericActionTypeParameters[5] + ", TResult",
            GenericActionTypeParameters[6] + ", TResult",
            GenericActionTypeParameters[7] + ", TResult",
            GenericActionTypeParameters[8] + ", TResult",
            GenericActionTypeParameters[9] + ", TResult",
            GenericActionTypeParameters[10] + ", TResult",
            GenericActionTypeParameters[11] + ", TResult",
            GenericActionTypeParameters[12] + ", TResult",
            GenericActionTypeParameters[13] + ", TResult",
            GenericActionTypeParameters[14] + ", TResult",
            GenericActionTypeParameters[15] + ", TResult",
            GenericActionTypeParameters[16] + ", TResult"
        };

        public static readonly string[] NamedGenericTypeArguments = new[]
        {
            "",
            "T t",
            "T1 t1, T2 t2",
            "T1 t1, T2 t2, T3 t3",
            "T1 t1, T2 t2, T3 t3, T4 t4",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 " +
                "t15",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 " +
                "t15, T16 t16"
        };

        public static readonly string[] QualifiedGenericActionTypeParameters = new[]
        {
            "",
            "in T",
            "in T1, in T2",
            "in T1, in T2, in T3",
            "in T1, in T2, in T3, in T4",
            "in T1, in T2, in T3, in T4, in T5",
            "in T1, in T2, in T3, in T4, in T5, in T6",
            "in T1, in T2, in T3, in T4, in T5, in T6, in T7",
            "in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8",
            "in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9",
            "in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10",
            "in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11",
            "in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12",
            "in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13",
            "in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14",
            "in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15",
            "in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in" +
                " T16",
        };

        public static readonly string[] QualifiedGenericFuncTypeParameters = new[]
        {
            "out TResult",
            QualifiedGenericActionTypeParameters[1] + ", out TResult",
            QualifiedGenericActionTypeParameters[2] + ", out TResult",
            QualifiedGenericActionTypeParameters[3] + ", out TResult",
            QualifiedGenericActionTypeParameters[4] + ", out TResult",
            QualifiedGenericActionTypeParameters[5] + ", out TResult",
            QualifiedGenericActionTypeParameters[6] + ", out TResult",
            QualifiedGenericActionTypeParameters[7] + ", out TResult",
            QualifiedGenericActionTypeParameters[8] + ", out TResult",
            QualifiedGenericActionTypeParameters[9] + ", out TResult",
            QualifiedGenericActionTypeParameters[10] + ", out TResult",
            QualifiedGenericActionTypeParameters[11] + ", out TResult",
            QualifiedGenericActionTypeParameters[12] + ", out TResult",
            QualifiedGenericActionTypeParameters[13] + ", out TResult",
            QualifiedGenericActionTypeParameters[14] + ", out TResult",
            QualifiedGenericActionTypeParameters[15] + ", out TResult",
            QualifiedGenericActionTypeParameters[16] + ", out TResult",
        };

        public static readonly string[] UnmanagedGenericActionTypeConstraints = new[]
        {
            "",
            $@"
            where U : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged
            where U5 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged
            where U5 : unmanaged
            where U6 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged
            where U5 : unmanaged
            where U6 : unmanaged
            where U7 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged
            where U5 : unmanaged
            where U6 : unmanaged
            where U7 : unmanaged
            where U8 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged
            where U5 : unmanaged
            where U6 : unmanaged
            where U7 : unmanaged
            where U8 : unmanaged
            where U9 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged
            where U5 : unmanaged
            where U6 : unmanaged
            where U7 : unmanaged
            where U8 : unmanaged
            where U9 : unmanaged
            where U10 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged
            where U5 : unmanaged
            where U6 : unmanaged
            where U7 : unmanaged
            where U8 : unmanaged
            where U9 : unmanaged
            where U10 : unmanaged
            where U11 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged
            where U5 : unmanaged
            where U6 : unmanaged
            where U7 : unmanaged
            where U8 : unmanaged
            where U9 : unmanaged
            where U10 : unmanaged
            where U11 : unmanaged
            where U12 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged
            where U5 : unmanaged
            where U6 : unmanaged
            where U7 : unmanaged
            where U8 : unmanaged
            where U9 : unmanaged
            where U10 : unmanaged
            where U11 : unmanaged
            where U12 : unmanaged
            where U13 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged
            where U5 : unmanaged
            where U6 : unmanaged
            where U7 : unmanaged
            where U8 : unmanaged
            where U9 : unmanaged
            where U10 : unmanaged
            where U11 : unmanaged
            where U12 : unmanaged
            where U13 : unmanaged
            where U14 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged
            where U5 : unmanaged
            where U6 : unmanaged
            where U7 : unmanaged
            where U8 : unmanaged
            where U9 : unmanaged
            where U10 : unmanaged
            where U11 : unmanaged
            where U12 : unmanaged
            where U13 : unmanaged
            where U14 : unmanaged
            where U15 : unmanaged",
            $@"
            where U1 : unmanaged
            where U2 : unmanaged
            where U3 : unmanaged
            where U4 : unmanaged
            where U5 : unmanaged
            where U6 : unmanaged
            where U7 : unmanaged
            where U8 : unmanaged
            where U9 : unmanaged
            where U10 : unmanaged
            where U11 : unmanaged
            where U12 : unmanaged
            where U13 : unmanaged
            where U14 : unmanaged
            where U15 : unmanaged
            where U16 : unmanaged",
        };

        public static readonly string[] UnmanagedGenericFuncTypeConstraints = new[]
        {
            $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[1] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[2] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[3] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[4] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[5] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[6] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[7] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[8] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[9] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[10] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[11] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[12] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[13] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[14] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[15] + $@"
            where UResult : unmanaged",
            UnmanagedGenericActionTypeConstraints[16] + $@"
            where UResult : unmanaged",
        };

        internal static string GetUnmanagedCallConv(string callConv)
        {
            return callConv switch
            {
                CallConvCdecl => UnmanagedCallConvCdecl,
                CallConvStdCall => UnmanagedCallConvStdcall,
                CallConvThisCall => UnmanagedCallConvThiscall,
                _ => throw new NotSupportedException()
            };
        }
    }
}