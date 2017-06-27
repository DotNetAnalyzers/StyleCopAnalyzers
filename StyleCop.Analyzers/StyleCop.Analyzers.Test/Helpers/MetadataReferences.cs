// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Metadata references used to create test projects.
    /// </summary>
    internal static class MetadataReferences
    {
        internal static readonly MetadataReference CorlibReference = CreateDotNetFrameworkMetadataReference("mscorlib").WithAliases(ImmutableArray.Create("global", "corlib"));
        internal static readonly MetadataReference SystemReference = CreateDotNetFrameworkMetadataReference("System").WithAliases(ImmutableArray.Create("global", "system"));
        internal static readonly MetadataReference SystemCoreReference = CreateDotNetFrameworkMetadataReference("System.Core");
        internal static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        internal static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        internal static readonly MetadataReference SystemRuntimeReference;
        internal static readonly MetadataReference SystemValueTupleReference;

        static MetadataReferences()
        {
            if (typeof(string).Assembly.GetType("System.ValueTuple", false) != null)
            {
                // mscorlib contains ValueTuple, so no need to add a separate reference
                SystemRuntimeReference = null;
                SystemValueTupleReference = null;
            }
            else
            {
                Assembly systemRuntime = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(x => x.GetName().Name == "System.Runtime");
                if (systemRuntime != null)
                {
                    SystemRuntimeReference = MetadataReference.CreateFromFile(systemRuntime.Location);
                }

                Assembly systemValueTuple = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(x => x.GetName().Name == "System.ValueTuple");
                if (systemValueTuple != null)
                {
                    SystemValueTupleReference = MetadataReference.CreateFromFile(systemValueTuple.Location);
                }
            }
        }

        private static MetadataReference CreateDotNetFrameworkMetadataReference(string name)
        {
            var referenceAssemblyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Reference Assemblies", "Microsoft", "Framework", ".NETFramework", "v4.6");
            var assemblyFilePath = Path.Combine(referenceAssemblyPath, $"{name}.dll");

            return MetadataReference.CreateFromFile(assemblyFilePath);
        }
    }
}
