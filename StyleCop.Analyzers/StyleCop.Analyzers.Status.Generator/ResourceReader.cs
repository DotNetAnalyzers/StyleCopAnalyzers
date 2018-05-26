// Copyright (c) Dennis Fischer. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Status.Generator
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Resources;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides a method that creates binary resources in memory
    /// </summary>
    internal class ResourceReader
    {
        /// <summary>
        /// Creates a <see cref="ResourceDescription"/> from a resx file im memory.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <param name="path">The path to the resource</param>
        /// <returns>A resource description for the resx file</returns>
        internal static ResourceDescription ReadResource(string name, string path)
        {
            return new ResourceDescription(
                name,
                () =>
            {
                using (ResXResourceReader reader = new ResXResourceReader(path))
                {
                    IDictionaryEnumerator enumerator = reader.GetEnumerator();
                    MemoryStream memStream = new MemoryStream();
                    using (IResourceWriter writer = new ResourceWriter(memStream))
                    {
                        while (enumerator.MoveNext())
                        {
                            writer.AddResource(enumerator.Key.ToString(), enumerator.Value);
                        }
                    }
                    return new MemoryStream(memStream.ToArray());
                }
            },
            false);
        }

        /// <summary>
        /// Gets a list that contains a <see cref="ResourceDescription"/> for each resx
        /// file in the directory and it's subdirectories.
        /// </summary>
        /// <param name="root">
        /// The path of the root directory.
        /// </param>
        /// <returns>
        /// A <see cref="ImmutableArray{ResourceDescription}"/> that contains a <see cref="ResourceDescription"/> for each resx
        /// file in the directory and it's subdirectories.
        /// </returns>
        internal static ImmutableArray<ResourceDescription> GetResourcesRecursive(string root)
        {
            root = Path.GetFullPath(root);
            List<ResourceDescription> descriptions = new List<ResourceDescription>();

            foreach (var path in Directory.EnumerateFiles(root, "*.resx", SearchOption.AllDirectories))
            {
                string relativePath = Path.ChangeExtension(
                    Path.GetFullPath(path)
                    .Substring(root.Length)
                    .Replace(Path.DirectorySeparatorChar, '.')
                    .Replace(Path.AltDirectorySeparatorChar, '.'), "resources")
                    .TrimStart('.');

                descriptions.Add(ReadResource(relativePath, path));
            }

            return ImmutableArray.CreateRange(descriptions);
        }
    }
}
