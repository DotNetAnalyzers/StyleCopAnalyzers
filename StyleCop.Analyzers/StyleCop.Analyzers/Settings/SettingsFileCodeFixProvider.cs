// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Implements a code fix that will generate a StyleCop settings file if it does not exist yet.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SettingsFileCodeFixProvider))]
    [Shared]
    internal class SettingsFileCodeFixProvider : CodeFixProvider
    {
        private const string StyleCopSettingsFileName = "stylecop.json";
        private const string DefaultSettingsFileContent = @"{
  ""$schema"": ""https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json"",
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": """ + DocumentationSettings.DefaultCompanyName + @"""
    }
  }
}
";

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1600ElementsMustBeDocumented.DiagnosticId,
                SA1601PartialElementsMustBeDocumented.DiagnosticId,
                SA1602EnumerationItemsMustBeDocumented.DiagnosticId,
                FileHeaderAnalyzers.SA1633DescriptorMissing.Id,
                FileHeaderAnalyzers.SA1634Descriptor.Id,
                FileHeaderAnalyzers.SA1635Descriptor.Id,
                FileHeaderAnalyzers.SA1636Descriptor.Id,
                FileHeaderAnalyzers.SA1637Descriptor.Id,
                FileHeaderAnalyzers.SA1638Descriptor.Id,
                FileHeaderAnalyzers.SA1639Descriptor.Id,
                FileHeaderAnalyzers.SA1640Descriptor.Id,
                FileHeaderAnalyzers.SA1641Descriptor.Id,
                SA1649FileNameMustMatchTypeName.DiagnosticId);

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var project = context.Document.Project;
            var workspace = project.Solution.Workspace;

            // check if the settings file already exists
            if (project.AdditionalDocuments.Any(IsStyleCopSettingsDocument))
            {
                return SpecializedTasks.CompletedTask;
            }

            // check if we are allowed to add it
            if (!workspace.CanApplyChange(ApplyChangesKind.AddAdditionalDocument))
            {
                return SpecializedTasks.CompletedTask;
            }

            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        SettingsResources.SettingsFileCodeFix,
                        cancellationToken => GetTransformedSolutionAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SettingsFileCodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            // Added this to make it explicitly clear that this code fix does not support fix all actions.
            return null;
        }

        private static bool IsStyleCopSettingsDocument(TextDocument document)
        {
            return string.Equals(document.Name, StyleCopSettingsFileName, StringComparison.OrdinalIgnoreCase);
        }

        private static Task<Solution> GetTransformedSolutionAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var project = document.Project;
            var solution = project.Solution;

            var newDocumentId = DocumentId.CreateNewId(project.Id);

            var newSolution = solution.AddAdditionalDocument(newDocumentId, StyleCopSettingsFileName, DefaultSettingsFileContent);

            return Task.FromResult(newSolution);
        }
    }
}
