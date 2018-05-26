// Copyright (c) Dennis Fischer. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Status.Generator
{
    using System;

    /// <summary>
    /// An exception that gets thrown if the compilation failed.
    /// </summary>
    [Serializable]
    public class CompilationFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationFailedException"/> class.
        /// </summary>
        public CompilationFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message that should be reported</param>
        public CompilationFailedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message that should be reported</param>
        /// <param name="inner">The exception that caused this exception to be thrown</param>
        public CompilationFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
