// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Settings.ObjectModel;
    using Path = System.IO.Path;

    internal static class FileNameHelpers
    {
        internal static string GetFileNameAndSuffix(string path, out string suffix)
        {
            string fileName = Path.GetFileName(path);
            int firstDot = fileName.IndexOf('.');
            if (firstDot >= 0)
            {
                suffix = fileName.Substring(firstDot);
                fileName = fileName.Substring(0, firstDot);
            }
            else
            {
                suffix = string.Empty;
            }

            return fileName;
        }

        internal static string GetConventionalFileName(TypeDeclarationSyntax typeDeclaration, FileNamingConvention convention)
        {
            if (typeDeclaration.TypeParameterList == null)
            {
                return GetSimpleFileName(typeDeclaration);
            }

            switch (convention)
            {
            case FileNamingConvention.Metadata:
                return GetMetadataFileName(typeDeclaration);

            default:
                return GetStyleCopFileName(typeDeclaration);
            }
        }

        internal static string GetSimpleFileName(TypeDeclarationSyntax typeDeclaration)
        {
            return $"{typeDeclaration.Identifier.ValueText}";
        }

        private static string GetMetadataFileName(TypeDeclarationSyntax typeDeclaration)
        {
            return $"{typeDeclaration.Identifier.ValueText}`{typeDeclaration.Arity}";
        }

        private static string GetStyleCopFileName(TypeDeclarationSyntax typeDeclaration)
        {
            var typeParameterList = string.Join(",", typeDeclaration.TypeParameterList.Parameters.Select(p => p.Identifier.ValueText));
            return $"{typeDeclaration.Identifier.ValueText}{{{typeParameterList}}}";
        }
    }
}
