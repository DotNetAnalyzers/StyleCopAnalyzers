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
            Dictionary<string, ImmutableHashSet<string>>? fieldDependencies = null;
            Dictionary<string, FieldVariableInfo>? fieldVariableInfos = null;

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
                    bool hasExcusedMemberForCurrent = false;

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
                                    fieldVariableInfos = new Dictionary<string, FieldVariableInfo>();
                                    foreach (var m in members)
                                    {
                                        if (m is FieldDeclarationSyntax f)
                                        {
                                            var access = MemberOrderHelper.GetAccessLevelForOrdering(f, f.GetModifiers());
                                            var spanStart = f.SpanStart;
                                            foreach (var v in f.Declaration.Variables)
                                            {
                                                fieldVariableInfos[v.Identifier.ValueText] = new FieldVariableInfo(access, spanStart);
                                            }
                                        }
                                    }

                                    fieldDependencies = FieldDependencyHelper.ComputeFieldDependencies(context.SemanticModel, members, context.CancellationToken);
                                }

                                isExcused = IsRequiredToAppearAfter(currentField, previousField, previousInfo.AccessLevel, fieldDependencies, fieldVariableInfos!);
                            }

                            if (isExcused)
                            {
                                hasExcusedMemberForCurrent = true;
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
                        else if (!hasExcusedMemberForCurrent)
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
            Dictionary<string, ImmutableHashSet<string>> fieldDependencies,
            Dictionary<string, FieldVariableInfo> variableInfos)
        {
            if (fieldDependencies == null || fieldDependencies.Count == 0)
            {
                return false;
            }

            if (FieldDependencyHelper.DependsOn(currentField, precedingField, fieldDependencies))
            {
                return true;
            }

            foreach (var variable in currentField.Declaration.Variables)
            {
                if (fieldDependencies.TryGetValue(variable.Identifier.ValueText, out var deps))
                {
                    foreach (var dep in deps)
                    {
                        if (variableInfos.TryGetValue(dep, out var depInfo))
                        {
                            if (precedingField.SpanStart < depInfo.SpanStart && precedingAccessLevel >= depInfo.AccessLevel)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private readonly struct FieldVariableInfo
        {
            public FieldVariableInfo(AccessLevel accessLevel, int spanStart)
            {
                this.AccessLevel = accessLevel;
                this.SpanStart = spanStart;
            }

            public AccessLevel AccessLevel { get; }

            public int SpanStart { get; }
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
