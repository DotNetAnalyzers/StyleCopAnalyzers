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
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// An element within a C# code file is out of order in relation to the other elements in the code.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code elements within a file do not follow a standard ordering
    /// scheme.</para>
    ///
    /// <para>To comply with this rule, elements at the file root level or within a namespace should be positioned in the
    /// following order:</para>
    ///
    /// <list type="bullet">
    /// <item><description>Extern alias directives</description></item>
    /// <item><description>Using directives</description></item>
    /// <item><description>Namespaces</description></item>
    /// <item><description>Delegates</description></item>
    /// <item><description>Enums</description></item>
    /// <item><description>Interfaces</description></item>
    /// <item><description>Structs</description></item>
    /// <item><description>Classes</description></item>
    /// </list>
    ///
    /// <para>Within a class, struct, or interface, elements should be positioned in the following order:</para>
    ///
    /// <list type="bullet">
    /// <item><description>Fields</description></item>
    /// <item><description>Constructors</description></item>
    /// <item><description>Finalizers</description></item>
    /// <item><description>Delegates</description></item>
    /// <item><description>Events</description></item>
    /// <item><description>Enums</description></item>
    /// <item><description>Interfaces</description></item>
    /// <item><description>Properties</description></item>
    /// <item><description>Indexers</description></item>
    /// <item><description>Methods</description></item>
    /// <item><description>Structs</description></item>
    /// <item><description>Classes</description></item>
    /// </list>
    ///
    /// <para>Complying with a standard ordering scheme based on element type can increase the readability and
    /// maintainability of the file and encourage code reuse.</para>
    ///
    /// <para>When implementing an interface, it is sometimes desirable to group all members of the interface next to
    /// one another. This will sometimes require violating this rule, if the interface contains elements of different
    /// types. This problem can be solved through the use of partial classes.</para>
    ///
    /// <list type="number">
    /// <item><description>Add the partial attribute to the class, if the class is not already
    /// partial.</description></item>
    /// <item><description>Add a second partial class with the same name. It is possible to place this in the same file,
    /// just below the original class, or within a second file.</description></item>
    /// <item><description>Move the interface inheritance and all members of the interface implementation to the second
    /// part of the class.</description></item>
    /// </list>
    ///
    /// <para>For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Represents a customer of the system.
    /// /// &lt;/summary&gt;
    /// public partial class Customer
    /// {
    ///     // Contains the main functionality of the class.
    /// }
    ///
    /// /// &lt;content&gt;
    /// /// Implements the ICollection class.
    /// /// &lt;/content&gt;
    /// public partial class Customer : ICollection
    /// {
    ///     public int Count
    ///     {
    ///         get { return this.count; }
    ///     }
    ///
    ///     public bool IsSynchronized
    ///     {
    ///         get { return false; }
    ///     }
    ///
    ///     public object SyncRoot
    ///     {
    ///         get { return null; }
    ///     }
    ///
    ///     public void CopyTo(Array array, int index)
    ///     {
    ///         throw new NotImplementedException();
    ///     }
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1201ElementsMustAppearInTheCorrectOrder : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1201ElementsMustAppearInTheCorrectOrder"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1201";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1201Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1201MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1201Description), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1201.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<SyntaxKind> TypeDeclarationKinds =
            ImmutableArray.Create(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration);

        // extern alias and usings are missing here because the compiler itself is enforcing the right order.
        private static readonly ImmutableArray<SyntaxKind> OuterOrder = ImmutableArray.Create(
            SyntaxKind.NamespaceDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.ClassDeclaration);

        private static readonly ImmutableArray<SyntaxKind> TypeMemberOrder = ImmutableArray.Create(
            SyntaxKind.FieldDeclaration,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.DestructorDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.EventDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.IndexerDeclaration,
            SyntaxKind.ConversionOperatorDeclaration,
            SyntaxKind.OperatorDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.ClassDeclaration);

        private static readonly Dictionary<SyntaxKind, string> MemberNames = new Dictionary<SyntaxKind, string>
        {
            [SyntaxKind.NamespaceDeclaration] = "namespace",
            [SyntaxKind.DelegateDeclaration] = "delegate",
            [SyntaxKind.EnumDeclaration] = "enum",
            [SyntaxKind.InterfaceDeclaration] = "interface",
            [SyntaxKind.StructDeclaration] = "struct",
            [SyntaxKind.ClassDeclaration] = "class",
            [SyntaxKind.FieldDeclaration] = "field",
            [SyntaxKind.ConstructorDeclaration] = "constructor",
            [SyntaxKind.DestructorDeclaration] = "destructor",
            [SyntaxKind.DelegateDeclaration] = "delegate",
            [SyntaxKind.EventDeclaration] = "event",
            [SyntaxKind.EnumDeclaration] = "enum",
            [SyntaxKind.InterfaceDeclaration] = "interface",
            [SyntaxKind.PropertyDeclaration] = "property",
            [SyntaxKind.IndexerDeclaration] = "indexer",
            [SyntaxKind.MethodDeclaration] = "method",
            [SyntaxKind.ConversionOperatorDeclaration] = "conversion",
            [SyntaxKind.OperatorDeclaration] = "operator",
        };

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> NamespaceDeclarationAction = HandleNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TypeDeclarationAction = HandleTypeDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(CompilationUnitAction, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(NamespaceDeclarationAction, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(TypeDeclarationAction, TypeDeclarationKinds);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int kindIndex = elementOrder.IndexOf(OrderingTrait.Kind);
            if (kindIndex < 0)
            {
                return;
            }

            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            HandleMemberList(context, elementOrder, kindIndex, typeDeclaration.Members, TypeMemberOrder);
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int kindIndex = elementOrder.IndexOf(OrderingTrait.Kind);
            if (kindIndex < 0)
            {
                return;
            }

            var compilationUnit = (CompilationUnitSyntax)context.Node;

            HandleMemberList(context, elementOrder, kindIndex, compilationUnit.Members, OuterOrder);
        }

        private static void HandleNamespaceDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int kindIndex = elementOrder.IndexOf(OrderingTrait.Kind);
            if (kindIndex < 0)
            {
                return;
            }

            var compilationUnit = (NamespaceDeclarationSyntax)context.Node;

            HandleMemberList(context, elementOrder, kindIndex, compilationUnit.Members, OuterOrder);
        }

        private static void HandleMemberList(SyntaxNodeAnalysisContext context, ImmutableArray<OrderingTrait> elementOrder, int kindIndex, SyntaxList<MemberDeclarationSyntax> members, ImmutableArray<SyntaxKind> order)
        {
            for (int i = 0; i < members.Count - 1; i++)
            {
                if (members[i + 1].IsKind(SyntaxKind.IncompleteMember))
                {
                    i++;
                    continue;
                }

                if (members[i].IsKind(SyntaxKind.IncompleteMember))
                {
                    continue;
                }

                bool compareKind = true;
                for (int j = 0; compareKind && j < kindIndex; j++)
                {
                    switch (elementOrder[j])
                    {
                    case OrderingTrait.Accessibility:
                        if (MemberOrderHelper.GetAccessLevelForOrdering(members[i + 1], members[i + 1].GetModifiers())
                            != MemberOrderHelper.GetAccessLevelForOrdering(members[i], members[i].GetModifiers()))
                        {
                            compareKind = false;
                        }

                        continue;

                    case OrderingTrait.Constant:
                    case OrderingTrait.Readonly:
                        // Only fields may be marked const or readonly, and all fields have the same kind.
                        continue;

                    case OrderingTrait.Static:
                        bool currentIsStatic = members[i].GetModifiers().Any(SyntaxKind.StaticKeyword);
                        bool nextIsStatic = members[i + 1].GetModifiers().Any(SyntaxKind.StaticKeyword);
                        if (currentIsStatic != nextIsStatic)
                        {
                            compareKind = false;
                        }

                        continue;

                    case OrderingTrait.Kind:
                    default:
                        continue;
                    }
                }

                if (!compareKind)
                {
                    continue;
                }

                var elementSyntaxKind = members[i].Kind();
                elementSyntaxKind = elementSyntaxKind == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : elementSyntaxKind;
                int index = order.IndexOf(elementSyntaxKind);

                var nextElementSyntaxKind = members[i + 1].Kind();
                nextElementSyntaxKind = nextElementSyntaxKind == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : nextElementSyntaxKind;
                int nextIndex = order.IndexOf(nextElementSyntaxKind);

                if (index > nextIndex)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(members[i + 1]), MemberNames[nextElementSyntaxKind], MemberNames[elementSyntaxKind]));
                }
            }
        }
    }
}
