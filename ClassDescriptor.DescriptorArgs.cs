using System.Collections.Generic;
using System;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly partial struct ClassDescriptor
    {
        internal readonly ref struct DescriptorArgs
        {
            public readonly ArgumentInfo ArgumentInfo;
            public readonly string ClassName;
            public readonly string FirstArgument;
            public readonly string FirstParameter;
            public readonly InterfaceDescriptor Interface;
            public readonly MethodDescriptor Method;
            public readonly IReadOnlyList<MethodReference> References;

            public DescriptorArgs
            (
                in InterfaceDescriptor interfaceDescriptor,
                in MethodDescriptor methodDescriptor,
                in ArgumentInfo argumentInfo,
                IReadOnlyList<MethodReference> references
            )
            {
                string identifier = interfaceDescriptor.IsAction ? "Action" : "Func";
                ArgumentInfo = argumentInfo;
                ClassName = $"Native{identifier}_{Guid.NewGuid():N}";
                Interface = interfaceDescriptor;
                Method = methodDescriptor;
                References = references;
                switch (methodDescriptor.IsFromFunctionPointer)
                {
                    case true:
                        FirstArgument = "functionPtr";
                        FirstParameter = "nint functionPtr";
                        break;
                    default:
                        FirstArgument = identifier.ToLower();
                        FirstParameter = $"{identifier}{Interface.TypeArgumentList} {FirstArgument}";
                        break;
                }
            }
        }
    }
}
