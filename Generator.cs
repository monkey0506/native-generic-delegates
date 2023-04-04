// NativeGenericDelegates
// https://github.com/monkey0506/native-generic-delegates
// Copyright(C) 2022-2023 Michael Rittenhouse monkey0506@gmail.com

// This C# project and all associated project files are hereby committed to the
// public domain pursuant to the WTFPL http://www.wtfpl.net/about/ without
// warranty of any kind, express or implied, including but not limited to
// fitness for a particular purpose. Attribution is appreciated, but not
// required.

// About this version
//
// v1.0.0 of this project aimed to provide dynamically instanciated Delegate
// objects that implemented one of the provided interfaces, but relied on
// classes from System.Reflection.Emit, which is incompatible with some .NET
// platforms (those using AOT compilation).
//
// This version instead aims to create classes that implement the provided
// interfaces at compile-time, using an incremental source generator. This
// version is a work-in-progress that may have bugs and is not feature
// complete. Importantly, the generated class objects are not instances of the
// Delegate or MulticastDelegate types. There is also no support for custom
// marshalling at this time.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace NativeGenericDelegatesGenerator
{
    [Generator]
    public class Generator : IIncrementalGenerator
    {
        private const string ActionIdentifier = "Action";
        private const string FuncIdentifier = "Func";

        private const string FromActionIdentifer = "FromAction";
        private const string FromFuncIdentifier = "FromFunc";

        private const string FromFunctionPointerIdentifier = "FromFunctionPointer";

        private const string INativeActionIdentifier = "INativeAction";
        private const string INativeActionGenericIdentifier = "INativeAction<";
        private const string INativeFuncIdentifier = "INativeFunc";
        private const string INativeFuncGenericIdentifier = "INativeFunc<";

        private const string CallConvCdecl = "Cdecl";
        private const string CallConvStdCall = "StdCall";
        private const string CallConvThisCall = "ThisCall";

        private const string UnmanagedCallConvCdecl = "Cdecl";
        private const string UnmanagedCallConvStdcall = "Stdcall";
        private const string UnmanagedCallConvThiscall = "Thiscall";

        private const string RootNamespace = "NativeGenericDelegates";

        private const string DeclarationsSourceFileName = RootNamespace + ".Declarations.g.cs";
        private const string SourceFileName = RootNamespace + ".g.cs";

        private static readonly string[] GenericActionTypeParameters = new[]
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

        private static readonly string[] GenericFuncTypeParameters = new[]
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

        private static readonly string[] NamedGenericTypeArguments = new[]
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
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15",
            "T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16"
        };

        private static readonly string[] QualifiedGenericActionTypeParameters = new[]
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
            "in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16",
        };

        private static readonly string[] QualifiedGenericFuncTypeParameters = new[]
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

        private static readonly string[] UnmanagedGenericActionTypeConstraints = new[]
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

        private static readonly string[] UnmanagedGenericFuncTypeConstraints = new[]
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

        private readonly struct NativeGenericDelegateInfo
        {
            public readonly string ClassNamePrefix;
            public readonly string FunctionPointerTypeArgumentsWithReturnType;
            public readonly string Identifier;
            public readonly string IdentifierWithTypeArguments;
            public readonly string IdentifierWithTypeParameters;
            public readonly string InvokeArguments;
            public readonly string InvokeReturnType;
            public readonly bool IsAction;
            public readonly bool IsFromFunctionPointerGeneric;
            public readonly string NamedArguments;
            public readonly string ReturnKeyword;
            public readonly string TypeArgumentCheckCondition;
            public readonly int TypeArgumentCount;
            public readonly string TypeArguments;
            public readonly bool UnmanagedTypeArgumentsOnly;

            public NativeGenericDelegateInfo(IMethodSymbol methodSymbol, CancellationToken cancellationToken)
            {
                INamedTypeSymbol interfaceSymbol = methodSymbol.ContainingType;
                bool isAction = interfaceSymbol.Name.StartsWith(INativeActionIdentifier);
                string identifier = isAction ? ActionIdentifier : FuncIdentifier;
                IEnumerable<int> range = Enumerable.Range(1, interfaceSymbol.Arity);
                ImmutableArray<string> typeArguments = interfaceSymbol.TypeArguments.Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)).ToImmutableArray();
                ImmutableArray<string> typeParameters = interfaceSymbol.TypeParameters.Select(x => x.ToString()).ToImmutableArray();
                cancellationToken.ThrowIfCancellationRequested();
                ClassNamePrefix = $"Native{identifier}_{Guid.NewGuid():N}";
                FunctionPointerTypeArgumentsWithReturnType = string.Join(", ", typeArguments);
                Identifier = identifier;
                IdentifierWithTypeArguments = $"{identifier}<{FunctionPointerTypeArgumentsWithReturnType}>";
                IdentifierWithTypeParameters = $"{identifier}<{string.Join(", ", typeParameters)}>";
                InvokeArguments = string.Join(", ", range.Take(typeArguments.Length - (isAction ? 0 : 1)).Select(x => $"_{x}"));
                InvokeReturnType = isAction ? "void" : typeArguments[typeArguments.Length - 1];
                IsAction = isAction;
                IsFromFunctionPointerGeneric = methodSymbol.IsGenericMethod;
                NamedArguments = string.Join(", ", range.Take(typeArguments.Length - (isAction ? 0 : 1)).Select(x => $"{typeArguments[x - 1]} _{x}"));
                ReturnKeyword = isAction ? "" : "return ";
                TypeArgumentCount = typeArguments.Length;
                TypeArguments = string.Join(", ", typeArguments.Take(typeArguments.Length - (isAction ? 0 : 1)));
                UnmanagedTypeArgumentsOnly = interfaceSymbol.TypeArguments.All(x => x.IsUnmanagedType);
                cancellationToken.ThrowIfCancellationRequested();
                StringBuilder sb = new();
                bool wrap = interfaceSymbol.Arity > 1;
                for (int i = 0; i < interfaceSymbol.Arity; ++i)
                {
                    _ = sb.Append($"{(wrap ? "(" : "")}typeof({typeParameters[i]}) == typeof({typeArguments[i]}){(wrap ? ")" : "")}");
                    if ((i + 1) < interfaceSymbol.Arity)
                    {
                        _ = sb.Append(" && ");
                    }
                }
                TypeArgumentCheckCondition = sb.ToString();
                cancellationToken.ThrowIfCancellationRequested();
                if (methodSymbol.IsGenericMethod)
                {
                    typeArguments = methodSymbol.TypeArguments.Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)).ToImmutableArray();
                    FunctionPointerTypeArgumentsWithReturnType = $"{string.Join(", ", typeArguments)}{(isAction ? ", void" : "")}";
                    if (!wrap)
                    {
                        _ = sb.Insert(0, '(').Append(')');
                    }
                    _ = sb.Append(" && ");
                    for (int i = 0; i < typeArguments.Length; ++i)
                    {
                        _ = sb.Append($"(typeof({methodSymbol.TypeParameters[i]}) == typeof({typeArguments[i]}))");
                        if ((i + 1) < typeArguments.Length)
                        {
                            _ = sb.Append(" && ");
                        }
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                    TypeArgumentCheckCondition = sb.ToString();
                }
                else if (isAction)
                {
                    FunctionPointerTypeArgumentsWithReturnType += ", void";
                }
            }
        }

        private readonly struct NativeGenericDelegateConcreteClassInfo
        {
            public readonly int ArgumentCount;
            public readonly string ClassDefinitions;
            public readonly string FromDelegateTypeCheck;
            public readonly string FromFunctionPointerTypeCheck;
            public readonly string FromFunctionPointerGenericTypeCheck;
            public readonly bool IsAction;

            private static void BuildClassDefinition(StringBuilder sb, in NativeGenericDelegateInfo info, string callConv)
            {
                string unmanagedCallConv = GetUnmanagedCallConv(callConv);
                string funcPtr = $"delegate* unmanaged[{unmanagedCallConv}]<{info.FunctionPointerTypeArgumentsWithReturnType}>";
                string call = info.UnmanagedTypeArgumentsOnly ? $"{info.ReturnKeyword}(({funcPtr})_functionPtr)({info.InvokeArguments});" : $"{info.ReturnKeyword}_delegate({info.InvokeArguments});";
                string getDelegate = info.UnmanagedTypeArgumentsOnly ? "Invoke" : $"Marshal.GetDelegateForFunctionPointer<NonGeneric{info.Identifier}>(functionPtr)";
                _ = sb.AppendLine($@"
    file unsafe class {info.ClassNamePrefix}_{callConv} : INative{info.IdentifierWithTypeArguments}
    {{
        private readonly NonGeneric{info.Identifier} _delegate;
        private readonly {funcPtr} _functionPtr;

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate {info.InvokeReturnType} NonGeneric{info.Identifier}({info.NamedArguments});

        internal {info.ClassNamePrefix}_{callConv}({info.IdentifierWithTypeArguments} _delegate)
        {{
            ArgumentNullException.ThrowIfNull(_delegate);
            this._delegate = (NonGeneric{info.Identifier})Delegate.CreateDelegate(typeof(NonGeneric{info.Identifier}), _delegate.Target, _delegate.Method);
            _functionPtr = ({funcPtr})Marshal.GetFunctionPointerForDelegate(this._delegate);
        }}

        internal {info.ClassNamePrefix}_{callConv}(nint functionPtr)
        {{
            ArgumentNullException.ThrowIfNull((void*)functionPtr);
            _delegate = {getDelegate};
            _functionPtr = ({funcPtr})functionPtr;
        }}

        public nint GetFunctionPointer()
        {{
            return (nint)_functionPtr;
        }}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnmanagedCallConv(CallConvs = new[] {{ typeof(CallConv{unmanagedCallConv}) }})]
        public {info.InvokeReturnType} Invoke({info.NamedArguments})
        {{
            {call}
        }}

        public {info.IdentifierWithTypeArguments} To{info.Identifier}()
        {{
            return ({info.IdentifierWithTypeArguments})Delegate.CreateDelegate(typeof({info.IdentifierWithTypeArguments}), _delegate.Target, _delegate.Method);
        }}
    }}");
            }

            private static string BuildClassDefinitions(in NativeGenericDelegateInfo info)
            {
                StringBuilder sb = new();
                BuildClassDefinition(sb, in info, CallConvCdecl);
                BuildClassDefinition(sb, in info, CallConvStdCall);
                BuildClassDefinition(sb, in info, CallConvThisCall);
                return sb.ToString();
            }

            private static void BuildTypeCheck(StringBuilder sb, in NativeGenericDelegateInfo info, bool isDelegate)
            {
                string arg = "functionPtr";
                string cast = "";
                if (isDelegate)
                {
                    arg = "_delegate";
                    cast = $"({info.IdentifierWithTypeArguments})(object)";
                }
                _ = sb.AppendLine($@"            if ({info.TypeArgumentCheckCondition})
            {{
                return (INative{info.IdentifierWithTypeParameters})(object)(callingConvention switch
                {{
                    CallingConvention.Cdecl => new {info.ClassNamePrefix}_Cdecl({cast}{arg}),
                    CallingConvention.StdCall => new {info.ClassNamePrefix}_StdCall({cast}{arg}),
                    CallingConvention.ThisCall => new {info.ClassNamePrefix}_ThisCall({cast}{arg}),
                    _ => throw new NotSupportedException()
                }});
            }}");
            }

            private static (string FromDelegate, string FromFunctionPointer, string FromFunctionPointerGeneric) BuildTypeChecks(in NativeGenericDelegateInfo info)
            {
                StringBuilder fromDelegate = new();
                StringBuilder fromFunctionPointer = new();
                StringBuilder fromFunctionPointerGeneric = new();
                if (info.IsFromFunctionPointerGeneric)
                {
                    BuildTypeCheck(fromFunctionPointerGeneric, in info, isDelegate: false);
                }
                else
                {
                    BuildTypeCheck(fromDelegate, in info, isDelegate: true);
                    BuildTypeCheck(fromFunctionPointer, in info, isDelegate: false);
                }
                return (fromDelegate.ToString(), fromFunctionPointer.ToString(), fromFunctionPointerGeneric.ToString());
            }

            public NativeGenericDelegateConcreteClassInfo(in NativeGenericDelegateInfo info)
            {
                ArgumentCount = info.TypeArgumentCount - (info.IsAction ? 0 : 1);
                ClassDefinitions = BuildClassDefinitions(in info);
                (FromDelegateTypeCheck, FromFunctionPointerTypeCheck, FromFunctionPointerGenericTypeCheck) = BuildTypeChecks(in info);
                IsAction = info.IsAction;
            }
        }

        private static void BuildNativeAction(StringBuilder sb, string callConv)
        {
            string unmanagedCallConv = GetUnmanagedCallConv(callConv);
            string funcPtr = $"delegate* unmanaged[{unmanagedCallConv}]<void>";
            _ = sb.AppendLine($@"    file unsafe class NativeAction_{callConv} : INativeAction
    {{
        private readonly NonGenericAction nonGenericAction;
        private readonly {funcPtr} functionPtr;

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate void NonGenericAction();

        public NativeAction_{callConv}(Action action)
        {{
            nonGenericAction = (NonGenericAction)Delegate.CreateDelegate(typeof(Action), action.Target, action.Method);
            functionPtr = ({funcPtr})Marshal.GetFunctionPointerForDelegate(nonGenericAction);
        }}

        public NativeAction_{callConv}(nint functionPtr)
        {{
            nonGenericAction = Invoke;
            this.functionPtr = ({funcPtr})functionPtr;
        }}

        public nint GetFunctionPointer()
        {{
            return (nint)functionPtr;
        }}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnmanagedCallConv(CallConvs = new[] {{ typeof(CallConv{unmanagedCallConv}) }})]
        public void Invoke()
        {{
            (({funcPtr})functionPtr)();
        }}

        public Action ToAction()
        {{
            return (Action)Delegate.CreateDelegate(typeof(Action), nonGenericAction.Target, nonGenericAction.Method);
        }}
    }}
");
        }

        private static void BuildNativeActions(StringBuilder sb)
        {
            BuildNativeAction(sb, CallConvCdecl);
            BuildNativeAction(sb, CallConvStdCall);
            BuildNativeAction(sb, CallConvThisCall);
        }

        private static string BuildPartialInterfaceDeclaration(bool isAction, int argumentCount)
        {
            string constraints = UnmanagedGenericActionTypeConstraints[argumentCount];
            string identifier = ActionIdentifier;
            string qualifiedTypeParameters = QualifiedGenericActionTypeParameters[argumentCount];
            string returnType = "void";
            string typeParameters = GenericActionTypeParameters[argumentCount];
            if (!isAction)
            {
                constraints = UnmanagedGenericFuncTypeConstraints[argumentCount];
                identifier = FuncIdentifier;
                qualifiedTypeParameters = QualifiedGenericFuncTypeParameters[argumentCount];
                returnType = "TResult";
                typeParameters = GenericFuncTypeParameters[argumentCount];
            }
            string genericIdentifier = $"{identifier}<{typeParameters}>";
            string namedArguments = NamedGenericTypeArguments[argumentCount];
            string unmanagedTypeParameters = typeParameters.Replace("T", "U");
            return $@"    public partial interface INative{identifier}<{qualifiedTypeParameters}>
    {{
        public static partial INative{genericIdentifier} From{identifier}({genericIdentifier} {identifier.ToLower()}, CallingConvention callingConvention = CallingConvention.Winapi);
        public static partial INative{genericIdentifier} FromFunctionPointer(nint functionPtr, CallingConvention callingConvention = CallingConvention.Winapi);
        public static partial INative{genericIdentifier} FromFunctionPointer<{unmanagedTypeParameters}>(nint functionPtr, CallingConvention callingConvention = CallingConvention.Winapi){constraints};

        public nint GetFunctionPointer();
        public {returnType} Invoke({namedArguments});
        public {genericIdentifier} To{identifier}();
    }}
";
        }

        private static string BuildPartialInterfaceImplementation(bool isAction, int argumentCount, StringBuilder fromDelegate, StringBuilder fromFunctionPointer, StringBuilder fromFunctionPointerGeneric)
        {
            string constraints = UnmanagedGenericActionTypeConstraints[argumentCount];
            string identifier = ActionIdentifier;
            string qualifiedTypeParameters = QualifiedGenericActionTypeParameters[argumentCount];
            string typeParameters = GenericActionTypeParameters[argumentCount];
            if (!isAction)
            {
                constraints = UnmanagedGenericFuncTypeConstraints[argumentCount];
                identifier = FuncIdentifier;
                qualifiedTypeParameters = QualifiedGenericFuncTypeParameters[argumentCount];
                typeParameters = GenericFuncTypeParameters[argumentCount];
            }
            string genericIdentifier = $"{identifier}<{typeParameters}>";
            string unmanagedTypeParameters = typeParameters.Replace('T', 'U');
            int index = argumentCount - (isAction ? 1 : 0);
            return $@"    public partial interface INative{identifier}<{qualifiedTypeParameters}>
    {{
        public static partial INative{genericIdentifier} From{identifier}({genericIdentifier} _delegate, CallingConvention callingConvention)
        {{
            {fromDelegate}
        }}

        public static partial INative{genericIdentifier} FromFunctionPointer(nint functionPtr, CallingConvention callingConvention)
        {{
            {fromFunctionPointer}
        }}

        public static partial INative{genericIdentifier} FromFunctionPointer<{unmanagedTypeParameters}>(nint functionPtr, CallingConvention callingConvention){constraints}
        {{
            {fromFunctionPointerGeneric}
        }}
    }}
";
        }

        private static void BuildTypeCheck(ref StringBuilder sb, string typeCheck)
        {
            if (sb is null)
            {
                sb = new($@"if (callingConvention == CallingConvention.Winapi)
            {{
                callingConvention = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? CallingConvention.StdCall : CallingConvention.Cdecl;
            }}
");
            }
            _ = sb.Append(typeCheck);
        }

        private static string GetUnmanagedCallConv(string callConv)
        {
            return callConv switch
            {
                CallConvCdecl => UnmanagedCallConvCdecl,
                CallConvStdCall => UnmanagedCallConvStdcall,
                CallConvThisCall => UnmanagedCallConvThiscall,
                _ => throw new NotSupportedException()
            };
        }

        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            var methodSymbols = initContext.SyntaxProvider.CreateSyntaxProvider(static (node, _) =>
            {
                if (node is InvocationExpressionSyntax)
                {
                    string s = node.ToString();
                    if (s.StartsWith(INativeActionGenericIdentifier) || s.StartsWith(INativeFuncGenericIdentifier))
                    {
                        return true;
                    }
                }
                return false;
            },
            static (context, _) => context).Collect().SelectMany(static (contextArray, cancellationToken) =>
            {
                // we use a hash set to denote a unique combination of interface type arguments and method type arguments (for generic FromFunctionPointer)
                // we don't want to create duplicate classes if we are using FromAction/FromFunc alongside the non-generic FromFunctionPointer with the same
                // interface type arguments, so we can't use the IMethodSymbol as the hash key in these cases, hence the separate list of IMethodSymbols
                HashSet<ISymbol> symbolHashSet = new(SymbolEqualityComparer.Default);
                List<IMethodSymbol> symbolList = new();
                foreach (var context in contextArray)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    IMethodSymbol? methodSymbol = (IMethodSymbol?)context.SemanticModel.GetSymbolInfo(((InvocationExpressionSyntax)context.Node).Expression, cancellationToken).Symbol;
                    switch (methodSymbol?.Name)
                    {
                        case FromActionIdentifer:
                        case FromFuncIdentifier:
                        case FromFunctionPointerIdentifier:
                            break;
                        default:
                            continue;
                    }
                    INamedTypeSymbol? interfaceSymbol = methodSymbol.ContainingType;
                    // this may be overkill (and there may be a simpler way to validate this as one of the interfaces we define), but this should cover any erroneous matches
                    if ((interfaceSymbol is null) || (interfaceSymbol.ContainingNamespace is null) || (interfaceSymbol.ContainingNamespace.ContainingNamespace is null) ||
                        (!interfaceSymbol.ContainingNamespace.ContainingNamespace.IsGlobalNamespace) || (interfaceSymbol.ContainingNamespace.Name != RootNamespace) ||
                        (interfaceSymbol.TypeKind != TypeKind.Interface) || !interfaceSymbol.IsGenericType || (interfaceSymbol.Arity == 0) ||
                        ((interfaceSymbol.Name != INativeActionIdentifier) && (interfaceSymbol.Name != INativeFuncIdentifier)))
                    {
                        continue;
                    }
                    if (methodSymbol.IsGenericMethod)
                    {
                        if (methodSymbol.Arity != interfaceSymbol.Arity)
                        {
                            continue;
                        }
                        if (methodSymbol.TypeArguments.Where(x => x is not INamedTypeSymbol namedTypeArgument || namedTypeArgument.IsGenericType).Any())
                        {
                            continue;
                        }
                    }
                    if (interfaceSymbol.TypeArguments.Where(x => x is not INamedTypeSymbol namedTypeArgument || namedTypeArgument.IsGenericType).Any())
                    {
                        continue;
                    }
                    if (symbolHashSet.Add(methodSymbol.IsGenericMethod ? methodSymbol : interfaceSymbol))
                    {
                        symbolList.Add(methodSymbol);
                    }
                }
                return symbolList.ToImmutableArray();
            });
            var infos = methodSymbols.Select(static (methodSymbol, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return new NativeGenericDelegateInfo(methodSymbol, cancellationToken);
            }).Select(static (info, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return new NativeGenericDelegateConcreteClassInfo(in info);
            }).Collect();
            initContext.RegisterPostInitializationOutput(PostInitialization);
            initContext.RegisterSourceOutput(infos, static (context, infos) =>
            {
                StringBuilder classes = new();
                StringBuilder[] fromAction = new StringBuilder[17];
                StringBuilder[] fromFunc = new StringBuilder[17];
                StringBuilder[] fromFunctionPointerAction = new StringBuilder[17];
                StringBuilder[] fromFunctionPointerFunc = new StringBuilder[17];
                StringBuilder[] fromFunctionPointerActionGeneric = new StringBuilder[17];
                StringBuilder[] fromFunctionPointerFuncGeneric = new StringBuilder[17];
                StringBuilder notImplementedType = new("throw new NotImplementedException();");
                string notImplementedFallthrough = $"            {notImplementedType}";
                StringBuilder partialImplementations = new();
                foreach (var info in infos)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    classes.Append(info.ClassDefinitions);
                    int index = info.ArgumentCount - (info.IsAction ? 1 : 0);
                    if (info.IsAction)
                    {
                        BuildTypeCheck(ref fromAction[index], info.FromDelegateTypeCheck);
                        BuildTypeCheck(ref fromFunctionPointerAction[index], info.FromFunctionPointerTypeCheck);
                        BuildTypeCheck(ref fromFunctionPointerActionGeneric[index], info.FromFunctionPointerGenericTypeCheck);
                    }
                    else
                    {
                        BuildTypeCheck(ref fromFunc[index], info.FromDelegateTypeCheck);
                        BuildTypeCheck(ref fromFunctionPointerFunc[index], info.FromFunctionPointerTypeCheck);
                        BuildTypeCheck(ref fromFunctionPointerFuncGeneric[index], info.FromFunctionPointerGenericTypeCheck);
                    }
                }
                for (int i = 0; i < 17; ++i)
                {
                    fromAction[i] = fromAction[i]?.Append(notImplementedFallthrough) ?? notImplementedType;
                    fromFunc[i] = fromFunc[i]?.Append(notImplementedFallthrough) ?? notImplementedType;
                    fromFunctionPointerAction[i] = fromFunctionPointerAction[i]?.Append(notImplementedFallthrough) ?? notImplementedType;
                    fromFunctionPointerFunc[i] = fromFunctionPointerFunc[i]?.Append(notImplementedFallthrough) ?? notImplementedType;
                    fromFunctionPointerActionGeneric[i] = fromFunctionPointerActionGeneric[i]?.Append(notImplementedFallthrough) ?? notImplementedType;
                    fromFunctionPointerFuncGeneric[i] = fromFunctionPointerFuncGeneric[i]?.Append(notImplementedFallthrough) ?? notImplementedType;
                }
                var unimplementedActions = Enumerable.Range(1, 16).ToList();
                var unimplementedFuncs = Enumerable.Range(0, 17).ToList();
                foreach (var info in infos)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    int index = info.ArgumentCount - (info.IsAction ? 1 : 0);
                    if (info.IsAction)
                    {
                        if (unimplementedActions.Remove(info.ArgumentCount))
                        {
                            partialImplementations.AppendLine().Append(BuildPartialInterfaceImplementation(info.IsAction, info.ArgumentCount, fromAction[index], fromFunctionPointerAction[index], fromFunctionPointerActionGeneric[index]));
                        }
                    }
                    else if (unimplementedFuncs.Remove(info.ArgumentCount))
                    {
                        partialImplementations.AppendLine().Append(BuildPartialInterfaceImplementation(info.IsAction, info.ArgumentCount, fromFunc[index], fromFunctionPointerFunc[index], fromFunctionPointerFuncGeneric[index]));
                    }
                }
                foreach (var actionArgumentCount in unimplementedActions)
                {
                    partialImplementations.AppendLine().Append(BuildPartialInterfaceImplementation(isAction: true, actionArgumentCount, notImplementedType, notImplementedType, notImplementedType));
                }
                foreach (var funcArgumentCount in unimplementedFuncs)
                {
                    partialImplementations.AppendLine().Append(BuildPartialInterfaceImplementation(isAction: false, funcArgumentCount, notImplementedType, notImplementedType, notImplementedType));
                }
                StringBuilder source = new($@"// <auto-generated/>
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS8826 // Partial method declarations have different signatures (erroneous)

namespace {RootNamespace}
{{{classes}{partialImplementations}}}

#pragma warning restore CS8826 // Partial method declarations have different signatures (erroneous)
");
                context.AddSource(SourceFileName, source.ToString());
            });
        }

        private static void PostInitialization(IncrementalGeneratorPostInitializationContext context)
        {
            var source = new StringBuilder($@"// <auto-generated/>
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace {RootNamespace}
{{
    public interface INativeAction
    {{
        public static INativeAction FromAction(Action action, CallingConvention callingConvention = CallingConvention.Winapi)
        {{
            if (callingConvention == CallingConvention.Winapi)
            {{
                callingConvention = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? CallingConvention.StdCall : CallingConvention.Cdecl;
            }}
            return callingConvention switch
            {{
                CallingConvention.Cdecl => new NativeAction_Cdecl(action),
                CallingConvention.StdCall => new NativeAction_StdCall(action),
                CallingConvention.ThisCall => new NativeAction_ThisCall(action),
                _ => throw new NotSupportedException()
            }};
        }}

        public static INativeAction FromFunctionPointer(nint functionPtr, CallingConvention callingConvention = CallingConvention.Winapi)
        {{
            if (callingConvention == CallingConvention.Winapi)
            {{
                callingConvention = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? CallingConvention.StdCall : CallingConvention.Cdecl;
            }}
            return callingConvention switch
            {{
                CallingConvention.Cdecl => new NativeAction_Cdecl(functionPtr),
                CallingConvention.StdCall => new NativeAction_StdCall(functionPtr),
                CallingConvention.ThisCall => new NativeAction_ThisCall(functionPtr),
                _ => throw new NotSupportedException()
            }};
        }}

        public nint GetFunctionPointer();
        public void Invoke();
        public Action ToAction();
    }}

");
            BuildNativeActions(source);
            _ = source.Append($"{BuildPartialInterfaceDeclaration(isAction: false, 0)}");
            for (int i = 1; i < 17; ++i)
            {
                _ = source.AppendLine().AppendLine($"{BuildPartialInterfaceDeclaration(isAction: true, argumentCount: i)}");
                _ = source.Append($"{BuildPartialInterfaceDeclaration(isAction: false, argumentCount: i)}");
            }
            _ = source.AppendLine("}");
            context.AddSource(DeclarationsSourceFileName, source.ToString());
        }
    }
}
