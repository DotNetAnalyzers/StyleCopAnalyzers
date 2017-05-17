// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using System.Reflection;
    using System.IO;

    /// <summary>
    /// Metadata references used to create test projects.
    /// </summary>
    internal static class MetadataReferences
    {
        internal static readonly MetadataReference MscorlibReference = MetadataReference.CreateFromFile(GetReferencePath("mscorlib")).WithAliases(ImmutableArray.Create("global", "corlib"));
        internal static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location).WithAliases(ImmutableArray.Create("global", "corlib"));
        internal static readonly MetadataReference RuntimeReference = MetadataReference.CreateFromFile(GetReferencePath("System.Runtime")).WithAliases(ImmutableArray.Create("global", "corlib"));

        internal static readonly MetadataReference SystemCollectionsReference = MetadataReference.CreateFromFile(GetReferencePath("System.Collections"));
        internal static readonly MetadataReference SystemComponentModelAnnotationsReference = MetadataReference.CreateFromFile(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute).GetTypeInfo().Assembly.Location);
        internal static readonly MetadataReference SystemComponentModelPrimitivesReference = MetadataReference.CreateFromFile(typeof(System.ComponentModel.DesignOnlyAttribute).GetTypeInfo().Assembly.Location);
        internal static readonly MetadataReference SystemDiagnosticsDebugReference = MetadataReference.CreateFromFile(typeof(System.Diagnostics.Debug).GetTypeInfo().Assembly.Location).WithAliases(ImmutableArray.Create("global", "system"));
        internal static readonly MetadataReference SystemLinqReference = MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location);
        internal static readonly MetadataReference SystemLinqExpressionsReference = MetadataReference.CreateFromFile(typeof(System.Linq.Expressions.Expression).GetTypeInfo().Assembly.Location);
        internal static readonly MetadataReference SystemNetPrimitivesReference = MetadataReference.CreateFromFile(typeof(System.Net.Cookie).GetTypeInfo().Assembly.Location);
        internal static readonly MetadataReference SystemValueTupleReference = MetadataReference.CreateFromFile(typeof(ValueTuple<,>).GetTypeInfo().Assembly.Location);

        internal static readonly MetadataReference MicrosoftCodeAnalysisCommonReference = MetadataReference.CreateFromFile(typeof(Compilation).GetTypeInfo().Assembly.Location);
        internal static readonly MetadataReference MicrosoftCodeAnalysisCSharpReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).GetTypeInfo().Assembly.Location);
        internal static readonly MetadataReference MicrosoftVisualStudioTestPlatformCommon = MetadataReference.CreateFromFile(typeof(Microsoft.VisualStudio.TestPlatform.Common.TestPlatformDefaults).GetTypeInfo().Assembly.Location);

        private static string GetReferencePath(string dll) => Path.Combine(Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location), dll + ".dll");
    }
}
