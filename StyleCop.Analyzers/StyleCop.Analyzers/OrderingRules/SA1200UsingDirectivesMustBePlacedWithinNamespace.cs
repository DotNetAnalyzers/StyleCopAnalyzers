// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

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
    /// <item>Placing using-alias directives within the namespace eliminates compiler confusion between conflicting
    /// types.</item>
    /// <item>When multiple namespaces are defined within a single file, placing using directives within the namespace
    /// elements scopes references and aliases.</item>
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
    /// <c>Guid g = new Guid("hello");</c></para>
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
    internal class SA1200UsingDirectivesMustBePlacedWithinNamespace : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1200UsingDirectivesMustBePlacedWithinNamespace"/>
        /// analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1200";
        private const string Title = "Using directives must be placed within namespace";
        private const string MessageFormat = "Using directive must appear within a namespace declaration";
        private const string Description = "A C# using directive is placed outside of a namespace element.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1200.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SyntaxNodeAnalysisContext> CompilationUnitAction = HandleCompilationUnit;

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
            context.RegisterSyntaxNodeActionHonorExclusions(CompilationUnitAction, SyntaxKind.CompilationUnit);
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context)
        {
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
                    // suppress SA1200 if file contains an attribute in the global namespace
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
                // Using directive must appear within a namespace declaration
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, directive.GetLocation()));
            }
        }
    }
}
