// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

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
    internal class SA1121UseBuiltInTypeAlias : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1121UseBuiltInTypeAlias"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1121";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1121Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1121MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1121Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1121.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink, WellKnownDiagnosticTags.Unnecessary);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;

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
            Analyzer analyzer = new Analyzer(context.Compilation.GetOrCreateUsingAliasCache());
            context.RegisterSyntaxNodeActionHonorExclusions(analyzer.HandleIdentifierNameSyntax, SyntaxKind.IdentifierName);
        }

        private sealed class Analyzer
        {
            private readonly ConcurrentDictionary<SyntaxTree, bool> usingAliasCache;

            public Analyzer(ConcurrentDictionary<SyntaxTree, bool> usingAliasCache)
            {
                this.usingAliasCache = usingAliasCache;
            }

            public void HandleIdentifierNameSyntax(SyntaxNodeAnalysisContext context)
            {
                IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)context.Node;
                if (identifierNameSyntax.IsVar)
                {
                    return;
                }

                if (identifierNameSyntax.Identifier.IsMissing)
                {
                    return;
                }

                switch (identifierNameSyntax.Identifier.ValueText)
                {
                case "bool":
                case "byte":
                case "char":
                case "decimal":
                case "double":
                case "short":
                case "int":
                case "long":
                case "object":
                case "sbyte":
                case "float":
                case "string":
                case "ushort":
                case "uint":
                case "ulong":
                    return;

                default:
                    break;
                }

                if (identifierNameSyntax.FirstAncestorOrSelf<UsingDirectiveSyntax>() != null
                    && identifierNameSyntax.FirstAncestorOrSelf<TypeArgumentListSyntax>() == null)
                {
                    return;
                }

                // Most source files will not have any using alias directives. Then we don't have to use semantics
                // if the identifier name doesn't match the name of a special type
                if (!identifierNameSyntax.SyntaxTree.ContainsUsingAlias(this.usingAliasCache))
                {
                    switch (identifierNameSyntax.Identifier.ValueText)
                    {
                    case nameof(Boolean):
                    case nameof(Byte):
                    case nameof(Char):
                    case nameof(Decimal):
                    case nameof(Double):
                    case nameof(Int16):
                    case nameof(Int32):
                    case nameof(Int64):
                    case nameof(Object):
                    case nameof(SByte):
                    case nameof(Single):
                    case nameof(String):
                    case nameof(UInt16):
                    case nameof(UInt32):
                    case nameof(UInt64):
                        break;

                    default:
                        return;
                    }
                }

                SemanticModel semanticModel = context.SemanticModel;
                INamedTypeSymbol symbol = semanticModel.GetSymbolInfo(identifierNameSyntax, context.CancellationToken).Symbol as INamedTypeSymbol;

                switch (symbol?.SpecialType)
                {
                case SpecialType.System_Boolean:
                case SpecialType.System_Byte:
                case SpecialType.System_Char:
                case SpecialType.System_Decimal:
                case SpecialType.System_Double:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_Object:
                case SpecialType.System_SByte:
                case SpecialType.System_Single:
                case SpecialType.System_String:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                    break;

                default:
                    return;
                }

                SyntaxNode locationNode = identifierNameSyntax;
                if (identifierNameSyntax.Parent is QualifiedNameSyntax)
                {
                    locationNode = identifierNameSyntax.Parent;
                }
                else if ((identifierNameSyntax.Parent as MemberAccessExpressionSyntax)?.Name == identifierNameSyntax)
                {
                    // this "weird" syntax appears for qualified references within a nameof expression
                    locationNode = identifierNameSyntax.Parent;
                }
                else if (identifierNameSyntax.Parent is NameMemberCrefSyntax && identifierNameSyntax.Parent.Parent is QualifiedCrefSyntax)
                {
                    locationNode = identifierNameSyntax.Parent.Parent;
                }

                // Allow nameof
                if (IsNameInNameOfExpression(identifierNameSyntax))
                {
                    return;
                }

                // Use built-in type alias
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, locationNode.GetLocation()));
            }

            private static bool IsNameInNameOfExpression(IdentifierNameSyntax identifierNameSyntax)
            {
                // The only time a type name can appear as an argument is for the invocation expression created for the
                // nameof keyword. This assumption is the foundation of the following simple analysis algorithm.

                // This covers the case nameof(Int32)
                if (identifierNameSyntax.Parent is ArgumentSyntax)
                {
                    return true;
                }

                MemberAccessExpressionSyntax simpleMemberAccess = identifierNameSyntax.Parent as MemberAccessExpressionSyntax;

                // This covers the case nameof(System.Int32)
                if (simpleMemberAccess != null)
                {
                    // This final check ensures that we don't consider nameof(System.Int32.ToString) the same as
                    // nameof(System.Int32)
                    return identifierNameSyntax.Parent.Parent.IsKind(SyntaxKind.Argument)
                        && simpleMemberAccess.Name == identifierNameSyntax;
                }

                return false;
            }
        }
    }
}
