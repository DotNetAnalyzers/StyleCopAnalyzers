// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules;

    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Parameter names in overriding and implementing methods should match inherited names.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a parameter name in a method that overrides a base method or
    /// implements an interface member does not match an inherited parameter name.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1315ParametersShouldMatchInheritedNames : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1315ParametersShouldMatchInheritedNames"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1315";

        /// <summary>
        /// The property containing the inherited parameter names used by the code fix.
        /// </summary>
        internal const string InheritedParameterNamesProperty = "InheritedParameterNames";

        private static readonly string HelpLink = HelpLinkHelper.GetHelpLink(DiagnosticId);
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1315Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1315MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1315Description), NamingResources.ResourceManager, typeof(NamingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SymbolAnalysisContext> MethodAction = HandleMethod;

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(MethodAction, SymbolKind.Method);
        }

        private static void HandleMethod(SymbolAnalysisContext context)
        {
            var method = (IMethodSymbol)context.Symbol;
            if (method.AssociatedSymbol != null
                || method.DeclaringSyntaxReferences.IsDefaultOrEmpty)
            {
                return;
            }

            var containingType = method.ContainingType;
            if (!method.IsOverride
                && method.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                && (containingType == null || containingType.AllInterfaces.IsDefaultOrEmpty))
            {
                return;
            }

            var inheritedMethods = new List<IMethodSymbol>();
            if (method.IsOverride)
            {
                if (method.OverriddenMethod == null)
                {
                    return;
                }

                inheritedMethods.Add(method.OverriddenMethod);
            }
            else if (!method.ExplicitInterfaceImplementations.IsDefaultOrEmpty)
            {
                foreach (var interfaceMethod in method.ExplicitInterfaceImplementations)
                {
                    inheritedMethods.Add(interfaceMethod);
                }
            }
            else
            {
                foreach (var implementedInterface in containingType.AllInterfaces)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();

                    foreach (var interfaceMember in implementedInterface.GetMembers(method.Name))
                    {
                        if (interfaceMember is IMethodSymbol interfaceMethod
                            && method.Equals(containingType.FindImplementationForInterfaceMember(interfaceMethod)))
                        {
                            inheritedMethods.Add(interfaceMethod);
                        }
                    }
                }
            }

            if (inheritedMethods.Count == 0)
            {
                return;
            }

            for (int parameterIndex = 0; parameterIndex < method.Parameters.Length; parameterIndex++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var parameter = method.Parameters[parameterIndex];
                var inheritedNames = new List<string>();
                bool nameMatchesInheritedParameter = false;

                foreach (var inheritedMethod in inheritedMethods)
                {
                    if (parameterIndex >= inheritedMethod.Parameters.Length)
                    {
                        continue;
                    }

                    string inheritedName = inheritedMethod.Parameters[parameterIndex].Name;
                    if (parameter.Name == inheritedName)
                    {
                        nameMatchesInheritedParameter = true;
                        break;
                    }

                    if (!inheritedNames.Contains(inheritedName))
                    {
                        inheritedNames.Add(inheritedName);
                    }
                }

                if (nameMatchesInheritedParameter || inheritedNames.Count == 0 || parameter.Locations.IsDefaultOrEmpty)
                {
                    continue;
                }

                string serializedNames = string.Join(",", inheritedNames);
                var properties = ImmutableDictionary<string, string>.Empty
                    .SetItem(InheritedParameterNamesProperty, serializedNames);

                context.ReportDiagnostic(Diagnostic.Create(Descriptor, parameter.Locations[0], properties, parameter.Name));
            }
        }
    }
