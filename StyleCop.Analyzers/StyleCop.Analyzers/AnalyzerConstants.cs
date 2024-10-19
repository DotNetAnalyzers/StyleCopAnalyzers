﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    internal static class AnalyzerConstants
    {
        /// <summary>
        /// Gets a reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to indicate that the diagnostic is disabled by default because it is an alternative to a reference StyleCop
        /// rule.
        /// </summary>
        /// <value>
        /// A reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to indicate that the diagnostic is disabled by default because it is an alternative to a reference StyleCop
        /// rule.
        /// </value>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors.", Justification = "This property behaves more like an opaque value than a Boolean.")]
        internal static bool DisabledAlternative => false;

        /// <summary>
        /// Gets a reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to indicate that the diagnostic should be enabled by default.
        /// </summary>
        /// <value>
        /// A reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to indicate that the diagnostic should be enabled by default.
        /// </value>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors.", Justification = "This property behaves more like an opaque value than a Boolean.")]
        internal static bool EnabledByDefault => true;

        /// <summary>
        /// Gets a reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to indicate that the diagnostic should be disabled by default.
        /// </summary>
        /// <value>
        /// A reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to indicate that the diagnostic should be disabled by default.
        /// </value>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors.", Justification = "This property behaves more like an opaque value than a Boolean.")]
        internal static bool DisabledByDefault => false;
    }
}
