// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.MaintainabilityRules;
    using TestHelper;

    public abstract class SA1402ForNonBlockDeclarationUnitTestsBase : CodeFixVerifier
    {
        public abstract string Keyword { get; }

        protected SA1402SettingsConfiguration SettingsConfiguration { get; set; } = SA1402SettingsConfiguration.ConfigureAsTopLevelType;

        protected override string GetSettings()
        {
            return this.SettingsConfiguration.GetSettings(this.Keyword);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1402FileMayOnlyContainASingleType();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1402CodeFixProvider();
        }
    }
}
