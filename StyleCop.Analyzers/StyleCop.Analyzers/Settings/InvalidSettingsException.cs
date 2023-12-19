// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers
{
    using System;

    /// <summary>
    /// Exception thrown when an invalid settings have been encountered.
    /// </summary>
    internal class InvalidSettingsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSettingsException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        internal InvalidSettingsException(string message)
            : base(message)
        {
        }
    }
}
