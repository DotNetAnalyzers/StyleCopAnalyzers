// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Threading;
    using Microsoft.CodeAnalysis;

    internal static class SemanticModelExtensions
    {
        private static readonly Func<SemanticModel, int, CancellationToken, ImmutableArrayWrapper<IImportScopeWrapper>> GetImportScopesAccessor;

        static SemanticModelExtensions()
        {
            GetImportScopesAccessor = LightupHelpers.CreateImmutableArrayMethodAccessor<SemanticModel, int, CancellationToken, IImportScopeWrapper>(typeof(SemanticModel), typeof(int), typeof(CancellationToken), nameof(GetImportScopes));
        }

        public static ImmutableArrayWrapper<IImportScopeWrapper> GetImportScopes(this SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            return GetImportScopesAccessor(semanticModel, position, cancellationToken);
        }
    }
}
