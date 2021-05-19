// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// A C# using directive is placed outside of a namespace element.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a using directive or a using-alias directive is placed outside of a
    /// namespace element, unless the file does not contain any namespace elements.</para>
    ///
    /// <para>For example, the following code would result in two violations of this rule.</para>
    ///
    /// <code language="cs">
    /// using System;
    /// using Guid = System.Guid;
    ///
    /// namespace Microsoft.Sample
    /// {
    ///   public class Program
    ///   {
    ///   }
    /// }
    /// </code>
    ///
    /// <para>The following code, however, would not result in any violations of this rule:</para>
    ///
    /// <code language="cs">
    /// namespace Microsoft.Sample
    /// {
    ///   using System;
    ///   using Guid = System.Guid;
    ///
    ///   public class Program
    ///   {
    ///   }
    /// }
    /// </code>
    ///
    /// <para>There are subtle differences between placing using directives within a namespace element, rather than
    /// outside of the namespace, including:</para>
    ///
    /// <list type="number">
    /// <item><description>Placing using-alias directives within the namespace eliminates compiler confusion between
    /// conflicting types.</description></item>
    /// <item><description>When multiple namespaces are defined within a single file, placing using directives within
    /// the namespace elements scopes references and aliases.</description></item>
    /// </list>
    ///
    /// <h2>1. Eliminating Type Confusion</h2>
    ///
    /// <para>Consider the following code, which contains a using-alias directive defined outside of the namespace
    /// element. The code creates a new class called <c>Guid</c>, and also defines a using-alias directive to map the
    /// name <c>Guid</c> to the type <see cref="Guid"/>. Finally, the code creates an instance of the type
    /// <c>Guid</c>:</para>
    ///
    /// <code language="cs">
    /// using Guid = System.Guid;
    ///
    /// namespace Microsoft.Sample
    /// {
    ///   public class Guid
    ///   {
    ///     public Guid(string s)
    ///     {
    ///     }
    ///   }
    ///
    ///   public class Program
    ///   {
    ///     public static void Main(string[] args)
    ///     {
    ///       Guid g = new Guid("hello");
    ///     }
    ///   }
    /// }
    /// </code>
    ///
    /// <para>This code will compile cleanly, without any compiler errors. However, it is unclear which version of the
    /// <c>Guid</c> type is being allocated. If the using directive is moved inside of the namespace, as shown below, a
    /// compiler error will occur:</para>
    ///
    /// <code language="cs">
    /// namespace Microsoft.Sample
    /// {
    ///   using Guid = System.Guid;
    ///
    ///   public class Guid
    ///   {
    ///     public Guid(string s)
    ///     {
    ///     }
    ///   }
    ///
    ///   public class Program
    ///   {
    ///     public static void Main(string[] args)
    ///     {
    ///       Guid g = new Guid("hello");
    ///     }
    ///   }
    /// }
    /// </code>
    ///
    /// <para>The code fails on the following compiler error, found on the line containing
    /// <c>Guid g = new Guid("hello");</c>:</para>
    ///
    /// <quote>CS0576: Namespace 'Microsoft.Sample' contains a definition conflicting with alias 'Guid'</quote>
    ///
    /// <para>The code creates an alias to the <see cref="Guid"/> type called <c>Guid</c>, and also creates its own type
    /// called <c>Guid</c> with a matching constructor interface. Later, the code creates an instance of the type
    /// <c>Guid</c>. To create this instance, the compiler must choose between the two different definitions of
    /// <c>Guid</c>. When the using-alias directive is placed outside of the namespace element, the compiler will choose
    /// the local definition of <c>Guid</c> defined within the local namespace, and completely ignore the using-alias
    /// directive defined outside of the namespace. This, unfortunately, is not obvious when reading the code.</para>
    ///
    /// <para>When the using-alias directive is positioned within the namespace, however, the compiler has to choose
    /// between two different, conflicting <c>Guid</c> types both defined within the same namespace. Both of these types
    /// provide a matching constructor. The compiler is unable to make a decision, so it flags the compiler
    /// error.</para>
    ///
    /// <para>Placing the using-alias directive outside of the namespace is a bad practice because it can lead to
    /// confusion in situations such as this, where it is not obvious which version of the type is actually being used.
    /// This can potentially lead to a bug which might be difficult to diagnose.</para>
    ///
    /// <para>Placing using-alias directives within the namespace element eliminates this as a source of bugs.</para>
    ///
    /// <h2>2. Multiple Namespaces</h2>
    ///
    /// <para>Placing multiple namespace elements within a single file is generally a bad idea, but if and when this is
    /// done, it is a good idea to place all using directives within each of the namespace elements, rather than
    /// globally at the top of the file.This will scope the namespaces tightly, and will also help to avoid the kind of
    /// behavior described above.</para>
    ///
    /// <para>It is important to note that when code has been written with using directives placed outside of the
    /// namespace, care should be taken when moving these directives within the namespace, to ensure that this is not
    /// changing the semantics of the code.As explained above, placing using-alias directives within the namespace
    /// element allows the compiler to choose between conflicting types in ways that will not happen when the directives
    /// are placed outside of the namespace.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1200UsingDirectivesMustBePlacedCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1200UsingDirectivesMustBePlacedCorrectly"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1200";

        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1200.md";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1200Title), OrderingResources.ResourceManager, typeof(OrderingResources));

        private static readonly LocalizableString MessageFormatInside = new LocalizableResourceString(nameof(OrderingResources.SA1200MessageFormatInside), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString DescriptionInside = new LocalizableResourceString(nameof(OrderingResources.SA1200DescriptionInside), OrderingResources.ResourceManager, typeof(OrderingResources));

        private static readonly LocalizableString MessageFormatOutside = new LocalizableResourceString(nameof(OrderingResources.SA1200MessageFormatOutside), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString DescriptionOutside = new LocalizableResourceString(nameof(OrderingResources.SA1200DescriptionOutside), OrderingResources.ResourceManager, typeof(OrderingResources));

#pragma warning disable SA1202 // Elements should be ordered by access
        internal static readonly DiagnosticDescriptor DescriptorInside =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormatInside, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, DescriptionInside, HelpLink);

        internal static readonly DiagnosticDescriptor DescriptorOutside =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormatOutside, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, DescriptionOutside, HelpLink);
