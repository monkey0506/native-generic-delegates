// NativeGenericDelegates
// https://github.com/monkey0506/native-generic-delegates
// Copyright(C) 2022-2023 Michael Rittenhouse monkey0506@gmail.com

// This C# project and all associated project files are hereby committed to the
// public domain pursuant to the WTFPL http://www.wtfpl.net/about/ without
// warranty of any kind, express or implied, including but not limited to
// fitness for a particular purpose. Attribution is appreciated, but not
// required.

using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System;

namespace NativeGenericDelegatesGenerator
{
    internal readonly struct NativeGenericDelegateInfo
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
        public readonly string? MarshalReturnAs;
        public readonly string NamedArguments;
        public readonly string ReturnKeyword;
        public readonly string TypeArgumentCheckWithMarshalInfoCondition;
        public readonly int TypeArgumentCount;
        public readonly string TypeArguments;
        public readonly bool UnmanagedTypeArgumentsOnly;

        private static string GetMarshalAsAttributeString(string? value)
        {
            if (value is null)
            {
                return "null";
            }
            int index = value.IndexOf(',');
            if (index == -1)
            {
                return $"new MarshalAsAttribute({value})";
            }
            string head = value.Substring(0, index);
            string tail = value.Substring(index + 2);
            return $"new MarshalAsAttribute({head}) {{ {tail} }}";
        }

        public NativeGenericDelegateInfo(MethodSymbolWithMarshalInfo methodSymbolWithMarshalInfo, CancellationToken cancellationToken)
        {
            IMethodSymbol methodSymbol = methodSymbolWithMarshalInfo.MethodSymbol;
            INamedTypeSymbol interfaceSymbol = methodSymbol.ContainingType;
            bool isAction = interfaceSymbol.Name.StartsWith(Constants.INativeActionIdentifier);
            string identifier = isAction ? Constants.ActionIdentifier : Constants.FuncIdentifier;
            int typeArgumentCount = interfaceSymbol.Arity - (isAction ? 0 : 1);
            IEnumerable<int> range = Enumerable.Range(0, typeArgumentCount);
            ImmutableArray<string> typeArgumentsWithReturnType = interfaceSymbol.TypeArguments.Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)).ToImmutableArray();
            ImmutableArray<string> marshalParamsAs = range.Select
            (
                x => methodSymbolWithMarshalInfo.MarshalParamsAs is not null && methodSymbolWithMarshalInfo.MarshalParamsAs.Value[x] is not null ?
                    $"[MarshalAs({methodSymbolWithMarshalInfo.MarshalParamsAs.Value[x]})] " : ""
            ).ToImmutableArray();
            ImmutableArray<string> typeParameters = interfaceSymbol.TypeParameters.Select(x => x.ToString()).ToImmutableArray();
            cancellationToken.ThrowIfCancellationRequested();
            ClassNamePrefix = $"Native{identifier}_{Guid.NewGuid():N}";
            FunctionPointerTypeArgumentsWithReturnType = string.Join(", ", typeArgumentsWithReturnType);
            Identifier = identifier;
            IdentifierWithTypeArguments = $"{identifier}<{FunctionPointerTypeArgumentsWithReturnType}>";
            IdentifierWithTypeParameters = $"{identifier}<{string.Join(", ", typeParameters)}>";
            InvokeArguments = string.Join(", ", range.Select(x => $"_{x + 1}"));
            InvokeReturnType = isAction ? "void" : typeArgumentsWithReturnType[typeArgumentCount];
            IsAction = isAction;
            IsFromFunctionPointerGeneric = methodSymbol.IsGenericMethod;
            MarshalReturnAs = methodSymbolWithMarshalInfo.MarshalReturnAs;
            NamedArguments = string.Join(", ", range.Select(x => $"{marshalParamsAs[x]}{typeArgumentsWithReturnType[x]} _{x + 1}"));
            ReturnKeyword = isAction ? "" : "return ";
            TypeArgumentCount = typeArgumentCount;
            TypeArguments = string.Join(", ", typeArgumentsWithReturnType.Take(typeArgumentCount));
            UnmanagedTypeArgumentsOnly = interfaceSymbol.TypeArguments.All(x => x.IsUnmanagedType);
            cancellationToken.ThrowIfCancellationRequested();
            string marshalReturnAsAttribute = GetMarshalAsAttributeString(methodSymbolWithMarshalInfo.MarshalReturnAs);
            string marshalParamsAsAttributes = "null";
            StringBuilder sb = new("new MarshalAsAttribute?[] { ");
            string andNewLine = $@" &&
                ";
            if (methodSymbolWithMarshalInfo.MarshalParamsAs is not null && methodSymbolWithMarshalInfo.MarshalParamsAs.Value.Length > 0)
            {
                for (int i = 0; i < typeArgumentCount; ++i)
                {
                    if (i > 0)
                    {
                        _ = sb.Append(", ");
                    }
                    _ = sb.Append(GetMarshalAsAttributeString(methodSymbolWithMarshalInfo.MarshalParamsAs.Value[i]));
                }
                _ = sb.Append(" }");
                marshalParamsAsAttributes = sb.ToString();
            }
            cancellationToken.ThrowIfCancellationRequested();
            sb.Clear();
            for (int i = 0; i < interfaceSymbol.Arity; ++i)
            {
                _ = sb.Append($"(typeof({typeParameters[i]}) == typeof({typeArgumentsWithReturnType[i]}))");
                if ((i + 1) < interfaceSymbol.Arity)
                {
                    _ = sb.Append(andNewLine);
                }
            }
            if (methodSymbol.IsGenericMethod)
            {
                typeArgumentsWithReturnType = methodSymbol.TypeArguments.Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)).ToImmutableArray();
                FunctionPointerTypeArgumentsWithReturnType = $"{string.Join(", ", typeArgumentsWithReturnType)}{(isAction ? ", void" : "")}";
                _ = sb.Append(andNewLine);
                for (int i = 0; i < methodSymbol.Arity; ++i)
                {
                    _ = sb.Append($"(typeof({methodSymbol.TypeParameters[i]}) == typeof({typeArgumentsWithReturnType[i]}))");
                    if ((i + 1) < methodSymbol.Arity)
                    {
                        _ = sb.Append(andNewLine);
                    }
                }
            }
            else if (isAction)
            {
                FunctionPointerTypeArgumentsWithReturnType += ", void";
            }
            cancellationToken.ThrowIfCancellationRequested();
            _ = sb.Append(andNewLine).Append($"MarshalInfo.Equals({(isAction ? "null" : "marshalReturnAs")}, marshalParamsAs, {marshalReturnAsAttribute}, {marshalParamsAsAttributes})");
            TypeArgumentCheckWithMarshalInfoCondition = sb.ToString();
        }
    }
}
