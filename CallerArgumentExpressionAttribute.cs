// This file has been borrowed from <https://github.com/dotnet/runtime> with the following license notice:
// 
//      Licensed to the .NET Foundation under one or more agreements.
//      The .NET Foundation licenses this file to you under the MIT license.
// 
// Only trivial changes have been made to this source file, such as formatting and modern C# features.

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute
    {
        public string ParameterName { get; } = parameterName;
    }
}
