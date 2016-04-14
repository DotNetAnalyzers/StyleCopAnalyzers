// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.NamingRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Helpers.ObjectPools;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Parameters should match inherited names
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the name of a parameter does not match its inherited name.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1315ParametersShouldMatchInheritedNames : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1315ParametersShouldMatchInheritedNames"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1315";
        internal const string PropertyName = "NewNames";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(NamingResources.SA1315Title), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(NamingResources.SA1315MessageFormat), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(NamingResources.SA1315Description), NamingResources.ResourceManager, typeof(NamingResources));
        private static readonly string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1315.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.NamingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;
        private static readonly Action<SymbolAnalysisContext> HandleMethodAction = HandleMethod;

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
            context.RegisterSymbolAction(HandleMethodAction, SymbolKind.Method);
        }

        private static void HandleMethod(SymbolAnalysisContext context)
        {
            var symbol = context.Symbol as IMethodSymbol;

            if (!symbol.IsOverride && !NamedTypeHelpers.IsImplementingAnInterfaceMember(symbol))
            {
                return;
            }

            if (!symbol.CanBeReferencedByName
                || !symbol.Locations.Any(x => x.IsInSource)
                || string.IsNullOrWhiteSpace(symbol.Name))
            {
                return;
            }

            var pool = SharedPools.Default<List<IMethodSymbol>>();

            using (var pooledObject = pool.GetPooledObject())
            {
                var originalDefinitions = pooledObject.Object;
                NamedTypeHelpers.GetOriginalDefinitions(originalDefinitions, symbol);

                if (originalDefinitions.Count == 0)
                {
                    // We did not find any original definitions so we don't have to do anything. This happens if the method has an override modifier
                    // but does not have any valid method it is overriding.
                    return;
                }

                for (int i = 0; i < symbol.Parameters.Length; i++)
                {
                    var currentParameter = symbol.Parameters[i];
                    bool foundMatch = false;
                    foreach (var originalDefinition in originalDefinitions)
                    {
                        var originalParameter = originalDefinition.Parameters[i];

                        if (currentParameter.Name == originalParameter.Name)
                        {
                            foundMatch = true;
                            break;
                        }
                    }

                    if (!foundMatch)
                    {

                        var baseClassMethod = originalDefinitions.FirstOrDefault(x => x.ContainingType.TypeKind != TypeKind.Interface);
                        if (baseClassMethod != null)
                        {
                            // If there is a base class with a matching method declaration prefer it
                            var properties = ImmutableDictionary<string, string>.Empty.SetItem(PropertyName, baseClassMethod.Parameters[i].Name);

                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentParameter.Locations.First(), properties, null));
                        }
                        else
                        {
                            var resultBuilder = StringBuilderPool.Allocate();

                            // originalDefinitions must have at least one entry
                            resultBuilder.Append(originalDefinitions[0].Parameters[i].Name);

                            for (int j = 1; j < originalDefinitions.Count; j++)
                            {
                                resultBuilder.Append(',');
                                resultBuilder.Append(originalDefinitions[j].Parameters[i].Name);
                            }

                            var properties = ImmutableDictionary<string, string>.Empty.SetItem(PropertyName, StringBuilderPool.ReturnAndFree(resultBuilder));

                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, currentParameter.Locations.First(), properties, null));
                        }
                    }
                }
            }
        }
    }
}
