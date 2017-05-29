// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// Provides extension methods and utilities for working with <see cref="SyntaxNode"/>.
    /// </summary>
    internal static class NodeHelper
    {
        public static bool IsKind(this SyntaxNode node, SyntaxKindEx kind)
        {
            return node.IsKind((SyntaxKind)kind);
        }
    }
}
