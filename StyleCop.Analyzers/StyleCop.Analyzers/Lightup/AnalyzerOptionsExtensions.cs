// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class AnalyzerOptionsExtensions
    {
        private static readonly Func<AnalyzerOptions, object> AnalyzerConfigOptionsProviderAccessor;

        static AnalyzerOptionsExtensions()
        {
            AnalyzerConfigOptionsProviderAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<AnalyzerOptions, object>(typeof(AnalyzerOptions), nameof(AnalyzerConfigOptionsProvider));
        }

        public static AnalyzerConfigOptionsProviderWrapper AnalyzerConfigOptionsProvider(this AnalyzerOptions options)
        {
            return AnalyzerConfigOptionsProviderWrapper.FromObject(AnalyzerConfigOptionsProviderAccessor(options));
        }
    }
}
