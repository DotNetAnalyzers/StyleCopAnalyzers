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
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;
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
    /// <item><description>Extension blocks</description></item>
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
        internal const string MemberNameOrderProperty = "MemberNameOrder";

        private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1201Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1201MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1201Description), OrderingResources.ResourceManager, typeof(OrderingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly DiagnosticDescriptor InterruptedGroupDescriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, new LocalizableResourceString(nameof(OrderingResources.SA1201InterruptedGroupMessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources)), AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableDictionary<string, string> MemberNameOrderProperties =
            ImmutableDictionary<string, string>.Empty.Add(MemberNameOrderProperty, string.Empty);

        // extern alias and usings are missing here because the compiler itself is enforcing the right order.
        private static readonly ImmutableArray<SyntaxKind> OuterOrder = ImmutableArray.Create(
            SyntaxKindEx.FileScopedNamespaceDeclaration,
            SyntaxKind.NamespaceDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.ClassDeclaration);

        private static readonly Dictionary<SyntaxKind, string> MemberNames = new Dictionary<SyntaxKind, string>
        {
            [SyntaxKind.NamespaceDeclaration] = "a namespace",
            [SyntaxKindEx.FileScopedNamespaceDeclaration] = "a namespace",
            [SyntaxKind.DelegateDeclaration] = "a delegate",
            [SyntaxKind.EnumDeclaration] = "an enum",
            [SyntaxKind.InterfaceDeclaration] = "an interface",
            [SyntaxKind.StructDeclaration] = "a struct",
            [SyntaxKind.ClassDeclaration] = "a class",
            [SyntaxKindEx.RecordDeclaration] = "a record",
            [SyntaxKindEx.RecordStructDeclaration] = "a record struct",
            [SyntaxKindEx.UnionDeclaration] = "a union",
            [SyntaxKindEx.ExtensionBlockDeclaration] = "an extension",
            [SyntaxKind.FieldDeclaration] = "a field",
            [SyntaxKind.ConstructorDeclaration] = "a constructor",
            [SyntaxKind.DestructorDeclaration] = "a destructor",
            [SyntaxKind.EventDeclaration] = "an event",
            [SyntaxKind.EventFieldDeclaration] = "an event",
            [SyntaxKind.PropertyDeclaration] = "a property",
            [SyntaxKind.IndexerDeclaration] = "an indexer",
            [SyntaxKind.MethodDeclaration] = "a method",
            [SyntaxKind.ConversionOperatorDeclaration] = "a conversion",
            [SyntaxKind.OperatorDeclaration] = "an operator",
        };

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> BaseNamespaceDeclarationAction = HandleBaseNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TypeDeclarationAction = HandleTypeDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor, InterruptedGroupDescriptor, MemberNameOrderDescriptor);

        internal static DiagnosticDescriptor MemberNameOrderDescriptor { get; } =
            new DiagnosticDescriptor(DiagnosticId, Title, new LocalizableResourceString(nameof(OrderingResources.SA1201MemberNameOrderMessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources)), AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(context =>
            {
                context.RegisterSyntaxNodeAction(CompilationUnitAction, SyntaxKind.CompilationUnit);
                context.RegisterSyntaxNodeAction(BaseNamespaceDeclarationAction, SyntaxKinds.BaseNamespaceDeclaration);
                context.RegisterSyntaxNodeAction(TypeDeclarationAction, SyntaxKinds.TypeDeclaration);
                context.RegisterSyntaxNodeAction(TypeDeclarationAction, SyntaxKindEx.ExtensionBlockDeclaration);
            });
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;
            if (elementOrder.Contains(OrderingTrait.Name))
            {
                HandleMemberNameOrder(context, typeDeclaration, settings.OrderingRules);
            }

            int kindIndex = elementOrder.IndexOf(OrderingTrait.Kind);
            if (kindIndex < 0)
            {
                return;
            }

            if (settings.OrderingRules.MemberKindOrder is { } configuredOrder)
            {
                HandleConfiguredMemberList(context, typeDeclaration.Members, configuredOrder);
                return;
            }

            HandleMemberList(context, elementOrder, kindIndex, typeDeclaration.Members, MemberKindOrder.DefaultTypeOrder);
        }

        private static void HandleMemberNameOrder(
            SyntaxNodeAnalysisContext context,
            TypeDeclarationSyntax type,
            OrderingSettings orderingSettings)
        {
            var members = type.Members;
            var ordersFieldAccessibility = orderingSettings.ElementOrder.Contains(OrderingTrait.Accessibility);
            Dictionary<string, ImmutableHashSet<string>>? fieldDependencies = null;
            bool? preserveMemberOrder = null;
            bool? preserveInstanceOrder = null;
            for (var i = 1; i < members.Count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                var current = members[i];
                var previous = members[i - 1];
                if (MemberNameOrder.GetGroupKind(current, orderingSettings) != MemberNameOrder.GetGroupKind(previous, orderingSettings))
                {
                    continue;
                }

                var currentIdentifier = MemberNameOrder.GetSortableIdentifier(current);
                var previousIdentifier = MemberNameOrder.GetSortableIdentifier(previous);
                if (currentIdentifier.RawKind == 0
                    || previousIdentifier.RawKind == 0
                    || string.CompareOrdinal(currentIdentifier.ValueText, previousIdentifier.ValueText) >= 0
                    || MemberNameOrder.GetGroupPriority(current, orderingSettings) != MemberNameOrder.GetGroupPriority(previous, orderingSettings)
                    || MemberNameOrder.HasOrderSensitiveInitializers(current, previous)
                    || MemberNameOrder.StartsOutOfOrderGroup(members, i - 1, orderingSettings))
                {
                    continue;
                }

                preserveMemberOrder ??= MemberNameOrder.RequiresStableMemberOrder(type, context.SemanticModel, context.CancellationToken);
                if (preserveMemberOrder.Value)
                {
                    return;
                }

                if (MemberNameOrder.IsInHiddenRegion(currentIdentifier, context.CancellationToken)
                    || MemberNameOrder.IsInHiddenRegion(previousIdentifier, context.CancellationToken)
                    || MemberNameOrder.IsGeneratedDeclaration(current, context.SemanticModel, context.CancellationToken)
                    || MemberNameOrder.ContainsModuleInitializer(current, context.SemanticModel, context.CancellationToken)
                    || MemberNameOrder.ContainsModuleInitializer(previous, context.SemanticModel, context.CancellationToken))
                {
                    continue;
                }

                if (MemberNameOrder.RequiresAutoLayout(current) || MemberNameOrder.RequiresAutoLayout(previous))
                {
                    preserveInstanceOrder ??= MemberNameOrder.RequiresStableInstanceOrder(type, context.SemanticModel, context.CancellationToken);
                    if (preserveInstanceOrder.Value)
                    {
                        continue;
                    }
                }

                if (ordersFieldAccessibility && current is FieldDeclarationSyntax field && !field.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    fieldDependencies ??= FieldDependencyHelper.ComputeFieldDependencies(context.SemanticModel, members, context.CancellationToken);
                    if (FieldDependencyHelper.IsDependency(field, fieldDependencies)
                        && MemberNameOrder.CouldExposeFieldAccessibilityInversion(members, i, i - 1, orderingSettings, fieldDependencies))
                    {
                        continue;
                    }
                }

                context.ReportDiagnostic(Diagnostic.Create(
                    MemberNameOrderDescriptor,
                    currentIdentifier.GetLocation(),
                    MemberNameOrderProperties,
                    currentIdentifier.ValueText,
                    previousIdentifier.ValueText));
            }
        }

        private static void HandleConfiguredMemberList(SyntaxNodeAnalysisContext context, SyntaxList<MemberDeclarationSyntax> members, MemberKindOrder order)
        {
            var previousNamedIndex = -1;
            var previousRank = -1;
            var previousPriority = 0;
            var firstUnnamedIndex = -1;

            for (var i = 0; i < members.Count; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                var member = members[i];
                var priority = order.GetPrecedingPriority(member);
                if (MemberKindOrder.StartsNewGroup(member, priority, previousPriority))
                {
                    previousNamedIndex = -1;
                    firstUnnamedIndex = -1;
                }

                previousPriority = priority;
                if (member.IsKind(SyntaxKind.IncompleteMember))
                {
                    continue;
                }

                var rank = order.GetRank(member);
                if (rank < 0)
                {
                    if (previousNamedIndex >= 0 && firstUnnamedIndex < 0)
                    {
                        firstUnnamedIndex = i;
                    }

                    continue;
                }

                if (previousNamedIndex >= 0)
                {
                    if (rank < previousRank)
                    {
                        var previousKind = members[previousNamedIndex].Kind();
                        context.ReportDiagnostic(Diagnostic.Create(
                            Descriptor,
                            NamedTypeHelpers.GetNameOrIdentifierLocation(member),
                            Capitalize(MemberNames.GetValueOrDefault(member.Kind(), "<unknown>")),
                            MemberNames.GetValueOrDefault(previousKind, "<unknown>")));
                    }
                    else if (rank == previousRank && firstUnnamedIndex >= 0)
                    {
                        var interruption = members[firstUnnamedIndex];
                        context.ReportDiagnostic(Diagnostic.Create(
                            InterruptedGroupDescriptor,
                            NamedTypeHelpers.GetNameOrIdentifierLocation(interruption),
                            additionalLocations: ImmutableArray.Create(NamedTypeHelpers.GetNameOrIdentifierLocation(member)),
                            messageArgs: new object[] { MemberKindOrder.GetKindName(interruption.Kind()), MemberKindOrder.GetKindName(member.Kind()) }));
                    }
                }

                previousNamedIndex = i;
                previousRank = rank;
                firstUnnamedIndex = -1;
            }
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

        private static void HandleBaseNamespaceDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int kindIndex = elementOrder.IndexOf(OrderingTrait.Kind);
            if (kindIndex < 0)
            {
                return;
            }

            var baseNamespaceDeclaration = (BaseNamespaceDeclarationSyntaxWrapper)context.Node;

            HandleMemberList(context, elementOrder, kindIndex, baseNamespaceDeclaration.Members, OuterOrder);
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

                var previous = members[i];
                var current = members[i + 1];
                if (MemberOrderTraits.IsDefaultKindOutOfOrder(current, previous, elementOrder, kindIndex, order))
                {
                    var elementSyntaxKind = previous.Kind();
                    var nextElementSyntaxKind = current.Kind();

                    // [Issue #3160] Added hardening here to make sure that this won't crash when working with invalid code.
                    var nextElementMemberName = Capitalize(MemberNames.GetValueOrDefault(nextElementSyntaxKind, "<unknown>"));
                    var elementMemberName = MemberNames.GetValueOrDefault(elementSyntaxKind, "<unknown>");

                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, NamedTypeHelpers.GetNameOrIdentifierLocation(current), nextElementMemberName, elementMemberName));
                }
            }
        }

        private static string Capitalize(string value)
        {
            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }
    }
}
