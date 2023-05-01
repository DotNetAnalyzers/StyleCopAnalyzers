// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class CompilationStartAnalysisContextWithSettings
    {
        [SuppressMessage("MicrosoftCodeAnalysisPerformance", "RS1012:Start action has no registered actions", Justification = "This is not a start action")]
        public CompilationStartAnalysisContextWithSettings(
            CompilationStartAnalysisContext innerContext,
            SettingsHelper.SettingsFile settingsFile)
        {
            this.InnerContext = innerContext;
            this.SettingsFile = settingsFile;
        }

        public CompilationStartAnalysisContext InnerContext { get; }

        public SettingsHelper.SettingsFile SettingsFile { get; }

        /// <summary>
        /// Gets the Microsoft.CodeAnalysis.Compilation that is the subject of the analysis.
        /// </summary>
        /// <value>
        /// The Microsoft.CodeAnalysis.Compilation that is the subject of the analysis.
        /// </value>
        public Compilation Compilation => this.InnerContext.Compilation;
    }
}
