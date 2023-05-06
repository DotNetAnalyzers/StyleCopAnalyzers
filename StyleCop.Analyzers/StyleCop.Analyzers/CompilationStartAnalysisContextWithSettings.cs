// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Settings.ObjectModel;

    internal class CompilationStartAnalysisContextWithSettings
    {
        [SuppressMessage("MicrosoftCodeAnalysisPerformance", "RS1012:Start action has no registered actions", Justification = "This is not a start action")]
        public CompilationStartAnalysisContextWithSettings(
            CompilationStartAnalysisContext innerContext,
            SettingsHelper.SettingsFile settingsFile,
            ConcurrentDictionary<SyntaxTree, Lazy<StyleCopSettings>> settingsObjectCache)
        {
            this.InnerContext = innerContext;
            this.SettingsFile = settingsFile;
            this.SettingsObjectCache = settingsObjectCache;
        }

        public CompilationStartAnalysisContext InnerContext { get; }

        public SettingsHelper.SettingsFile SettingsFile { get; }

        public ConcurrentDictionary<SyntaxTree, Lazy<StyleCopSettings>> SettingsObjectCache { get; }

        /// <summary>
        /// Gets the Microsoft.CodeAnalysis.Compilation that is the subject of the analysis.
        /// </summary>
        /// <value>
        /// The Microsoft.CodeAnalysis.Compilation that is the subject of the analysis.
        /// </value>
        public Compilation Compilation => this.InnerContext.Compilation;
    }
}
