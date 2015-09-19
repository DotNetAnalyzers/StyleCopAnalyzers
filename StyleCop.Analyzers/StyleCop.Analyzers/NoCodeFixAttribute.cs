// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// This attribute is applied to <see cref="DiagnosticAnalyzer"/>s for which no code fix is currently planned.
    /// </summary>
    /// <remarks>
    /// <para>There are several reasons an analyzer does not have a code fix, including but not limited to the
    /// following:</para>
    /// <list type="bullet">
    /// <item>Visual Studio provides a built-in code fix.</item>
    /// <item>A code fix could not provide a useful solution.</item>
    /// </list>
    /// <para>The <see cref="Reason"/> should be provided.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class NoCodeFixAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoCodeFixAttribute"/> class.
        /// </summary>
        /// <param name="reason">The reason why the <see cref="DiagnosticAnalyzer"/> does not have a code fix.</param>
        public NoCodeFixAttribute(string reason)
        {
            this.Reason = reason;
        }

        /// <summary>
        /// Gets the reason why the <see cref="DiagnosticAnalyzer"/> does not have a code fix.
        /// </summary>
        /// <value>
        /// The reason why the <see cref="DiagnosticAnalyzer"/> does not have a code fix.
        /// </value>
        public string Reason
        {
            get;
        }
    }
}
