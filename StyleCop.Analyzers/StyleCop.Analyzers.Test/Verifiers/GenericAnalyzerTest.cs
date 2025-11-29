// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Host.Mef;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.VisualStudio.Composition;
    using StyleCop.Analyzers.Lightup;

    internal static class GenericAnalyzerTest
    {
        internal static readonly ReferenceAssemblies ReferenceAssemblies;

        private static readonly Lazy<IExportProviderFactory> ExportProviderFactory;

        static GenericAnalyzerTest()
        {
            string codeAnalysisTestVersion =
                typeof(Compilation).Assembly.GetName().Version.Major switch
                {
                    1 => "1.2.1",
                    2 => "2.8.2",
                    3 => "3.6.0",
                    4 => "4.0.1",
                    5 => "5.0.0",
                    _ => throw new InvalidOperationException("Unknown version."),
                };

            // Use appropriate default reference assemblies per the support matrix:
            // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version
            ReferenceAssemblies defaultReferenceAssemblies;
            if (LightupHelpers.SupportsCSharp12)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.Net.Net80;
            }
            else if (LightupHelpers.SupportsCSharp11)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.Net.Net70;
            }
            else if (LightupHelpers.SupportsCSharp10)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.Net.Net60;
            }
            else if (LightupHelpers.SupportsCSharp9)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.Net.Net50;
            }
            else if (LightupHelpers.SupportsCSharp8)
            {
                defaultReferenceAssemblies = ReferenceAssemblies.NetCore.NetCoreApp30;
            }
            else
            {
                defaultReferenceAssemblies = ReferenceAssemblies.Default;
            }

            ReferenceAssemblies = defaultReferenceAssemblies.AddPackages(ImmutableArray.Create(
                new PackageIdentity("Microsoft.CodeAnalysis.CSharp", codeAnalysisTestVersion),
                new PackageIdentity("System.ValueTuple", "4.5.0")));

            ExportProviderFactory = new Lazy<IExportProviderFactory>(
                () =>
                {
                    var discovery = new AttributedPartDiscovery(Resolver.DefaultInstance, isNonPublicSupported: true);
                    var parts = Task.Run(() => discovery.CreatePartsAsync(MefHostServices.DefaultAssemblies)).GetAwaiter().GetResult();
                    var catalog = ComposableCatalog.Create(Resolver.DefaultInstance).AddParts(parts);

                    var configuration = CompositionConfiguration.Create(catalog);
                    var runtimeComposition = RuntimeComposition.CreateRuntimeComposition(configuration);
                    return runtimeComposition.CreateExportProviderFactory();
                },
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        internal static AdhocWorkspace CreateWorkspace()
        {
            var exportProvider = ExportProviderFactory.Value.CreateExportProvider();
            var host = MefV1HostServices.Create(exportProvider.AsExportProvider());
            return new AdhocWorkspace(host);
        }
    }
}
