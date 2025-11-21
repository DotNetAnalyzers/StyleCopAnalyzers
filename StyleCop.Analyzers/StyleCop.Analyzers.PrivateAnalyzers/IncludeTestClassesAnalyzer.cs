// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.PrivateAnalyzers;

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class IncludeTestClassesAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Descriptor =
        new(PrivateDiagnosticIds.SP0001, "Include all test classes", "Expected test class '{0}' was not found", "Correctness", DiagnosticSeverity.Warning, isEnabledByDefault: true, customTags: new[] { "CompilationEnd" });

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(context =>
        {
            var assemblyName = context.Compilation.AssemblyName ?? string.Empty;
            if (!Regex.IsMatch(assemblyName, @"^StyleCop\.Analyzers\.Test\.CSharp\d+$"))
            {
                // This is not a test project where derived test classes are expected
                return;
            }

            // Map actual test class in current project to base type
            var testClasses = new ConcurrentDictionary<string, string>();

            context.RegisterSymbolAction(
                context =>
                {
                    var namedType = (INamedTypeSymbol)context.Symbol;
                    if (namedType.TypeKind != TypeKind.Class)
                    {
                        return;
                    }

                    testClasses[namedType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)] = namedType.BaseType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
                },
                SymbolKind.NamedType);

            context.RegisterCompilationEndAction(context =>
            {
                var currentVersion = int.Parse(assemblyName["StyleCop.Analyzers.Test.CSharp".Length..]);
                var currentTestString = "CSharp" + currentVersion;
                var previousTestString = currentVersion switch
                {
                    7 => string.Empty,
                    _ => "CSharp" + (currentVersion - 1).ToString(),
                };
                var previousAssemblyName = previousTestString switch
                {
                    "" => "StyleCop.Analyzers.Test",
                    _ => "StyleCop.Analyzers.Test." + previousTestString,
                };

                var previousAssembly = context.Compilation.Assembly.Modules.First().ReferencedAssemblySymbols.First(
                    symbol => symbol.Identity.Name == previousAssemblyName);
                if (previousAssembly is null)
                {
                    return;
                }

                var reportingLocation = context.Compilation.SyntaxTrees.FirstOrDefault()?.GetLocation(new TextSpan(0, 0)) ?? Location.None;
                var collector = new TestClassCollector(previousTestString);
                var previousTests = collector.Visit(previousAssembly);
                foreach (var previousTest in previousTests)
                {
                    string expectedTest;
                    if (previousTestString is "")
                    {
                        expectedTest = previousTest.Replace(previousAssemblyName, assemblyName).Replace("UnitTests", currentTestString + "UnitTests");
                    }
                    else
                    {
                        expectedTest = previousTest.Replace(previousTestString, currentTestString);
                    }

                    if (testClasses.TryGetValue(expectedTest, out var actualTest)
                        && actualTest == previousTest)
                    {
                        continue;
                    }

                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, reportingLocation, expectedTest));
                }
            });
        });
    }

    private sealed class TestClassCollector : SymbolVisitor<ImmutableSortedSet<string>>
    {
        private readonly string testString;

        public TestClassCollector(string testString)
        {
            this.testString = testString;
        }

        public override ImmutableSortedSet<string> Visit(ISymbol? symbol)
            => base.Visit(symbol) ?? throw new InvalidOperationException("Not reachable");

        public override ImmutableSortedSet<string>? DefaultVisit(ISymbol symbol)
            => ImmutableSortedSet<string>.Empty;

        public override ImmutableSortedSet<string> VisitAssembly(IAssemblySymbol symbol)
        {
            return this.Visit(symbol.GlobalNamespace);
        }

        public override ImmutableSortedSet<string> VisitNamespace(INamespaceSymbol symbol)
        {
            var result = ImmutableSortedSet<string>.Empty;
            foreach (var member in symbol.GetMembers())
            {
                result = result.Union(this.Visit(member)!);
            }

            return result;
        }

        public override ImmutableSortedSet<string> VisitNamedType(INamedTypeSymbol symbol)
        {
            if (this.testString is "")
            {
                if (symbol.Name.EndsWith("UnitTests"))
                {
                    return ImmutableSortedSet.Create(symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                }
                else
                {
                    return ImmutableSortedSet<string>.Empty;
                }
            }
            else if (symbol.Name.Contains(this.testString))
            {
                return ImmutableSortedSet.Create(symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            }
            else
            {
                return ImmutableSortedSet<string>.Empty;
            }
        }
    }
}
