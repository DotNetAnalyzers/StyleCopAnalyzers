﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Settings.ObjectModel;

    /// <summary>
    /// Provides extension methods to deal for analyzers.
    /// </summary>
    internal static class AnalyzerExtensions
    {
        /// <summary>
        /// Register an action to be executed at completion of parsing of a code document. A syntax tree action reports
        /// diagnostics about the <see cref="SyntaxTree"/> of a document.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        /// <param name="action">Action to be executed at completion of parsing of a document.</param>
        public static void RegisterSyntaxTreeAction(this AnalysisContext context, Action<SyntaxTreeAnalysisContext, StyleCopSettings> action)
        {
            context.RegisterSyntaxTreeAction(
                c =>
                {
                    StyleCopSettings settings = context.GetStyleCopSettings(c.Options, c.CancellationToken);
                    action(c, settings);
                });
        }

        /// <summary>
        /// Register an action to be executed at completion of parsing of a code document. A syntax tree action reports
        /// diagnostics about the <see cref="SyntaxTree"/> of a document.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        /// <param name="action">Action to be executed at completion of parsing of a document.</param>
        public static void RegisterSyntaxTreeAction(this CompilationStartAnalysisContext context, Action<SyntaxTreeAnalysisContext, StyleCopSettings> action)
        {
            context.RegisterSyntaxTreeAction(
                c =>
                {
                    StyleCopSettings settings = context.GetStyleCopSettings(c.Options, c.CancellationToken);
                    action(c, settings);
                });
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a <see cref="SyntaxNode"/> with an
        /// appropriate kind. A syntax node action can report diagnostics about a <see cref="SyntaxNode"/>, and can also
        /// collect state information to be used by other syntax node actions or code block end actions.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        /// <param name="action">Action to be executed at completion of semantic analysis of a
        /// <see cref="SyntaxNode"/>.</param>
        /// <param name="syntaxKind">The kind of syntax that should be analyzed.</param>
        /// <typeparam name="TLanguageKindEnum">Enum type giving the syntax node kinds of the source language for which
        /// the action applies.</typeparam>
        public static void RegisterSyntaxNodeAction<TLanguageKindEnum>(this AnalysisContext context, Action<SyntaxNodeAnalysisContext, StyleCopSettings> action, TLanguageKindEnum syntaxKind)
            where TLanguageKindEnum : struct
        {
            context.RegisterSyntaxNodeAction(action, LanguageKindArrays<TLanguageKindEnum>.GetOrCreateArray(syntaxKind));
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a <see cref="SyntaxNode"/> with an
        /// appropriate kind. A syntax node action can report diagnostics about a <see cref="SyntaxNode"/>, and can also
        /// collect state information to be used by other syntax node actions or code block end actions.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        /// <param name="action">Action to be executed at completion of semantic analysis of a
        /// <see cref="SyntaxNode"/>.</param>
        /// <param name="syntaxKinds">The kinds of syntax that should be analyzed.</param>
        /// <typeparam name="TLanguageKindEnum">Enum type giving the syntax node kinds of the source language for which
        /// the action applies.</typeparam>
        public static void RegisterSyntaxNodeAction<TLanguageKindEnum>(this AnalysisContext context, Action<SyntaxNodeAnalysisContext, StyleCopSettings> action, ImmutableArray<TLanguageKindEnum> syntaxKinds)
            where TLanguageKindEnum : struct
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    StyleCopSettings settings = context.GetStyleCopSettings(c.Options, c.CancellationToken);
                    action(c, settings);
                },
                syntaxKinds);
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a <see cref="SyntaxNode"/> with an
        /// appropriate kind. A syntax node action can report diagnostics about a <see cref="SyntaxNode"/>, and can also
        /// collect state information to be used by other syntax node actions or code block end actions.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        /// <param name="action">Action to be executed at completion of semantic analysis of a
        /// <see cref="SyntaxNode"/>.</param>
        /// <param name="syntaxKind">The kind of syntax that should be analyzed.</param>
        /// <typeparam name="TLanguageKindEnum">Enum type giving the syntax node kinds of the source language for which
        /// the action applies.</typeparam>
        public static void RegisterSyntaxNodeAction<TLanguageKindEnum>(this CompilationStartAnalysisContext context, Action<SyntaxNodeAnalysisContext, StyleCopSettings> action, TLanguageKindEnum syntaxKind)
            where TLanguageKindEnum : struct
        {
            context.RegisterSyntaxNodeAction(action, LanguageKindArrays<TLanguageKindEnum>.GetOrCreateArray(syntaxKind));
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a <see cref="SyntaxNode"/> with an
        /// appropriate kind. A syntax node action can report diagnostics about a <see cref="SyntaxNode"/>, and can also
        /// collect state information to be used by other syntax node actions or code block end actions.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        /// <param name="action">Action to be executed at completion of semantic analysis of a
        /// <see cref="SyntaxNode"/>.</param>
        /// <param name="syntaxKinds">The kinds of syntax that should be analyzed.</param>
        /// <typeparam name="TLanguageKindEnum">Enum type giving the syntax node kinds of the source language for which
        /// the action applies.</typeparam>
        public static void RegisterSyntaxNodeAction<TLanguageKindEnum>(this CompilationStartAnalysisContext context, Action<SyntaxNodeAnalysisContext, StyleCopSettings> action, ImmutableArray<TLanguageKindEnum> syntaxKinds)
            where TLanguageKindEnum : struct
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    StyleCopSettings settings = context.GetStyleCopSettings(c.Options, c.CancellationToken);
                    action(c, settings);
                },
                syntaxKinds);
        }

        private static class LanguageKindArrays<TLanguageKindEnum>
            where TLanguageKindEnum : struct
        {
            private static readonly ConcurrentDictionary<TLanguageKindEnum, ImmutableArray<TLanguageKindEnum>> Arrays =
                new ConcurrentDictionary<TLanguageKindEnum, ImmutableArray<TLanguageKindEnum>>();

            private static readonly Func<TLanguageKindEnum, ImmutableArray<TLanguageKindEnum>> CreateValueFactory = CreateValue;

            public static ImmutableArray<TLanguageKindEnum> GetOrCreateArray(TLanguageKindEnum syntaxKind)
            {
                return Arrays.GetOrAdd(syntaxKind, CreateValueFactory);
            }

            private static ImmutableArray<TLanguageKindEnum> CreateValue(TLanguageKindEnum syntaxKind)
            {
                return ImmutableArray.Create(syntaxKind);
            }
        }
    }
}
