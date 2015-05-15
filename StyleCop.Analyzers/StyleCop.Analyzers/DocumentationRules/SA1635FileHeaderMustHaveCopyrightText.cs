namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The file header at the top of a C# code file is missing copyright text.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the file header at the top of a C# file does not contain text within
    /// its copyright tag. For example:</para>
    ///
    /// <code language="csharp">
    /// //-----------------------------------------------------------------------
    /// // &lt;copyright file="Widget.cs" company="Sprocket Enterprises"&gt;
    /// // &lt;/copyright&gt;
    /// //-----------------------------------------------------------------------
    /// </code>
    ///
    /// <para>A file header should include copyright text, as follows:</para>
    ///
    /// <code language="csharp">
    /// //-----------------------------------------------------------------------
    /// // &lt;copyright file="Widget.cs" company="Sprocket Enterprises"&gt;
    /// //     Copyright (c) Sprocket Enterprises. All rights reserved.
    /// // &lt;/copyright&gt;
    /// //-----------------------------------------------------------------------
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1635FileHeaderMustHaveCopyrightText : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1635FileHeaderMustHaveCopyrightText"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1635";
        private const string Title = "File header must have copyright text";
        private const string MessageFormat = "File header must have copyright text";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "The file header at the top of a C# code file is missing copyright text.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1635.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => SupportedDiagnosticsValue;

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(HandleSyntaxTreeAxtion);
        }

        private static void HandleSyntaxTreeAxtion(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);

            var fileHeader = FileHeaderHelpers.ParseFileHeader(root);
            if (fileHeader.IsMissing || fileHeader.IsMalformed)
            {
                // this will be handled by SA1633
                return;
            }

            var copyrightElement = fileHeader.GetElement("copyright");
            if (copyrightElement == null)
            {
                // this will be handled by SA1634
                return;
            }

            if (string.IsNullOrWhiteSpace(copyrightElement.Value))
            {
                var location = fileHeader.GetElementLocation(copyrightElement);
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
            }
        }
    }
}
