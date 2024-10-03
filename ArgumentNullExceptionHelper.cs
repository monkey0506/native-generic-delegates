// Portions of this file have been borrowed from <https://github.com/dotnet/runtime> with the following license notice:
// 
//      Licensed to the .NET Foundation under one or more agreements.
//      The .NET Foundation licenses this file to you under the MIT license.
// 
// Only trivial changes have been made to the borrowed source code, such as formatting and modern C# features.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System
{
    internal static class ArgumentNullExceptionHelper
    {
        /// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.</summary>
        /// <param name="argument">The reference type argument to validate as non-null.</param>
        /// <param name="paramName">
        /// The name of the parameter with which <paramref name="argument"/> corresponds.
        /// </param>
        public static void ThrowIfNull
        (
            [NotNull] object? argument,
            [CallerArgumentExpression(nameof(argument))] string? paramName = null
        )
        {
            if (argument is null)
            {
                Throw(paramName);
            }
        }

        [DoesNotReturn]
        private static void Throw(string? paramName) => throw new ArgumentNullException(paramName);
    }
}
