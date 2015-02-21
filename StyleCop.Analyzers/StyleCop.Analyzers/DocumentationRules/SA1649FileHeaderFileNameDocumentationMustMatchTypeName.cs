namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The file tag within the file header at the top of a C# code file does not match the first type declared in the file. For generics
    /// that are defined as Class1&lt;T&gt; the name of the file needs to be Class1{T}.cs and this should appear in
    /// the header also. Partial classes are ignored.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the file tag within the file header at the top of a C# file does not contain the name
    /// of the first type in the file. For example, consider a C# source file named Class1.cs, with the following
    /// header:</para>
    ///
    /// <code language="csharp">
    /// //-----------------------------------------------------------------------
    /// // &lt;copyright file="ThisIsNotTheCorrectTypeName.cs" company="My Company"&gt;
    /// //     Custom company copyright tag.
    /// // &lt;/copyright&gt;
    /// //-----------------------------------------------------------------------
    /// public class Class1
    /// {
    /// }
    /// </code>
    ///
    /// <para>A violation of this rule would occur, since the file tag does not contain the correct name of the first
    /// type in the file. The header should be written as:</para>
    ///
    /// <code language="csharp">
    /// //-----------------------------------------------------------------------
    /// // &lt;copyright file="Class1.cs" company="My Company"&gt;
    /// //     Custom company copyright tag.
    /// // &lt;/copyright&gt;
    /// //-----------------------------------------------------------------------
    /// public class Class1
    /// {
    /// }
    /// </code>
    ///
    /// <para>A generic class should be written as:</para>
    ///
    /// <code language="csharp">
    /// //-----------------------------------------------------------------------
    /// // &lt;copyright file="Class1{T}.cs" company="My Company"&gt;
    /// //     Custom company copyright tag.
    /// // &lt;/copyright&gt;
    /// //-----------------------------------------------------------------------
    /// public class Class1&lt;T&gt;
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1649FileHeaderFileNameDocumentationMustMatchTypeName : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1649FileHeaderFileNameDocumentationMustMatchTypeName"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1649";
        private const string Title = "File header file name documentation must match type name";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "The file tag within the file header at the top of a C# code file does not match the first type declared in the file. For generics that are defined as Class1<T> the name of the file needs to be Class1{T}.cs and this should appear in the header also. Partial classes are ignored.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1649.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

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
