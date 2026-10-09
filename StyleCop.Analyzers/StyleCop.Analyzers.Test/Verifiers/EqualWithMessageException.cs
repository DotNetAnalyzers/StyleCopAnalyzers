// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System;
    using Xunit.Sdk;

    /// <summary>
    /// An <see cref="EqualException"/> with a user message. Adapted from
    /// <c>Microsoft.CodeAnalysis.Testing.Verifiers.XUnit</c> (MIT license, .NET Foundation).
    /// </summary>
    internal class EqualWithMessageException : EqualException
    {
        public EqualWithMessageException(object expected, object actual, string userMessage)
            : base(expected, actual)
        {
            this.UserMessage = userMessage;
        }

        public EqualWithMessageException(string expected, string actual, int expectedIndex, int actualIndex, string userMessage)
            : base(expected, actual, expectedIndex, actualIndex)
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
