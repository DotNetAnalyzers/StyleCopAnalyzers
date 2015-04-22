// This file originally obtained from 
// https://github.com/code-cracker/code-cracker/blob/08c1a01337964924eeed12be8b14c8ce8ec6b626/src/Common/CodeCracker.Common/Extensions/AnalyzerExtensions.cs
// It is subject to the Apache License 2.0
// This file has been modified since obtaining it from its original source.

namespace StyleCop.Analyzers
{
    using System;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class AnalyzerExtensions
    {
        internal static void RegisterSyntaxTreeActionHonorExclusions(this AnalysisContext context, Action<SyntaxTreeAnalysisContext> action)
        {
            context.RegisterSyntaxTreeAction(
                c =>
                {
                    if (c.IsGeneratedDocument())
                    {
                        return;
                    }

                    // Honor the containing document item's ExcludeFromStylecop=True
                    // MSBuild metadata, if analyzers have access to it.
                    //// TODO: code here

                    action(c);
                });
        }

        internal static void RegisterSyntaxNodeActionHonorExclusions<TLanguageKindEnum>(this AnalysisContext context, Action<SyntaxNodeAnalysisContext> action, params TLanguageKindEnum[] syntaxKinds) where TLanguageKindEnum : struct
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    if (c.IsGenerated())
                    {
                        return;
                    }

                    // Honor the containing document item's ExcludeFromStylecop=True
                    // MSBuild metadata, if analyzers have access to it.
                    //// TODO: code here

                    action(c);
                },
                syntaxKinds);
        }
    }
}
