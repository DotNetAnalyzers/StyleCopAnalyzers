namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The code uses one of the basic C# types, but does not use the built-in alias for the type.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when one of the following types are used anywhere in the code:
    /// <c>Boolean</c>, <c>Byte</c>, <c>Char</c>, <c>Decimal</c>, <c>Double</c>, <c>Int16</c>, <c>Int32</c>,
    /// <c>Int64</c>, <c>Object</c>, <c>SByte</c>, <c>Single</c>, <c>String</c>, <c>UInt16</c>, <c>UInt32</c>,
    /// <c>UInt64</c>.</para>
    ///
    /// <para>A violation also occurs when any of these types are represented in the code using the full namespace for
    /// the type:
    /// <c>System.Boolean</c>, <c>System.Byte</c>, <c>System.Char</c>, <c>System.Decimal</c>, <c>System.Double</c>,
    /// <c>System.Int16</c>, <c>System.Int32</c>, <c>System.Int64</c>, <c>System.Object</c>, <c>System.SByte</c>,
    /// <c>System.Single</c>, <c>System.String</c>, <c>System.UInt16</c>, <c>System.UInt32</c>,
    /// <c>System.UInt64</c>.</para>
    ///
    /// <para>Rather than using the type name or the fully-qualified type name, the built-in aliases for these types
    /// should always be used: <c>bool</c>, <c>byte</c>, <c>char</c>, <c>decimal</c>, <c>double</c>, <c>short</c>,
    /// <c>int</c>, <c>long</c>, <c>object</c>, <c>sbyte</c>, <c> float</c>, <c>string</c>, <c>ushort</c>, <c>uint</c>,
    /// <c>ulong</c>.</para>
    ///
    /// <para>The following table lists each of these types in all three formats:</para>
    ///
    /// <list type="table">
    /// <listheader>
    ///   <term>Type Alias</term>
    ///   <term>Type</term>
    ///   <term>Fully-Qualified Type</term>
    /// </listheader>
    /// <item>
    ///   <description>bool</description>
    ///   <description>Boolean</description>
    ///   <description>System.Boolean</description>
    /// </item>
    /// <item>
    ///   <description>byte</description>
    ///   <description>Byte</description>
    ///   <description>System.Byte</description>
    /// </item>
    /// <item>
    ///   <description>char</description>
    ///   <description>Char</description>
    ///   <description>System.Char</description>
    /// </item>
    /// <item>
    ///   <description>decimal</description>
    ///   <description>Decimal</description>
    ///   <description>System.Decimal</description>
    /// </item>
    /// <item>
    ///   <description>double</description>
    ///   <description>Double</description>
    ///   <description>System.Double</description>
    /// </item>
    /// <item>
    ///   <description>short</description>
    ///   <description>Int16</description>
    ///   <description>System.Int16</description>
    /// </item>
    /// <item>
    ///   <description>int</description>
    ///   <description>Int32</description>
    ///   <description>System.Int32</description>
    /// </item>
    /// <item>
    ///   <description>long</description>
    ///   <description>Int64</description>
    ///   <description>System.Int64</description>
    /// </item>
    /// <item>
    ///   <description>object</description>
    ///   <description>Object</description>
    ///   <description>System.Object</description>
    /// </item>
    /// <item>
    ///   <description>sbyte</description>
    ///   <description>SByte</description>
    ///   <description>System.SByte</description>
    /// </item>
    /// <item>
    ///   <description>float</description>
    ///   <description>Single</description>
    ///   <description>System.Single</description>
    /// </item>
    /// <item>
    ///   <description>string</description>
    ///   <description>String</description>
    ///   <description>System.String</description>
    /// </item>
    /// <item>
    ///   <description>ushort</description>
    ///   <description>UInt16</description>
    ///   <description>System.UInt16</description>
    /// </item>
    /// <item>
    ///   <description>uint</description>
    ///   <description>UInt32</description>
    ///   <description>System.UInt32</description>
    /// </item>
    /// <item>
    ///   <description>ulong</description>
    ///   <description>UInt64</description>
    ///   <description>System.UInt64</description>
    /// </item>
    /// </list>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1121UseBuiltInTypeAlias : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1121";
        internal const string Title = "Use built-in type alias";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "The code uses one of the basic C# types, but does not use the built-in alias for the type.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1121.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
