// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class WrapperHelper
    {
        private static readonly ImmutableDictionary<Type, Type> WrappedTypes;

        static WrapperHelper()
        {
            var codeAnalysisAssembly = typeof(SyntaxNode).GetTypeInfo().Assembly;
            var builder = ImmutableDictionary.CreateBuilder<Type, Type>();

            builder.Add(typeof(AnalyzerConfigOptionsProviderWrapper), codeAnalysisAssembly.GetType(AnalyzerConfigOptionsProviderWrapper.WrappedTypeName));
            builder.Add(typeof(AnalyzerConfigOptionsWrapper), codeAnalysisAssembly.GetType(AnalyzerConfigOptionsWrapper.WrappedTypeName));

            WrappedTypes = builder.ToImmutable();
        }

        /// <summary>
        /// Gets the type that is wrapped by the given wrapper.
        /// </summary>
        /// <param name = "wrapperType">Type of the wrapper for which the wrapped type should be retrieved.</param>
        /// <returns>The wrapped type, or null if there is no info.</returns>
        internal static Type GetWrappedType(Type wrapperType)
        {
            if (WrappedTypes.TryGetValue(wrapperType, out Type wrappedType))
            {
                return wrappedType;
            }

            return null;
        }
    }
}
