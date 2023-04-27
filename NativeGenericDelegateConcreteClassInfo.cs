// NativeGenericDelegates
// https://github.com/monkey0506/native-generic-delegates
// Copyright(C) 2022-2023 Michael Rittenhouse monkey0506@gmail.com

// This C# project and all associated project files are hereby committed to the
// public domain pursuant to the WTFPL http://www.wtfpl.net/about/ without
// warranty of any kind, express or implied, including but not limited to
// fitness for a particular purpose. Attribution is appreciated, but not
// required.

using System.Text;

namespace NativeGenericDelegatesGenerator
{
    internal readonly struct NativeGenericDelegateConcreteClassInfo
    {
        public readonly int ArgumentCount;
        public readonly string ClassDefinitions;
        public readonly string FromDelegateTypeCheck;
        public readonly string FromFunctionPointerTypeCheck;
        public readonly string FromFunctionPointerGenericTypeCheck;
        public readonly bool IsAction;
        public readonly RuntimeMarshalAsAttributeArrayCollection MarshalAsArrayCollection;

        private static void BuildClassDefinition(StringBuilder sb, in NativeGenericDelegateInfo info, string callConv)
        {
            string unmanagedCallConv = Constants.GetUnmanagedCallConv(callConv);
            string funcPtr = $"delegate* unmanaged[{unmanagedCallConv}]<{info.FunctionPointerTypeArgumentsWithReturnType}>";
            string call = info.UnmanagedTypeArgumentsOnly ? $"{info.ReturnKeyword}(({funcPtr})_functionPtr)({info.InvokeArguments});" : $"{info.ReturnKeyword}_delegate({info.InvokeArguments});";
            string getDelegate = info.UnmanagedTypeArgumentsOnly ? "Invoke" : $"Marshal.GetDelegateForFunctionPointer<NonGeneric{info.Identifier}>(functionPtr)";
            string returnMarshal = info.MarshalReturnAs is not null ? $@"
        [return: MarshalAs({info.MarshalReturnAs})]" : "";
            _ = sb.AppendLine($@"
    file unsafe class {info.ClassNamePrefix}_{callConv} : INative{info.IdentifierWithTypeArguments}
    {{
        private readonly NonGeneric{info.Identifier} _delegate;
        private readonly {funcPtr} _functionPtr;

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]{returnMarshal}
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
        [UnmanagedCallConv(CallConvs = new[] {{ typeof(CallConv{unmanagedCallConv}) }})]{returnMarshal}
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
            BuildClassDefinition(sb, in info, Constants.CallConvCdecl);
            BuildClassDefinition(sb, in info, Constants.CallConvStdCall);
            BuildClassDefinition(sb, in info, Constants.CallConvThisCall);
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
            _ = sb.AppendLine($@"            if ({info.TypeArgumentCheckWithMarshalInfoCondition})
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
            ArgumentCount = info.TypeArgumentCount;
            ClassDefinitions = BuildClassDefinitions(in info);
            (FromDelegateTypeCheck, FromFunctionPointerTypeCheck, FromFunctionPointerGenericTypeCheck) = BuildTypeChecks(in info);
            IsAction = info.IsAction;
            MarshalAsArrayCollection = info.MarshalAsArrayCollection;
        }
    }
}
