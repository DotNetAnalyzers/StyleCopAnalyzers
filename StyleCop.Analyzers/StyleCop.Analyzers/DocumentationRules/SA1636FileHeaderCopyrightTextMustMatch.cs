﻿namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The file header at the top of a C# code file does not contain the appropriate copyright text.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the file header at the top of a C# file does not contain the
    /// copyright text that has been specified for the project. To enable this rule, navigate to the StyleCop settings
    /// for the project and change to the Company Information tab, as shown below:</para>
    ///
    /// <para><img src="http://www.stylecop.com/docs/Images/CompanyInformationSettings.JPG"/></para>
    ///
    /// <para>Check the box at the top of the settings page, and fill in the required copyright text for your company.
    /// Click OK to save the settings. With these settings in place, every file within the project must contain the
    /// required copyright text within its file header copyright tag, as shown in the example below:</para>
    ///
    /// <code language="csharp">
    /// //-----------------------------------------------------------------------
    /// // &lt;copyright file="Widget.cs" company="My Company"&gt;
    /// //     Custom company copyright tag.
    /// // &lt;/copyright&gt;
    /// //-----------------------------------------------------------------------
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1636FileHeaderCopyrightTextMustMatch : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1636FileHeaderCopyrightTextMustMatch"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1636";
        private const string Title = "File header copyright text must match";
        private const string MessageFormat = "TODO: Message format";
        private const string Description = "The file header at the top of a C# code file does not contain the appropriate copyright text.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1636.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
