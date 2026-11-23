// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules;

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.Lightup;
using StyleCop.Analyzers.Settings;

/// <summary>
/// Offers to set <c>compatibility.multiTargetsLegacyFrameworks</c> in an existing StyleCop configuration file when
/// <see cref="SA1121CodeFixProvider"/> withholds its fix because another target of the shared source file does not
/// support native-sized integers.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1121MultiTargetsLegacyFrameworksCodeFixProvider))]
[Shared]
internal class SA1121MultiTargetsLegacyFrameworksCodeFixProvider : CodeFixProvider
{
    private const string SettingsKey = "stylecop.compatibility.multiTargetsLegacyFrameworks";

    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(SA1121UseBuiltInTypeAlias.DiagnosticId);

    /// <inheritdoc/>
    public override FixAllProvider? GetFixAllProvider()
    {
        // The fix edits a configuration file rather than the diagnosed document.
        return null;
    }

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Target? target = null;
        var targetComputed = false;

        foreach (var diagnostic in context.Diagnostics)
        {
            if (!await SA1121CodeFixProvider.IsUnsupportedLinkedNativeSizedIntegerAsync(context.Document, diagnostic, context.CancellationToken).ConfigureAwait(false))
            {
                continue;
            }

            if (!targetComputed)
            {
                target = await FindTargetAsync(context.Document, context.CancellationToken).ConfigureAwait(false);
                targetComputed = true;
            }

            if (target == null)
            {
                return;
            }

            var capturedTarget = target;
            context.RegisterCodeFix(
                CodeAction.Create(
                    string.Format(ReadabilityResources.SA1121MultiTargetsLegacyFrameworksCodeFix, capturedTarget.DisplayName),
                    cancellationToken => Task.FromResult(capturedTarget.Apply(context.Document.Project.Solution)),
                    nameof(SA1121MultiTargetsLegacyFrameworksCodeFixProvider)),
                diagnostic);
        }
    }

    private static async Task<Target?> FindTargetAsync(Document document, CancellationToken cancellationToken)
    {
        var project = document.Project;
        var workspace = project.Solution.Workspace;

        // stylecop.json values take precedence over .editorconfig values, so edit it when it exists.
        var settingsFile = project.AdditionalDocuments.FirstOrDefault(additionalDocument => SettingsHelper.IsStyleCopSettingsFile(additionalDocument.Name));
        if (settingsFile != null && workspace.CanApplyChange(ApplyChangesKind.ChangeAdditionalDocument))
        {
            var text = await settingsFile.GetTextAsync(cancellationToken).ConfigureAwait(false);
            var newText = SettingsFileEditor.SetJsonTrue(text.ToString(), "compatibility", "multiTargetsLegacyFrameworks");
            if (newText != null)
            {
                return new Target(settingsFile.Name, settingsFile.Id, text, newText, isAnalyzerConfig: false);
            }
        }

        if (!Enum.TryParse<ApplyChangesKind>("ChangeAnalyzerConfigDocument", out var changeKind)
            || !workspace.CanApplyChange(changeKind))
        {
            return null;
        }

        var configDocuments = await project.GetAnalyzerConfigDocumentsAsync(cancellationToken).ConfigureAwait(false);
        var documentDirectory = GetDirectory(document.FilePath);

        var editorConfig = configDocuments
            .Where(config => string.Equals(config.Name, ".editorconfig", StringComparison.OrdinalIgnoreCase)
                && (documentDirectory == null || IsAncestorOrSame(GetDirectory(config.FilePath), documentDirectory)))
            .OrderByDescending(config => config.Text.ToString().Contains("stylecop."))
            .ThenBy(config => GetDirectory(config.FilePath)?.Length ?? 0)
            .Cast<AnalyzerConfigDocumentInfo?>()
            .FirstOrDefault();
        if (editorConfig != null)
        {
            return CreateConfigTarget(editorConfig.Value, isGlobalConfig: false);
        }

        var globalConfig = configDocuments
            .Where(config => config.Name.EndsWith(".globalconfig", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(config => config.Text.ToString().Contains("stylecop."))
            .Cast<AnalyzerConfigDocumentInfo?>()
            .FirstOrDefault();
        if (globalConfig != null)
        {
            return CreateConfigTarget(globalConfig.Value, isGlobalConfig: true);
        }

        return null;
    }

    private static Target CreateConfigTarget(AnalyzerConfigDocumentInfo config, bool isGlobalConfig)
    {
        var newText = SettingsFileEditor.SetEditorConfigTrue(config.Text.ToString(), SettingsKey, isGlobalConfig);
        return new Target(config.Name, config.Id, config.Text, newText, isAnalyzerConfig: true);
    }

    private static string? GetDirectory(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return null;
        }

        var normalized = filePath!.Replace('\\', '/');
        var index = normalized.LastIndexOf('/');
        return index < 0 ? string.Empty : normalized.Substring(0, index + 1);
    }

    private static bool IsAncestorOrSame(string? configDirectory, string documentDirectory)
        => configDirectory != null && documentDirectory.StartsWith(configDirectory, StringComparison.OrdinalIgnoreCase);

    private sealed class Target
    {
        private readonly DocumentId documentId;
        private readonly SourceText oldText;
        private readonly string newText;
        private readonly bool isAnalyzerConfig;

        public Target(string displayName, DocumentId documentId, SourceText oldText, string newText, bool isAnalyzerConfig)
        {
            this.DisplayName = displayName;
            this.documentId = documentId;
            this.oldText = oldText;
            this.newText = newText;
            this.isAnalyzerConfig = isAnalyzerConfig;
        }

        public string DisplayName { get; }

        public Solution Apply(Solution solution)
        {
            var text = SourceText.From(this.newText, this.oldText.Encoding, this.oldText.ChecksumAlgorithm);
            return this.isAnalyzerConfig
                ? solution.WithAnalyzerConfigDocumentText(this.documentId, text)
                : solution.WithAdditionalDocumentText(this.documentId, text);
        }
    }
}
