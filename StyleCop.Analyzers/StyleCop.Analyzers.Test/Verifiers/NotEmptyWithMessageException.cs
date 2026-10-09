// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System;
    using Xunit.Sdk;

    /// <summary>
    /// A <see cref="NotEmptyException"/> with a user message. Adapted from
    /// <c>Microsoft.CodeAnalysis.Testing.Verifiers.XUnit</c> (MIT license, .NET Foundation).
    /// </summary>
    internal class NotEmptyWithMessageException : NotEmptyException
    {
        public NotEmptyWithMessageException(string userMessage)
        {
            this.UserMessage = userMessage;
        }

        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(this.UserMessage))
                {
                    return base.Message;
                }

                return this.UserMessage + Environment.NewLine + base.Message;
            }
        }
    }
}
