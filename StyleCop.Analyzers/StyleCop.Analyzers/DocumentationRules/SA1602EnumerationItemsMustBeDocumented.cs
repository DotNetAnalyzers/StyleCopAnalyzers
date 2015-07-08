namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// An item within a C# enumeration is missing an XML documentation header.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when an item within an enumeration is missing a header. For
    /// example:</para>
    ///
    /// <code>
    /// /// &lt;summary&gt;
    /// /// Types of animals.
    /// /// &lt;/summary&gt;
    /// public enum Animals
    /// {
    ///     Dog,
    ///     Cat,
    ///     Horse
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1602EnumerationItemsMustBeDocumented : DocumentationAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1602EnumerationItemsMustBeDocumented"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1602";

        private const string Title = "Enumeration items must be documented";
        private const string MessageFormat = "Enumeration items must be documented";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "An item within a publicly visible C# enumeration is missing an Xml documentation header.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1602.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

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
            context.RegisterSyntaxNodeActionHonorExclusions(this.HandleEnumMember, SyntaxKind.EnumMemberDeclaration);
        }

        /// <inheritdoc/>
        protected override DiagnosticDescriptor DescriptorFromEffectiveVisibility(SyntaxKind visibility, SyntaxNodeAnalysisContext context)
        {
            switch (visibility)
            {
            case SyntaxKind.PublicKeyword:
                return (context.SemanticModel.Compilation.Options.SpecificDiagnosticOptions.GetValueOrDefault(CS1591DiagnosticId, ReportDiagnostic.Default) != ReportDiagnostic.Suppress)
                    ? null
                    : Descriptor;

            case SyntaxKind.InternalKeyword:
                return (context.SemanticModel.Compilation.Options.SpecificDiagnosticOptions.GetValueOrDefault(SA16X0InternalElementsMustBeDocumented.DiagnosticId, ReportDiagnostic.Default) != ReportDiagnostic.Suppress)
                    ? null
                    : Descriptor;

            case SyntaxKind.PrivateKeyword:
                return Descriptor;

            default:
                throw new ArgumentOutOfRangeException("visibility");
            }
        }
    }
}