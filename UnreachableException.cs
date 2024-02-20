// This file has been borrowed from <https://github.com/dotnet/runtime> with the following license notice:
// 
//      Licensed to the .NET Foundation under one or more agreements.
//      The .NET Foundation licenses this file to you under the MIT license.
// 
// Only trivial changes have been made to this source file, such as formatting and modern C# features.

namespace System.Diagnostics
{
    /// <summary>
    /// Exception thrown when the program executes an instruction that was thought to be unreachable.
    /// </summary>
    internal sealed class UnreachableException : Exception
    {
        private const string Arg_UnreachableException =
            "The program executed an instruction that was thought to be unreachable.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UnreachableException"/> class with the default error message.
        /// </summary>
        public UnreachableException() : base(Arg_UnreachableException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnreachableException"/>
        /// class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UnreachableException(string? message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnreachableException"/>
        /// class with a specified error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public UnreachableException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
