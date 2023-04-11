﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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

    internal static class GenericAnalyzerTest
    {
        internal static readonly ReferenceAssemblies ReferenceAssemblies;

        internal static readonly ReferenceAssemblies ReferenceAssembliesNet50;

        internal static readonly ReferenceAssemblies ReferenceAssembliesNet60;

        internal static readonly ReferenceAssemblies ReferenceAssembliesNet70;

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
                    _ => throw new InvalidOperationException("Unknown version."),
                };

            ReferenceAssemblies = ReferenceAssemblies.Default.AddPackages(ImmutableArray.Create(
                new PackageIdentity("Microsoft.CodeAnalysis.CSharp", codeAnalysisTestVersion),
                new PackageIdentity("System.ValueTuple", "4.5.0")));

            ReferenceAssembliesNet50 = ReferenceAssemblies.Net.Net50.AddPackages(ImmutableArray.Create(
                new PackageIdentity("Microsoft.CodeAnalysis.CSharp", codeAnalysisTestVersion)));

            ReferenceAssembliesNet60 = ReferenceAssemblies.Net.Net60.AddPackages(ImmutableArray.Create(
                new PackageIdentity("Microsoft.CodeAnalysis.CSharp", codeAnalysisTestVersion)));

            ReferenceAssembliesNet70 = ReferenceAssemblies.Net.Net70.AddPackages(ImmutableArray.Create(
                new PackageIdentity("Microsoft.CodeAnalysis.CSharp", codeAnalysisTestVersion)));

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
