// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Store files as UTF-8 with byte order mark.
    /// </summary>
    /// <remarks>
    /// <para>Storing files in this encoding ensures that the files are always treated the same way by the compiler,
    /// even when compiled on systems with varying default system encodings. In addition,
    /// this encoding is the most widely supported encoding for features like visual diffs on GitHub and other tooling.
    /// This encoding is also the default encoding used when creating new C# source files within Visual Studio.
    /// </para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1412StoreFilesAsUtf8 : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1412StoreFilesAsUtf8"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1412";
        private const string Title = "Store files as UTF-8 with byte order mark";
        private const string MessageFormat = "Store files as UTF-8 with byte order mark";
        private const string Description = "Source files should be saved using the UTF-8 encoding with a byte order mark";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1412.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.MaintainabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxTreeAnalysisContext> SyntaxTreeAction = HandleSyntaxTree;

        private static byte[] utf8Preamble = Encoding.UTF8.GetPreamble();

        /// <summary>
        /// Gets the key for the detected encoding name in the <see cref="Diagnostic.Properties"/> collection.
        /// </summary>
        /// <value>
        /// The key for the detected encoding name in the <see cref="Diagnostic.Properties"/> collection.
        /// </value>
        public static string EncodingProperty { get; } = "Encoding";

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxTreeActionHonorExclusions(SyntaxTreeAction);
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            byte[] preamble = context.Tree.Encoding.GetPreamble();

            if (!IsUtf8Preamble(preamble))
            {
                ImmutableDictionary<string, string> properties = ImmutableDictionary<string, string>.Empty.SetItem(EncodingProperty, context.Tree.Encoding?.WebName ?? "<null>");
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.Create(context.Tree, TextSpan.FromBounds(0, 0)), properties));
            }
        }

        private static bool IsUtf8Preamble(byte[] preamble)
        {
            if (preamble == null || preamble.Length != utf8Preamble.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < utf8Preamble.Length; i++)
                {
                    if (utf8Preamble[i] != preamble[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
