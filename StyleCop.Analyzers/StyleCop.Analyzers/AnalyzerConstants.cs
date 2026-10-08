// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    internal static class AnalyzerConstants
    {
        /// <summary>
        /// Gets a value indicating whether the diagnostic is disabled by default because it is an alternative to a standard
        /// StyleCop rule. It is used with
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>.
        /// </summary>
        /// <value>
        /// A value indicating the diagnostic is disabled by default because it is an alternative to a standard  StyleCop rule.
        /// It is used with
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>.
        /// </value>
        internal static bool DisabledAlternative => false;

        /// <summary>
        /// Gets a value indicating whether the diagnostic should be enabled by default. It is used with
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>.
        /// </summary>
        /// <value>
        /// A value indicating whether the diagnostic should be enabled by default. It is used with
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>.
        /// </value>
        internal static bool EnabledByDefault => true;

        /// <summary>
        /// Gets a value indicating whether the diagnostic should be disabled by default. It is used with
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>.
        /// </summary>
        /// <value>
        /// A value indicating whether the diagnostic should be disabled by default. It is used with
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>.
        /// </value>
        internal static bool DisabledByDefault => false;
    }
}
