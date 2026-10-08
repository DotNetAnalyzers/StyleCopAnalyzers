// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.Helpers
{
    using System.IO;
    using Microsoft.CodeAnalysis.Testing;

    /// <summary>
    /// Reference assemblies for tests that need runtime support which the default (.NET 8) reference assemblies
    /// don't have.
    /// </summary>
    internal static class RuntimeReferenceAssemblies
    {
        /// <summary>
        /// Gets the .NET 9 reference assemblies. These are needed for code that uses the <c>allows ref struct</c>
        /// anti-constraint, which requires the runtime to support by-ref-like generics.
        /// </summary>
        /// <value>
        /// The .NET 9 reference assemblies.
        /// </value>
        public static ReferenceAssemblies Net90 { get; } =
            new ReferenceAssemblies(
                "net9.0",
                new PackageIdentity("Microsoft.NETCore.App.Ref", "9.0.0"),
                Path.Combine("ref", "net9.0"));
    }
}
