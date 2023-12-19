// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers
{
    using System;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// This attribute is applied to <see cref="DiagnosticAnalyzer"/>s for which no implementation is currently planned.
    /// </summary>
    /// <remarks>
    /// <para>There are several reasons an analyzer for a StyleCop diagnostic does not have an implementation, including
    /// but not limited to the following:</para>
    /// <list type="bullet">
    /// <item><description>Visual Studio provides a built-in diagnostic.</description></item>
    /// <item><description>The diagnostic is vaguely defined, so there is no clear direction for the implementation.</description></item>
    /// <item><description>The diagnostic has been superseded by a fine-grained set of new rules for customization.</description></item>
    /// </list>
    /// <para>The <see cref="Reason"/> should be provided.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class NoDiagnosticAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoDiagnosticAttribute"/> class.
        /// </summary>
        /// <param name="reason">The reason why the <see cref="DiagnosticAnalyzer"/> does not have an implementation.</param>
        public NoDiagnosticAttribute(string reason)
        {
            this.Reason = reason;
        }

        /// <summary>
        /// Gets the reason why the <see cref="DiagnosticAnalyzer"/> does not have an implementation.
        /// </summary>
        /// <value>
        /// The reason why the <see cref="DiagnosticAnalyzer"/> does not have an implementation.
        /// </value>
        public string Reason
        {
            get;
        }
    }
}
