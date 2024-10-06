﻿using System.Text;

namespace Monkeymoto.NativeGenericDelegates
{
    internal static class PostInitialization
    {
        private static void BuildInterfaceDefinition(StringBuilder sb, bool isAction, int argumentCount)
        {
            string? marshalReturnAsParameter;
            string? qualifiedTypeParameters;
            string? returnType;
            string? type;
            string? typeParameters;
            if (isAction)
            {
                marshalReturnAsParameter = string.Empty;
                returnType = "void";
                type = Constants.CategoryAction;
                if (argumentCount != 0)
                {
                    qualifiedTypeParameters = $"<{Constants.Actions.QualifiedTypeParameters[argumentCount]}>";
                    typeParameters = $"<{Constants.Actions.TypeParameters[argumentCount]}>";
                }
                else
                {
                    qualifiedTypeParameters = string.Empty;
                    typeParameters = string.Empty;
                }
            }
            else
            {
                marshalReturnAsParameter = $",{Constants.NewLineIndent3}MarshalAsAttribute? marshalReturnAs = null";
                qualifiedTypeParameters = $"<{Constants.Funcs.QualifiedTypeParameters[argumentCount]}>";
                returnType = "TResult";
                type = Constants.CategoryFunc;
                typeParameters = $"<{Constants.Funcs.TypeParameters[argumentCount]}>";
            }
            string genericType = $"{type}{typeParameters}";
            string parameters = Constants.Parameters[argumentCount];
            string typeAsArgument = type.ToLower();
            string callingConvention =
                $",{Constants.NewLineIndent3}CallingConvention callingConvention = CallingConvention.Winapi";
            string marshalParamsAsParameter = argumentCount != 0 ?
                $",{Constants.NewLineIndent3}MarshalAsAttribute?[]? marshalParamsAs = null" :
                string.Empty;
            string marshalMap = argumentCount != 0 ?
                $",{Constants.NewLineIndent3}MarshalMap? marshalMap = null" :
                string.Empty;
            _ = sb.Append
            (
$@"    internal interface INative{type}{qualifiedTypeParameters}
    {{
        protected object? Target {{ get; }}
        protected MethodInfo Method {{ get; }}

        public static INative{genericType} From{type}
        (
            {genericType} {typeAsArgument}{callingConvention}{marshalMap}{marshalReturnAsParameter}{marshalParamsAsParameter}
        )
        {{
            throw new NotImplementedException();
        }}

        public static INative{genericType} FromFunctionPointer
        (
            nint functionPtr{callingConvention}{marshalMap}{marshalReturnAsParameter}{marshalParamsAsParameter}
        )
        {{
            throw new NotImplementedException();
        }}

        public nint GetFunctionPointer();
        public {returnType} Invoke({parameters});
        public {genericType} To{type}() => ({genericType})Delegate.CreateDelegate(typeof({genericType}), Target, Method);
    }}
"
            );
        }

        public static string GetSourceText()
        {
            var source = new StringBuilder
            (
$@"// <auto-generated/>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable

namespace {Constants.RootNamespace}
{{"
            );
            for (int i = 0; i < 17; ++i)
            {
                BuildInterfaceDefinition(source.AppendLine(), isAction: true, argumentCount: i);
                BuildInterfaceDefinition(source.AppendLine(), isAction: false, argumentCount: i);
            }
            _ = source.AppendLine
            (
$@"
    internal sealed class MarshalMap : IEnumerable<KeyValuePair<Type, MarshalAsAttribute>>
    {{
        public MarshalMap() {{ }}
        public void Add(Type key, MarshalAsAttribute value) {{ }}
        IEnumerator<KeyValuePair<Type, MarshalAsAttribute>>
            IEnumerable<KeyValuePair<Type, MarshalAsAttribute>>.GetEnumerator() =>
                throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }}
}}

namespace System.Runtime.CompilerServices
{{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal sealed class InterceptsLocationAttribute(string filePath, int line, int character) : Attribute
    {{
        public string FilePath {{ get; }} = filePath;
        public int Line {{ get; }} = line;
        public int Character {{ get; }} = character;
    }}
}}

#nullable restore"
            );
            return source.ToString();
        }
    }
}
