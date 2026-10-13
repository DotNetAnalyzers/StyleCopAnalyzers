// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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
    /// An element within a C# code file is out of order within regard to access level, in relation to other elements in
    /// the code.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code elements within a file do not follow a standard ordering
    /// scheme based on access level.</para>
    ///
    /// <para>To comply with this rule, adjacent elements of the same type should be positioned in the following order by
    /// access level:</para>
    ///
    /// <list type="bullet">
    /// <item><description>public</description></item>
    /// <item><description>internal</description></item>
    /// <item><description>protected internal</description></item>
    /// <item><description>protected</description></item>
    /// <item><description>private</description></item>
    /// </list>
    ///
    /// <para>Complying with a standard ordering scheme based on access level can increase the readability and
    /// maintainability of the file and make it easier to identify the public interface that is being exposed from a
    /// class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1202ElementsMustBeOrderedByAccess : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1202ElementsMustBeOrderedByAccess"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1202";
        private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(OrderingResources.SA1202Title), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(OrderingResources.SA1202MessageFormat), OrderingResources.ResourceManager, typeof(OrderingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(OrderingResources.SA1202Description), OrderingResources.ResourceManager, typeof(OrderingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableHashSet<SyntaxKind> MemberKinds = ImmutableHashSet.Create(
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.FieldDeclaration,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.EventDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.IndexerDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.ConversionOperatorDeclaration,
            SyntaxKind.OperatorDeclaration,
            SyntaxKindEx.RecordDeclaration,
            SyntaxKindEx.RecordStructDeclaration,
            SyntaxKindEx.UnionDeclaration);

        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> CompilationUnitAction = HandleCompilationUnit;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> BaseNamespaceDeclarationAction = HandleBaseNamespaceDeclaration;
        private static readonly Action<SyntaxNodeAnalysisContext, StyleCopSettings> TypeDeclarationAction = HandleTypeDeclaration;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

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

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int accessibilityIndex = elementOrder.IndexOf(OrderingTrait.Accessibility);
            if (accessibilityIndex < 0)
            {
                return;
            }

            var compilationUnit = (CompilationUnitSyntax)context.Node;

            HandleMemberList(context, elementOrder, accessibilityIndex, compilationUnit.Members);
        }

        private static void HandleBaseNamespaceDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int accessibilityIndex = elementOrder.IndexOf(OrderingTrait.Accessibility);
            if (accessibilityIndex < 0)
            {
                return;
            }

            var baseNamespaceDeclaration = (BaseNamespaceDeclarationSyntaxWrapper)context.Node;

            HandleMemberList(context, elementOrder, accessibilityIndex, baseNamespaceDeclaration.Members);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context, StyleCopSettings settings)
        {
            var elementOrder = settings.OrderingRules.ElementOrder;
            int accessibilityIndex = elementOrder.IndexOf(OrderingTrait.Accessibility);
            if (accessibilityIndex < 0)
            {
                return;
            }

            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            HandleMemberList(context, elementOrder, accessibilityIndex, typeDeclaration.Members);
        }

        private static void HandleMemberList(SyntaxNodeAnalysisContext context, ImmutableArray<OrderingTrait> elementOrder, int accessibilityIndex, SyntaxList<MemberDeclarationSyntax> members)
        {
            var handledMembers = new List<MemberInfo>();
            bool hasExcusedMembers = false;
            Dictionary<FieldDeclarationSyntax, ImmutableHashSet<FieldDeclarationSyntax>> fieldDependencies = null;
            Dictionary<FieldDeclarationSyntax, AccessLevel> fieldAccessLevels = null;

            foreach (var member in members)
            {
                var currentSyntaxKind = member.Kind();
                currentSyntaxKind = currentSyntaxKind == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : currentSyntaxKind;

                // if the SyntaxKind of this member (e.g. SyntaxKind.IncompleteMember) will not
                // be handled, skip early.
                if (!MemberKinds.Contains(currentSyntaxKind))
                {
                    continue;
                }

                var modifiers = member.GetModifiers();
                AccessLevel currentAccessLevel = MemberOrderHelper.GetAccessLevelForOrdering(member, modifiers);
                bool currentIsConst = modifiers.Any(SyntaxKind.ConstKeyword);
                bool currentIsReadonly = modifiers.Any(SyntaxKind.ReadOnlyKeyword);
                bool currentIsStatic = modifiers.Any(SyntaxKind.StaticKeyword);

                var currentInfo = new MemberInfo(member, currentSyntaxKind, currentAccessLevel, currentIsConst, currentIsReadonly, currentIsStatic);

                if (handledMembers.Count > 0)
                {
                    for (int i = handledMembers.Count - 1; i >= 0; i--)
                    {
                        var previousInfo = handledMembers[i];

                        bool compareAccessLevel = true;
                        for (int j = 0; compareAccessLevel && j < accessibilityIndex; j++)
                        {
                            switch (elementOrder[j])
                            {
                            case OrderingTrait.Kind:
                                if (previousInfo.Kind != currentSyntaxKind)
                                {
                                    compareAccessLevel = false;
                                }

                                continue;

                            case OrderingTrait.Constant:
                                if (previousInfo.IsConst != currentIsConst)
                                {
                                    compareAccessLevel = false;
                                }

                                continue;

                            case OrderingTrait.Readonly:
                                if (previousInfo.IsReadonly != currentIsReadonly)
                                {
                                    compareAccessLevel = false;
                                }

                                continue;

                            case OrderingTrait.Static:
                                if (previousInfo.IsStatic != currentIsStatic)
                                {
                                    compareAccessLevel = false;
                                }

                                continue;

                            case OrderingTrait.Accessibility:
                            default:
                                continue;
                            }
                        }

                        if (!compareAccessLevel)
                        {
                            continue;
                        }

                        if (currentAccessLevel > previousInfo.AccessLevel)
                        {
                            bool isExcused = false;
                            if (member is FieldDeclarationSyntax currentField && previousInfo.Member is FieldDeclarationSyntax previousField)
                            {
                                if (fieldDependencies == null)
                                {
                                    fieldAccessLevels = new Dictionary<FieldDeclarationSyntax, AccessLevel>();
                                    fieldDependencies = ComputeFieldDependencies(context, members, fieldAccessLevels);
                                }

                                isExcused = IsRequiredToAppearAfter(currentField, previousField, previousInfo.AccessLevel, fieldDependencies, fieldAccessLevels);
                            }

                            if (isExcused)
                            {
                                hasExcusedMembers = true;
                                continue;
                            }

                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    Descriptor,
                                    NamedTypeHelpers.GetNameOrIdentifierLocation(member),
                                    AccessLevelHelper.GetName(currentAccessLevel),
                                    AccessLevelHelper.GetName(previousInfo.AccessLevel)));
                            break;
                        }
                        else if (!hasExcusedMembers)
                        {
                            break;
                        }
                    }
                }

                handledMembers.Add(currentInfo);
            }
        }

        private static bool IsRequiredToAppearAfter(
            FieldDeclarationSyntax currentField,
            FieldDeclarationSyntax precedingField,
            AccessLevel precedingAccessLevel,
            Dictionary<FieldDeclarationSyntax, ImmutableHashSet<FieldDeclarationSyntax>> fieldDependencies,
            Dictionary<FieldDeclarationSyntax, AccessLevel> fieldAccessLevels)
        {
            if (!fieldDependencies.TryGetValue(currentField, out var currentDeps) || currentDeps.IsEmpty)
            {
                return false;
            }

            if (currentDeps.Contains(precedingField))
            {
                return true;
            }

            foreach (var dep in currentDeps)
            {
                if (fieldAccessLevels.TryGetValue(dep, out var depAccess) && precedingAccessLevel > depAccess)
                {
                    return true;
                }
            }

            return false;
        }

        private static Dictionary<FieldDeclarationSyntax, ImmutableHashSet<FieldDeclarationSyntax>> ComputeFieldDependencies(
            SyntaxNodeAnalysisContext context,
            SyntaxList<MemberDeclarationSyntax> members,
            Dictionary<FieldDeclarationSyntax, AccessLevel> fieldAccessLevels)
        {
            var fieldDeclarations = new List<FieldDeclarationSyntax>();
            var fieldNames = new HashSet<string>();

            foreach (var member in members)
            {
                if (member is FieldDeclarationSyntax field)
                {
                    fieldDeclarations.Add(field);
                    var modifiers = field.GetModifiers();
                    fieldAccessLevels[field] = MemberOrderHelper.GetAccessLevelForOrdering(field, modifiers);

                    foreach (var variable in field.Declaration.Variables)
                    {
                        fieldNames.Add(variable.Identifier.ValueText);
                    }
                }
            }

            if (fieldDeclarations.Count < 2 || fieldNames.Count == 0)
            {
                return new Dictionary<FieldDeclarationSyntax, ImmutableHashSet<FieldDeclarationSyntax>>();
            }

            var fieldsWithCandidateInitializers = new List<FieldDeclarationSyntax>();
            foreach (var field in fieldDeclarations)
            {
                bool hasCandidate = false;
                foreach (var variable in field.Declaration.Variables)
                {
                    if (variable.Initializer != null && HasCandidateIdentifier(variable.Initializer.Value, fieldNames))
                    {
                        hasCandidate = true;
                        break;
                    }
                }

                if (hasCandidate)
                {
                    fieldsWithCandidateInitializers.Add(field);
                }
            }

            if (fieldsWithCandidateInitializers.Count == 0)
            {
                return new Dictionary<FieldDeclarationSyntax, ImmutableHashSet<FieldDeclarationSyntax>>();
            }

            var fieldSymbolMap = new Dictionary<ISymbol, FieldDeclarationSyntax>();
            INamedTypeSymbol enclosingType = null;

            foreach (var field in fieldDeclarations)
            {
                foreach (var variable in field.Declaration.Variables)
                {
                    var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken);
                    if (fieldSymbol != null)
                    {
                        fieldSymbolMap[fieldSymbol] = field;
                        enclosingType ??= fieldSymbol.ContainingType;
                    }
                }
            }

            if (enclosingType == null)
            {
                return new Dictionary<FieldDeclarationSyntax, ImmutableHashSet<FieldDeclarationSyntax>>();
            }

            var directDependencies = new Dictionary<FieldDeclarationSyntax, HashSet<FieldDeclarationSyntax>>();

            foreach (var field in fieldsWithCandidateInitializers)
            {
                HashSet<FieldDeclarationSyntax> deps = null;

                foreach (var variable in field.Declaration.Variables)
                {
                    if (variable.Initializer == null)
                    {
                        continue;
                    }

                    foreach (var node in variable.Initializer.Value.DescendantNodesAndSelf())
                    {
                        if (node is IdentifierNameSyntax identifierName
                            && fieldNames.Contains(identifierName.Identifier.ValueText)
                            && !IsInNameof(identifierName))
                        {
                            var symbolInfo = context.SemanticModel.GetSymbolInfo(identifierName, context.CancellationToken);
                            var symbol = symbolInfo.Symbol;

                            if (symbol is IFieldSymbol fieldSymbol
                                && !fieldSymbol.IsConst
                                && Equals(fieldSymbol.ContainingType?.OriginalDefinition, enclosingType.OriginalDefinition))
                            {
                                if (fieldSymbolMap.TryGetValue(fieldSymbol, out var targetField) && targetField != field)
                                {
                                    deps ??= new HashSet<FieldDeclarationSyntax>();
                                    deps.Add(targetField);
                                }
                            }
                        }
                    }
                }

                if (deps != null)
                {
                    directDependencies[field] = deps;
                }
            }

            if (directDependencies.Count == 0)
            {
                return new Dictionary<FieldDeclarationSyntax, ImmutableHashSet<FieldDeclarationSyntax>>();
            }

            var result = new Dictionary<FieldDeclarationSyntax, ImmutableHashSet<FieldDeclarationSyntax>>();

            foreach (var kvp in directDependencies)
            {
                var field = kvp.Key;
                var transitiveDeps = new HashSet<FieldDeclarationSyntax>();
                var queue = new Queue<FieldDeclarationSyntax>(kvp.Value);

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    if (transitiveDeps.Add(current))
                    {
                        if (directDependencies.TryGetValue(current, out var nextDeps))
                        {
                            foreach (var next in nextDeps)
                            {
                                if (!transitiveDeps.Contains(next))
                                {
                                    queue.Enqueue(next);
                                }
                            }
                        }
                    }
                }

                result[field] = transitiveDeps.ToImmutableHashSet();
            }

            return result;
        }

        private static bool HasCandidateIdentifier(SyntaxNode expression, HashSet<string> fieldNames)
        {
            foreach (var node in expression.DescendantNodesAndSelf())
            {
                if (node is IdentifierNameSyntax id && fieldNames.Contains(id.Identifier.ValueText))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsInNameof(SyntaxNode node)
        {
            for (SyntaxNode current = node.Parent; current != null; current = current.Parent)
            {
                if (current is InvocationExpressionSyntax invocation)
                {
                    if (invocation.Expression is IdentifierNameSyntax nameSyntax
                        && nameSyntax.Identifier.ValueText == "nameof")
                    {
                        return true;
                    }

                    break;
                }

                if (current is ExpressionSyntax || current is ArgumentSyntax || current is ArgumentListSyntax)
                {
                    continue;
                }

                break;
            }

            return false;
        }

        private readonly struct MemberInfo
        {
            public MemberInfo(MemberDeclarationSyntax member, SyntaxKind kind, AccessLevel accessLevel, bool isConst, bool isReadonly, bool isStatic)
            {
                this.Member = member;
                this.Kind = kind;
                this.AccessLevel = accessLevel;
                this.IsConst = isConst;
                this.IsReadonly = isReadonly;
                this.IsStatic = isStatic;
            }

            public MemberDeclarationSyntax Member { get; }

            public SyntaxKind Kind { get; }

            public AccessLevel AccessLevel { get; }

            public bool IsConst { get; }

            public bool IsReadonly { get; }

            public bool IsStatic { get; }
        }
    }
}
