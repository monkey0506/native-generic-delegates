using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace NativeGenericDelegatesGenerator
{
    internal readonly struct PartialImplementations
    {
        public readonly string ConcreteClassDefinitions;
        public readonly string InterfaceImplementations;

        public static string BuildPartialInterfaceDeclaration(bool isAction, int argumentCount)
        {
            string constraints = Constants.UnmanagedGenericActionTypeConstraints[argumentCount];
            string identifier = Constants.ActionIdentifier;
            string qualifiedTypeParameters = Constants.QualifiedGenericActionTypeParameters[argumentCount];
            string returnType = "void";
            string typeParameters = Constants.GenericActionTypeParameters[argumentCount];
            if (!isAction)
            {
                constraints = Constants.UnmanagedGenericFuncTypeConstraints[argumentCount];
                identifier = Constants.FuncIdentifier;
                qualifiedTypeParameters = Constants.QualifiedGenericFuncTypeParameters[argumentCount];
                returnType = "TResult";
                typeParameters = Constants.GenericFuncTypeParameters[argumentCount];
            }
            string genericIdentifier = $"{identifier}<{typeParameters}>";
            string namedArguments = Constants.NamedGenericTypeArguments[argumentCount];
            string unmanagedTypeParameters = typeParameters.Replace("T", "U");
            string marshalReturnAs = isAction ? "" : $", MarshalAsAttribute? marshalReturnAs = null";
            string marshalReturnAsRequired = isAction ? "" : $", MarshalAsAttribute marshalReturnAs";
            return $@"    public partial interface INative{identifier}<{qualifiedTypeParameters}>
    {{
        public static partial INative{genericIdentifier} From{identifier}({genericIdentifier} {identifier.ToLower()}{marshalReturnAs}, MarshalAsAttribute?[]? marshalParamsAs = null, CallingConvention callingConvention = CallingConvention.Winapi);
        public static partial INative{genericIdentifier} FromFunctionPointer(nint functionPtr{marshalReturnAs}, MarshalAsAttribute?[]? marshalParamsAs = null, CallingConvention callingConvention = CallingConvention.Winapi);
        public static partial INative{genericIdentifier} FromFunctionPointer<{unmanagedTypeParameters}>(nint functionPtr{marshalReturnAsRequired}, MarshalAsAttribute[] marshalParamsAs, CallingConvention callingConvention = CallingConvention.Winapi){constraints};

        public nint GetFunctionPointer();
        public {returnType} Invoke({namedArguments});
        public {genericIdentifier} To{identifier}();
    }}
";
        }

        public static string BuildPartialInterfaceImplementation(bool isAction, int argumentCount, StringBuilder fromDelegate, StringBuilder fromFunctionPointer, StringBuilder fromFunctionPointerGeneric)
        {
            string constraints = Constants.UnmanagedGenericActionTypeConstraints[argumentCount];
            string identifier = Constants.ActionIdentifier;
            string qualifiedTypeParameters = Constants.QualifiedGenericActionTypeParameters[argumentCount];
            string typeParameters = Constants.GenericActionTypeParameters[argumentCount];
            if (!isAction)
            {
                constraints = Constants.UnmanagedGenericFuncTypeConstraints[argumentCount];
                identifier = Constants.FuncIdentifier;
                qualifiedTypeParameters = Constants.QualifiedGenericFuncTypeParameters[argumentCount];
                typeParameters = Constants.GenericFuncTypeParameters[argumentCount];
            }
            string genericIdentifier = $"{identifier}<{typeParameters}>";
            string unmanagedTypeParameters = typeParameters.Replace('T', 'U');
            string marshalReturnAs = isAction ? "" : $", MarshalAsAttribute? marshalReturnAs";
            string marshalReturnAsRequired = isAction ? "" : $", MarshalAsAttribute marshalReturnAs";
            return $@"    public partial interface INative{identifier}<{qualifiedTypeParameters}>
    {{
        public static partial INative{genericIdentifier} From{identifier}({genericIdentifier} _delegate{marshalReturnAs}, MarshalAsAttribute?[]? marshalParamsAs, CallingConvention callingConvention)
        {{
            {fromDelegate}
        }}

        public static partial INative{genericIdentifier} FromFunctionPointer(nint functionPtr{marshalReturnAs}, MarshalAsAttribute?[]? marshalParamsAs, CallingConvention callingConvention)
        {{
            {fromFunctionPointer}
        }}

        public static partial INative{genericIdentifier} FromFunctionPointer<{unmanagedTypeParameters}>(nint functionPtr{marshalReturnAsRequired}, MarshalAsAttribute[] marshalParamsAs, CallingConvention callingConvention){constraints}
        {{
            {fromFunctionPointerGeneric}
        }}
    }}
";
        }

        private static void BuildTypeCheck(ref StringBuilder sb, string typeCheck)
        {
            sb ??= new StringBuilder($@"if (callingConvention == CallingConvention.Winapi)
            {{
                callingConvention = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? CallingConvention.StdCall : CallingConvention.Cdecl;
            }}").AppendLine();
            _ = sb.Append(typeCheck);
        }

        public PartialImplementations(SourceProductionContext context, ImmutableArray<NativeGenericDelegateConcreteClassInfo> infos)
        {
            StringBuilder classes = new();
            StringBuilder[] fromAction = new StringBuilder[16];
            StringBuilder[] fromFunc = new StringBuilder[17];
            StringBuilder[] fromFunctionPointerAction = new StringBuilder[16];
            StringBuilder[] fromFunctionPointerFunc = new StringBuilder[17];
            StringBuilder[] fromFunctionPointerActionGeneric = new StringBuilder[16];
            StringBuilder[] fromFunctionPointerFuncGeneric = new StringBuilder[17];
            StringBuilder notImplementedType = new("throw new NotImplementedException();");
            string notImplementedFallthrough = $"            {notImplementedType}";
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
            for (int i = 0; i < 16; ++i)
            {
                fromAction[i] = fromAction[i]?.Append(notImplementedFallthrough) ?? notImplementedType;
                fromFunc[i] = fromFunc[i]?.Append(notImplementedFallthrough) ?? notImplementedType;
                fromFunctionPointerAction[i] = fromFunctionPointerAction[i]?.Append(notImplementedFallthrough) ?? notImplementedType;
                fromFunctionPointerFunc[i] = fromFunctionPointerFunc[i]?.Append(notImplementedFallthrough) ?? notImplementedType;
                fromFunctionPointerActionGeneric[i] = fromFunctionPointerActionGeneric[i]?.Append(notImplementedFallthrough) ?? notImplementedType;
                fromFunctionPointerFuncGeneric[i] = fromFunctionPointerFuncGeneric[i]?.Append(notImplementedFallthrough) ?? notImplementedType;
            }
            fromFunc[16] = fromFunc[16]?.Append(notImplementedFallthrough) ?? notImplementedType;
            fromFunctionPointerFunc[16] = fromFunctionPointerFunc[16]?.Append(notImplementedFallthrough) ?? notImplementedType;
            fromFunctionPointerFuncGeneric[16] = fromFunctionPointerFuncGeneric[16]?.Append(notImplementedFallthrough) ?? notImplementedType;
            StringBuilder partialImplementations = new();
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
            ConcreteClassDefinitions = classes.ToString();
            InterfaceImplementations = partialImplementations.ToString();
        }


        public string GetSource()
        {
            return $@"// <auto-generated/>
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS8826 // Partial method declarations have different signatures (erroneous)
#nullable enable

namespace {Constants.RootNamespace}
{{
    file static class MarshalInfo
    {{
        internal static bool Equals(MarshalAsAttribute? marshalReturnAsLeft, MarshalAsAttribute?[]? marshalParamsAsLeft, MarshalAsAttribute? marshalReturnAsRight, MarshalAsAttribute?[]? marshalParamsAsRight)
        {{
            if (!Equals(marshalReturnAsLeft, marshalReturnAsRight))
            {{
                return false;
            }}
            if (marshalParamsAsLeft is null)
            {{
                return marshalParamsAsRight is null;
            }}
            else if (marshalParamsAsRight is null)
            {{
                return false;
            }}
            if (marshalParamsAsLeft.Length != marshalParamsAsRight.Length)
            {{
                return false;
            }}
            for (int i = 0; i < marshalParamsAsLeft.Length; ++i)
            {{
                if (!Equals(marshalParamsAsLeft[i], marshalParamsAsRight[i]))
                {{
                    return false;
                }}
            }}
            return true;
        }}

        private static bool Equals(MarshalAsAttribute? left, MarshalAsAttribute? right)
        {{
            if (left is null)
            {{
                return right is null;
            }}
            if (right is null)
            {{
                return false;
            }}
            return
                left.Value == right.Value &&
                left.SafeArraySubType == right.SafeArraySubType &&
                left.SafeArrayUserDefinedSubType == right.SafeArrayUserDefinedSubType &&
                left.IidParameterIndex == right.IidParameterIndex &&
                left.ArraySubType == right.ArraySubType &&
                left.SizeParamIndex == right.SizeParamIndex &&
                left.SizeConst == right.SizeConst &&
                left.MarshalType == right.MarshalType &&
                left.MarshalTypeRef == right.MarshalTypeRef &&
                left.MarshalCookie == right.MarshalCookie;
        }}
    }}
{ConcreteClassDefinitions}{InterfaceImplementations}}}

#nullable restore
#pragma warning restore CS8826 // Partial method declarations have different signatures (erroneous)
";
        }
    }
}
