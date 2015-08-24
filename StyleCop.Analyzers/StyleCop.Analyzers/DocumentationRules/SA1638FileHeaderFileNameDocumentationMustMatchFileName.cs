namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// The file tag within the file header at the top of a C# code file does not contain the name of the file.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the file tag within the file header at the top of a C# file does not
    /// contain the name of the file. For example, consider a C# source file named File1.cs, with the following
    /// header:</para>
    ///
    /// <code language="csharp">
    /// //-----------------------------------------------------------------------
    /// // &lt;copyright file="File2.cs" company="My Company"&gt;
    /// //     Custom company copyright tag.
    /// // &lt;/copyright&gt;
    /// //-----------------------------------------------------------------------
    /// </code>
    ///
    /// <para>A violation of this rule would occur, since the file tag does not contain the correct name of the file.
    /// The header should be written as:</para>
    ///
    /// <code language="csharp">
    /// //-----------------------------------------------------------------------
    /// // &lt;copyright file="File1.cs" company="My Company"&gt;
    /// //     Custom company copyright tag.
    /// // &lt;/copyright&gt;
    /// //-----------------------------------------------------------------------
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1638FileHeaderFileNameDocumentationMustMatchFileName : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1638FileHeaderFileNameDocumentationMustMatchFileName"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1638";
        private const string Title = "File header file name documentation must match file name";
        private const string MessageFormat = "File header file name documentation must match file name";
        private const string Description = "The file attribute within copyright tag of the file header at the top of a C# code file does not contain the name of the file.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1638.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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

            var fileAttribute = copyrightElement.Attribute("file");
            if (fileAttribute == null)
            {
                // this will be handled by SA1637
                return;
            }

            var fileName = Path.GetFileName(context.Tree.FilePath);

            if (!fileAttribute.Value.Equals(fileName, StringComparison.Ordinal))
            {
                var location = fileHeader.GetElementLocation(context.Tree, copyrightElement);
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
            }
        }
    }
}