#pragma warning restore SA1202 // Elements should be ordered by access

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> NamespaceDeclarationAction = HandleNamespaceDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DescriptorInside);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(CompilationUnitAction, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
        }

        /// <summary>
        /// This method reports a diagnostic for any using directive placed outside a namespace declaration. No
        /// diagnostics are reported unless <see cref="OrderingSettings.UsingDirectivesPlacement"/> is
        /// <see cref="UsingDirectivesPlacement.InsideNamespace"/>.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        /// <param name="settings">The effective StyleCop analysis settings.</param>
        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            if (settings.OrderingRules.UsingDirectivesPlacement != UsingDirectivesPlacement.InsideNamespace)
            {
                return;
            }

            CompilationUnitSyntax syntax = (CompilationUnitSyntax)context.Node;

            List<SyntaxNode> usingDirectives = new List<SyntaxNode>();
            foreach (SyntaxNode child in syntax.ChildNodes())
            {
                switch (child.Kind())
                {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.DelegateDeclaration:
                    // Suppress SA1200 if file contains a type in the global namespace
                    return;

                case SyntaxKind.AttributeList:
                    // Suppress SA1200 if file contains an attribute in the global namespace
                    return;

                case SyntaxKind.GlobalStatement:
                    // Suppress SA1200 if file contains top-level statements
                    return;

                case SyntaxKind.UsingDirective:
                    usingDirectives.Add(child);
                    continue;

                case SyntaxKind.ExternAliasDirective:
                case SyntaxKind.NamespaceDeclaration:
                default:
                    continue;
                }
            }

            foreach (var directive in usingDirectives)
            {
                // Using directive should appear within a namespace declaration
                context.ReportDiagnostic(Diagnostic.Create(DescriptorInside, directive.GetLocation()));
            }
        }

        /// <summary>
        /// This method reports a diagnostic for any using directive placed within a namespace declaration. No
        /// diagnostics are reported unless <see cref="OrderingSettings.UsingDirectivesPlacement"/> is
        /// <see cref="UsingDirectivesPlacement.OutsideNamespace"/>.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        /// <param name="settings">The effective StyleCop analysis settings.</param>
        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            if (settings.OrderingRules.UsingDirectivesPlacement != UsingDirectivesPlacement.OutsideNamespace)
            {
                return;
            }

            NamespaceDeclarationSyntax syntax = (NamespaceDeclarationSyntax)context.Node;
            foreach (UsingDirectiveSyntax directive in syntax.Usings)
            {
                // Using directive should appear outside a namespace declaration
#pragma warning disable RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor (https://github.com/dotnet/roslyn-analyzers/issues/4103)
                context.ReportDiagnostic(Diagnostic.Create(DescriptorOutside, directive.GetLocation()));
#pragma warning restore RS1005 // ReportDiagnostic invoked with an unsupported DiagnosticDescriptor
            }
        }
    }
}
