// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Lightup;

    internal static class StyleCopDiagnosticVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        internal static DiagnosticResult Diagnostic()
            => CSharpCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider, DefaultVerifier>.Diagnostic();

        internal static DiagnosticResult Diagnostic(string diagnosticId)
            => CSharpCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider, DefaultVerifier>.Diagnostic(diagnosticId);

        internal static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => new DiagnosticResult(descriptor);

        internal static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(source, new[] { expected }, cancellationToken);

        internal static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new CSharpTest
            {
                TestCode = source,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        internal static Task VerifyCSharpDiagnosticAsync(LanguageVersion? languageVersion, string source, DiagnosticResult expected, CancellationToken cancellationToken)
            => VerifyCSharpDiagnosticAsync(languageVersion, source, settings: null, new[] { expected }, cancellationToken);

        internal static Task VerifyCSharpDiagnosticAsync(LanguageVersion? languageVersion, string source, string settings, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new CSharpTest(languageVersion)
            {
                TestCode = source,
                Settings = settings,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        internal class CSharpTest : StyleCopCodeFixVerifier<TAnalyzer, EmptyCodeFixProvider>.CSharpTest
        {
            public CSharpTest()
                : this(languageVersion: null)
            {
            }

            public CSharpTest(LanguageVersion? languageVersion)
            {
                this.LanguageVersion = languageVersion ?? this.GetDefaultLanguageVersion();
            }

            private LanguageVersion? LanguageVersion { get; }

            protected override ParseOptions CreateParseOptions()
            {
                var parseOptions = base.CreateParseOptions();
                if (this.LanguageVersion is { } languageVersion)
                {
                    parseOptions = ((CSharpParseOptions)parseOptions).WithLanguageVersion(languageVersion);
                }

                return parseOptions;
            }

            // NOTE: If needed, this method can be temporarily updated to default to a preview version
            private LanguageVersion? GetDefaultLanguageVersion()
            {
                if (LightupHelpers.SupportsCSharp15)
                {
                    // C# 15 is still in preview, so the C# 15 test project runs every test with the preview language
                    // version. Remove this once C# 15 is the default language version of the referenced compiler.
                    return LanguageVersionEx.Preview;
                }

                return null;
            }
        }
    }
}
