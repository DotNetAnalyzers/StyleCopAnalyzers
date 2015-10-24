// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Analyzer for all file header related diagnostics.
    /// </summary>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1633.md">SA1633 File must have header</seealso>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1634.md">SA1634 File header must show copyright</seealso>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1635.md">SA1635 File header must have copyright text</seealso>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1636.md">SA1636 File header copyright text must match</seealso>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1637.md">SA1637 File header must contain file name</seealso>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1638.md">SA1638 File header file name documentation must match file name</seealso>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1639.md">SA1639 File header must have summary</seealso>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1640.md">SA1640 File header must have valid company text</seealso>
    /// <seealso href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1641.md">SA1641 File header company name text must match</seealso>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class FileHeaderAnalyzers : DiagnosticAnalyzer
    {
        private const string SA1633Identifier = "SA1633";
        private const string SA1634Identifier = "SA1634";
        private const string SA1635Identifier = "SA1635";
        private const string SA1636Identifier = "SA1636";
        private const string SA1637Identifier = "SA1637";
        private const string SA1638Identifier = "SA1638";
        private const string SA1639Identifier = "SA1639";
        private const string SA1640Identifier = "SA1640";
        private const string SA1641Identifier = "SA1641";

        private static readonly LocalizableString SA1633Title = new LocalizableResourceString(nameof(DocumentationResources.SA1633Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1633MessageFormatMissing = new LocalizableResourceString(nameof(DocumentationResources.SA1633MessageFormatMissing), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1633MessageFormatMalformed = new LocalizableResourceString(nameof(DocumentationResources.SA1633MessageFormatMalformed), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1633Description = new LocalizableResourceString(nameof(DocumentationResources.SA1633Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1633HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1633.md";

        private static readonly LocalizableString SA1634Title = new LocalizableResourceString(nameof(DocumentationResources.SA1634Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1634MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1634MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1634Description = new LocalizableResourceString(nameof(DocumentationResources.SA1634Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1634HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1634.md";

        private static readonly LocalizableString SA1635Title = new LocalizableResourceString(nameof(DocumentationResources.SA1635Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1635MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1635MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1635Description = new LocalizableResourceString(nameof(DocumentationResources.SA1635Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1635HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1635.md";

        private static readonly LocalizableString SA1636Title = new LocalizableResourceString(nameof(DocumentationResources.SA1636Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1636MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1636MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1636Description = new LocalizableResourceString(nameof(DocumentationResources.SA1636Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1636HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1636.md";

        private static readonly LocalizableString SA1637Title = new LocalizableResourceString(nameof(DocumentationResources.SA1637Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1637MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1637MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1637Description = new LocalizableResourceString(nameof(DocumentationResources.SA1637Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1637HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1637.md";

        private static readonly LocalizableString SA1638Title = new LocalizableResourceString(nameof(DocumentationResources.SA1638Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1638MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1638MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1638Description = new LocalizableResourceString(nameof(DocumentationResources.SA1638Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1638HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1638.md";

        private static readonly LocalizableString SA1639Title = new LocalizableResourceString(nameof(DocumentationResources.SA1639Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1639MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1639MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1639Description = new LocalizableResourceString(nameof(DocumentationResources.SA1639Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1639HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1639.md";

        private static readonly LocalizableString SA1640Title = new LocalizableResourceString(nameof(DocumentationResources.SA1640Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1640MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1640MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1640Description = new LocalizableResourceString(nameof(DocumentationResources.SA1640Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1640HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1640.md";

        private static readonly LocalizableString SA1641Title = new LocalizableResourceString(nameof(DocumentationResources.SA1641Title), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1641MessageFormat = new LocalizableResourceString(nameof(DocumentationResources.SA1641MessageFormat), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly LocalizableString SA1641Description = new LocalizableResourceString(nameof(DocumentationResources.SA1641Description), DocumentationResources.ResourceManager, typeof(DocumentationResources));
        private static readonly string SA1641HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1641.md";

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;

        /// <summary>
        /// Gets the diagnostic descriptor for SA1633 with a missing header.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1633.</value>
        public static DiagnosticDescriptor SA1633DescriptorMissing { get; } =
            new DiagnosticDescriptor(SA1633Identifier, SA1633Title, SA1633MessageFormatMissing, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1633Description, SA1633HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for SA1633 with a malformed header.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1633.</value>
        public static DiagnosticDescriptor SA1633DescriptorMalformed { get; } =
            new DiagnosticDescriptor(SA1633Identifier, SA1633Title, SA1633MessageFormatMalformed, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1633Description, SA1633HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for SA1634.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1634.</value>
        public static DiagnosticDescriptor SA1634Descriptor { get; } =
            new DiagnosticDescriptor(SA1634Identifier, SA1634Title, SA1634MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1634Description, SA1634HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for SA1635.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1635.</value>
        public static DiagnosticDescriptor SA1635Descriptor { get; } =
            new DiagnosticDescriptor(SA1635Identifier, SA1635Title, SA1635MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1635Description, SA1635HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for SA1636.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1636.</value>
        public static DiagnosticDescriptor SA1636Descriptor { get; } =
            new DiagnosticDescriptor(SA1636Identifier, SA1636Title, SA1636MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1636Description, SA1636HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for SA1637.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1637.</value>
        public static DiagnosticDescriptor SA1637Descriptor { get; } =
            new DiagnosticDescriptor(SA1637Identifier, SA1637Title, SA1637MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1637Description, SA1637HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for SA1638.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1638.</value>
        public static DiagnosticDescriptor SA1638Descriptor { get; } =
            new DiagnosticDescriptor(SA1638Identifier, SA1638Title, SA1638MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1638Description, SA1638HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for SA1639.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1639.</value>
        public static DiagnosticDescriptor SA1639Descriptor { get; } =
            new DiagnosticDescriptor(SA1639Identifier, SA1639Title, SA1639MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, SA1639Description, SA1639HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for SA1640.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1640.</value>
        public static DiagnosticDescriptor SA1640Descriptor { get; } =
            new DiagnosticDescriptor(SA1640Identifier, SA1640Title, SA1640MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1640Description, SA1640HelpLink);

        /// <summary>
        /// Gets the diagnostic descriptor for SA1641.
        /// </summary>
        /// <value>The <see cref="DiagnosticDescriptor"/> for SA1641.</value>
        public static DiagnosticDescriptor SA1641Descriptor { get; } =
            new DiagnosticDescriptor(SA1641Identifier, SA1641Title, SA1641MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, SA1641Description, SA1641HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(
                SA1633DescriptorMissing,
                SA1634Descriptor,
                SA1635Descriptor,
                SA1636Descriptor,
                SA1637Descriptor,
                SA1638Descriptor,
                SA1639Descriptor,
                SA1640Descriptor,
                SA1641Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            var compilation = context.Compilation;

            // Disabling SA1633 will disable all other header related diagnostics.
            if (!compilation.IsAnalyzerSuppressed(SA1633Identifier))
            {
                Analyzer analyzer = new Analyzer(context.Options);
                context.RegisterSyntaxTreeActionHonorExclusions(ctx => analyzer.HandleSyntaxTreeAction(ctx, compilation));
            }
        }

        private sealed class Analyzer
        {
            private readonly DocumentationSettings documentationSettings;

            public Analyzer(AnalyzerOptions options)
            {
                StyleCopSettings settings = options.GetStyleCopSettings();
                this.documentationSettings = settings.DocumentationRules;
            }

            public void HandleSyntaxTreeAction(SyntaxTreeAnalysisContext context, Compilation compilation)
            {
                var root = context.Tree.GetRoot(context.CancellationToken);

                // don't process empty files
                if (root.FullSpan.IsEmpty)
                {
                    return;
                }

                if (this.documentationSettings.XmlHeader)
                {
                    var fileHeader = FileHeaderHelpers.ParseXmlFileHeader(root);
                    if (fileHeader.IsMissing)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SA1633DescriptorMissing, fileHeader.GetLocation(context.Tree)));
                        return;
                    }

                    if (fileHeader.IsMalformed)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SA1633DescriptorMalformed, fileHeader.GetLocation(context.Tree)));
                        return;
                    }

                    if (!compilation.IsAnalyzerSuppressed(SA1634Identifier))
                    {
                        this.CheckCopyrightHeader(context, compilation, fileHeader);
                    }

                    if (!compilation.IsAnalyzerSuppressed(SA1639Identifier))
                    {
                        this.CheckSummaryHeader(context, compilation, fileHeader);
                    }
                }
                else
                {
                    var fileHeader = FileHeaderHelpers.ParseFileHeader(root);
                    if (fileHeader.IsMissing)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(SA1633DescriptorMissing, fileHeader.GetLocation(context.Tree)));
                        return;
                    }

                    if (!compilation.IsAnalyzerSuppressed(SA1635Identifier))
                    {
                        if (string.IsNullOrWhiteSpace(fileHeader.CopyrightText))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(SA1635Descriptor, fileHeader.GetLocation(context.Tree)));
                            return;
                        }

                        if (compilation.IsAnalyzerSuppressed(SA1636Identifier))
                        {
                            return;
                        }

                        if (!this.CompareCopyrightText(fileHeader.CopyrightText))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(SA1636Descriptor, fileHeader.GetLocation(context.Tree)));
                            return;
                        }
                    }
                }
            }

            private void CheckCopyrightHeader(SyntaxTreeAnalysisContext context, Compilation compilation, XmlFileHeader fileHeader)
            {
                var copyrightElement = fileHeader.GetElement("copyright");
                if (copyrightElement == null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(SA1634Descriptor, fileHeader.GetLocation(context.Tree)));
                    return;
                }

                if (!compilation.IsAnalyzerSuppressed(SA1637Identifier))
                {
                    this.CheckFile(context, compilation, fileHeader, copyrightElement);
                }

                if (!compilation.IsAnalyzerSuppressed(SA1640Identifier))
                {
                    this.CheckCompanyName(context, compilation, fileHeader, copyrightElement);
                }

                if (!compilation.IsAnalyzerSuppressed(SA1635Identifier))
                {
                    this.CheckCopyrightText(context, compilation, fileHeader, copyrightElement);
                }
            }

            private void CheckFile(SyntaxTreeAnalysisContext context, Compilation compilation, XmlFileHeader fileHeader, XElement copyrightElement)
            {
                var fileAttribute = copyrightElement.Attribute("file");
                if (fileAttribute == null)
                {
                    var location = fileHeader.GetElementLocation(context.Tree, copyrightElement);
                    context.ReportDiagnostic(Diagnostic.Create(SA1637Descriptor, location));
                    return;
                }

                if (compilation.IsAnalyzerSuppressed(SA1638Identifier))
                {
                    return;
                }

                var fileName = Path.GetFileName(context.Tree.FilePath);
                if (string.CompareOrdinal(fileAttribute.Value, fileName) != 0)
                {
                    var location = fileHeader.GetElementLocation(context.Tree, copyrightElement);
                    context.ReportDiagnostic(Diagnostic.Create(SA1638Descriptor, location));
                }
            }

            private void CheckCopyrightText(SyntaxTreeAnalysisContext context, Compilation compilation, XmlFileHeader fileHeader, XElement copyrightElement)
            {
                var copyrightText = copyrightElement.Value;
                if (string.IsNullOrWhiteSpace(copyrightText))
                {
                    var location = fileHeader.GetElementLocation(context.Tree, copyrightElement);
                    context.ReportDiagnostic(Diagnostic.Create(SA1635Descriptor, location));
                    return;
                }

                if (compilation.IsAnalyzerSuppressed(SA1636Identifier))
                {
                    return;
                }

                var settingsCopyrightText = this.documentationSettings.CopyrightText;
                if (string.Equals(settingsCopyrightText, DocumentationSettings.DefaultCopyrightText, StringComparison.OrdinalIgnoreCase))
                {
                    // The copyright text is meaningless until the company name is configured by the user.
                    return;
                }

                // trim any leading / trailing new line or whitespace characters (those are a result of the XML formatting)
                if (!this.CompareCopyrightText(copyrightText.Trim('\r', '\n', ' ', '\t')))
                {
                    var location = fileHeader.GetElementLocation(context.Tree, copyrightElement);
                    context.ReportDiagnostic(Diagnostic.Create(SA1636Descriptor, location));
                }
            }

            private void CheckCompanyName(SyntaxTreeAnalysisContext context, Compilation compilation, XmlFileHeader fileHeader, XElement copyrightElement)
            {
                var companyName = copyrightElement.Attribute("company")?.Value;
                if (string.IsNullOrWhiteSpace(companyName))
                {
                    var location = fileHeader.GetElementLocation(context.Tree, copyrightElement);
                    context.ReportDiagnostic(Diagnostic.Create(SA1640Descriptor, location));
                    return;
                }

                if (compilation.IsAnalyzerSuppressed(SA1641Identifier))
                {
                    return;
                }

                if (string.Equals(this.documentationSettings.CompanyName, DocumentationSettings.DefaultCompanyName, StringComparison.OrdinalIgnoreCase))
                {
                    // The company name is meaningless until configured by the user.
                    return;
                }

                if (string.CompareOrdinal(companyName, this.documentationSettings.CompanyName) != 0)
                {
                    var location = fileHeader.GetElementLocation(context.Tree, copyrightElement);
                    context.ReportDiagnostic(Diagnostic.Create(SA1641Descriptor, location));
                }
            }

            private void CheckSummaryHeader(SyntaxTreeAnalysisContext context, Compilation compilation, XmlFileHeader fileHeader)
            {
                var summaryElement = fileHeader.GetElement("summary");
                if (summaryElement == null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(SA1639Descriptor, fileHeader.GetLocation(context.Tree)));
                    return;
                }

                if (string.IsNullOrWhiteSpace(summaryElement.Value))
                {
                    var location = fileHeader.GetElementLocation(context.Tree, summaryElement);
                    context.ReportDiagnostic(Diagnostic.Create(SA1639Descriptor, location));
                }
            }

            private bool CompareCopyrightText(string copyrightText)
            {
                // make sure that both \n and \r\n are accepted from the settings.
                var reformattedCopyrightTextParts = this.documentationSettings.CopyrightText.Replace("\r\n", "\n").Split('\n');
                var fileHeaderCopyrightTextParts = copyrightText.Replace("\r\n", "\n").Split('\n');

                if (reformattedCopyrightTextParts.Length != fileHeaderCopyrightTextParts.Length)
                {
                    return false;
                }

                // compare line by line, ignoring leading and trailing whitespace on each line.
                for (var i = 0; i < reformattedCopyrightTextParts.Length; i++)
                {
                    if (string.CompareOrdinal(reformattedCopyrightTextParts[i].Trim(), fileHeaderCopyrightTextParts[i].Trim()) != 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
