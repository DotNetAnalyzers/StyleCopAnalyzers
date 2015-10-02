// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    internal static class AnalyzerConstants
    {
        static AnalyzerConstants()
        {
#if DEBUG
            // In DEBUG builds, the tests are enabled to simplify development and testing.
            DisabledNoTests = true;
#else
            DisabledNoTests = false;
#endif
        }

        /// <summary>
        /// Gets a reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to disable a diagnostic which is currently untested.
        /// </summary>
        /// <value>
        /// A reference value which can be passed to
        /// <see cref="DiagnosticDescriptor(string, string, string, string, DiagnosticSeverity, bool, string, string, string[])"/>
        /// to disable a diagnostic which is currently untested.
        /// </value>
        [ExcludeFromCodeCoverage]
        internal static bool DisabledNoTests { get; }

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
        internal static bool DisabledByDefault => false;
    }
}
