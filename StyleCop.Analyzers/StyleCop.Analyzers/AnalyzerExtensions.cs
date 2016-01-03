// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Settings.ObjectModel;

    /// <summary>
    /// Provides extension methods to deal for analyzers.
    /// </summary>
    internal static class AnalyzerExtensions
    {
        /// <summary>
        /// A cache of the result of computing whether a document has an auto-generated header.
        /// </summary>
        /// <remarks>
        /// This allows many analyzers that run on every token in the file to avoid checking
        /// the same state in the document repeatedly.
        /// </remarks>
        private static Tuple<WeakReference<Compilation>, ConcurrentDictionary<SyntaxTree, bool>> generatedHeaderCache
            = Tuple.Create(new WeakReference<Compilation>(null), default(ConcurrentDictionary<SyntaxTree, bool>));

        private static Tuple<WeakReference<Compilation>, StrongBox<StyleCopSettings>> settingsCache
            = Tuple.Create(new WeakReference<Compilation>(null), default(StrongBox<StyleCopSettings>));

        /// <summary>
        /// Register an action to be executed at completion of parsing of a code document. A syntax tree action reports
        /// diagnostics about the <see cref="SyntaxTree"/> of a document.
        /// </summary>
        /// <remarks>This method honors exclusions.</remarks>
        /// <param name="context">The analysis context.</param>
        /// <param name="action">Action to be executed at completion of parsing of a document.</param>
        public static void RegisterSyntaxTreeActionHonorExclusions(this CompilationStartAnalysisContext context, Action<SyntaxTreeAnalysisContext> action)
        {
            Compilation compilation = context.Compilation;
            ConcurrentDictionary<SyntaxTree, bool> cache = GetOrCreateGeneratedDocumentCache(compilation);

            context.RegisterSyntaxTreeAction(
                c =>
                {
                    if (c.IsGeneratedDocument(cache))
                    {
                        return;
                    }

                    // Honor the containing document item's ExcludeFromStylecop=True
                    // MSBuild metadata, if analyzers have access to it.
                    //// TODO: code here

                    action(c);
                });
        }

        /// <summary>
        /// Register an action to be executed at completion of parsing of a code document. A syntax tree action reports
        /// diagnostics about the <see cref="SyntaxTree"/> of a document.
        /// </summary>
        /// <remarks>This method honors exclusions.</remarks>
        /// <param name="context">The analysis context.</param>
        /// <param name="action">Action to be executed at completion of parsing of a document.</param>
        public static void RegisterSyntaxTreeActionHonorExclusions(this CompilationStartAnalysisContext context, Action<SyntaxTreeAnalysisContext, StyleCopSettings> action)
        {
            Compilation compilation = context.Compilation;
            ConcurrentDictionary<SyntaxTree, bool> cache = GetOrCreateGeneratedDocumentCache(compilation);
            StrongBox<StyleCopSettings> settingsCache = GetOrCreateStyleCopSettingsCache(compilation);

            context.RegisterSyntaxTreeAction(
                c =>
                {
                    if (c.IsGeneratedDocument(cache))
                    {
                        return;
                    }

                    // Honor the containing document item's ExcludeFromStylecop=True
                    // MSBuild metadata, if analyzers have access to it.
                    //// TODO: code here

                    StyleCopSettings settings = settingsCache.Value;
                    if (settings == null)
                    {
                        StyleCopSettings updatedSettings = SettingsHelper.GetStyleCopSettings(c.Options, c.CancellationToken);
                        StyleCopSettings previous = Interlocked.CompareExchange(ref settingsCache.Value, updatedSettings, null);
                        settings = previous ?? updatedSettings;
                    }

                    action(c, settings);
                });
        }

        /// <summary>
        /// Gets or creates a cache which can be used with <see cref="GeneratedCodeAnalysisExtensions"/> methods to
        /// efficiently determine whether or not a source file is considered generated.
        /// </summary>
        /// <param name="compilation">The compilation which the cache applies to.</param>
        /// <returns>A cache which tracks the syntax trees in a compilation which are considered generated.</returns>
        public static ConcurrentDictionary<SyntaxTree, bool> GetOrCreateGeneratedDocumentCache(this Compilation compilation)
        {
            var headerCache = generatedHeaderCache;

            Compilation cachedCompilation;
            if (!headerCache.Item1.TryGetTarget(out cachedCompilation) || cachedCompilation != compilation)
            {
                var replacementCache = Tuple.Create(new WeakReference<Compilation>(compilation), new ConcurrentDictionary<SyntaxTree, bool>());
                while (true)
                {
                    var prior = Interlocked.CompareExchange(ref generatedHeaderCache, replacementCache, headerCache);
                    if (prior == headerCache)
                    {
                        headerCache = replacementCache;
                        break;
                    }

                    headerCache = prior;
                    if (headerCache.Item1.TryGetTarget(out cachedCompilation) && cachedCompilation == compilation)
                    {
                        break;
                    }
                }
            }

            return headerCache.Item2;
        }

        /// <summary>
        /// Gets a <see cref="StrongBox{T}"/> which can store a <see cref="StyleCopSettings"/> instance to improve
        /// efficiency across multiple analyzers which examine settings.
        /// </summary>
        /// <param name="compilation">The compilation which the cache applies to.</param>
        /// <returns>A <see cref="StrongBox{T}"/> which can store a <see cref="StyleCopSettings"/> instance.</returns>
        public static StrongBox<StyleCopSettings> GetOrCreateStyleCopSettingsCache(this Compilation compilation)
        {
            var currentSettingsCache = settingsCache;

            Compilation cachedCompilation;
            if (!currentSettingsCache.Item1.TryGetTarget(out cachedCompilation) || cachedCompilation != compilation)
            {
                var replacementCache = Tuple.Create(new WeakReference<Compilation>(compilation), new StrongBox<StyleCopSettings>(null));
                while (true)
                {
                    var prior = Interlocked.CompareExchange(ref settingsCache, replacementCache, currentSettingsCache);
                    if (prior == currentSettingsCache)
                    {
                        currentSettingsCache = replacementCache;
                        break;
                    }

                    currentSettingsCache = prior;
                    if (currentSettingsCache.Item1.TryGetTarget(out cachedCompilation) && cachedCompilation == compilation)
                    {
                        break;
                    }
                }
            }

            return currentSettingsCache.Item2;
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a <see cref="SyntaxNode"/> with an
        /// appropriate kind. A syntax node action can report diagnostics about a <see cref="SyntaxNode"/>, and can also
        /// collect state information to be used by other syntax node actions or code block end actions.
        /// </summary>
        /// <remarks>This method honors exclusions.</remarks>
        /// <param name="context">Action will be executed only if the kind of a <see cref="SyntaxNode"/> matches
        /// <paramref name="syntaxKind"/>.</param>
        /// <param name="action">Action to be executed at completion of semantic analysis of a
        /// <see cref="SyntaxNode"/>.</param>
        /// <param name="syntaxKind">The kind of syntax that should be analyzed.</param>
        /// <typeparam name="TLanguageKindEnum">Enum type giving the syntax node kinds of the source language for which
        /// the action applies.</typeparam>
        public static void RegisterSyntaxNodeActionHonorExclusions<TLanguageKindEnum>(this CompilationStartAnalysisContext context, Action<SyntaxNodeAnalysisContext> action, TLanguageKindEnum syntaxKind)
            where TLanguageKindEnum : struct
        {
            context.RegisterSyntaxNodeActionHonorExclusions(action, LanguageKindArrays<TLanguageKindEnum>.GetOrCreateArray(syntaxKind));
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a <see cref="SyntaxNode"/> with an
        /// appropriate kind. A syntax node action can report diagnostics about a <see cref="SyntaxNode"/>, and can also
        /// collect state information to be used by other syntax node actions or code block end actions.
        /// </summary>
        /// <remarks>This method honors exclusions.</remarks>
        /// <param name="context">Action will be executed only if the kind of a <see cref="SyntaxNode"/> matches
        /// <paramref name="syntaxKind"/>.</param>
        /// <param name="action">Action to be executed at completion of semantic analysis of a
        /// <see cref="SyntaxNode"/>.</param>
        /// <param name="syntaxKind">The kind of syntax that should be analyzed.</param>
        /// <typeparam name="TLanguageKindEnum">Enum type giving the syntax node kinds of the source language for which
        /// the action applies.</typeparam>
        public static void RegisterSyntaxNodeActionHonorExclusions<TLanguageKindEnum>(this CompilationStartAnalysisContext context, Action<SyntaxNodeAnalysisContext, StyleCopSettings> action, TLanguageKindEnum syntaxKind)
            where TLanguageKindEnum : struct
        {
            context.RegisterSyntaxNodeActionHonorExclusions(action, LanguageKindArrays<TLanguageKindEnum>.GetOrCreateArray(syntaxKind));
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a <see cref="SyntaxNode"/> with an
        /// appropriate kind. A syntax node action can report diagnostics about a <see cref="SyntaxNode"/>, and can also
        /// collect state information to be used by other syntax node actions or code block end actions.
        /// </summary>
        /// <remarks>This method honors exclusions.</remarks>
        /// <param name="context">Action will be executed only if the kind of a <see cref="SyntaxNode"/> matches one of
        /// the <paramref name="syntaxKinds"/> values.</param>
        /// <param name="action">Action to be executed at completion of semantic analysis of a
        /// <see cref="SyntaxNode"/>.</param>
        /// <param name="syntaxKinds">The kinds of syntax that should be analyzed.</param>
        /// <typeparam name="TLanguageKindEnum">Enum type giving the syntax node kinds of the source language for which
        /// the action applies.</typeparam>
        public static void RegisterSyntaxNodeActionHonorExclusions<TLanguageKindEnum>(this CompilationStartAnalysisContext context, Action<SyntaxNodeAnalysisContext> action, ImmutableArray<TLanguageKindEnum> syntaxKinds)
            where TLanguageKindEnum : struct
        {
            Compilation compilation = context.Compilation;
            ConcurrentDictionary<SyntaxTree, bool> cache = GetOrCreateGeneratedDocumentCache(compilation);

            context.RegisterSyntaxNodeAction(
                c =>
                {
                    if (c.IsGenerated(cache))
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

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a <see cref="SyntaxNode"/> with an
        /// appropriate kind. A syntax node action can report diagnostics about a <see cref="SyntaxNode"/>, and can also
        /// collect state information to be used by other syntax node actions or code block end actions.
        /// </summary>
        /// <remarks>This method honors exclusions.</remarks>
        /// <param name="context">Action will be executed only if the kind of a <see cref="SyntaxNode"/> matches one of
        /// the <paramref name="syntaxKinds"/> values.</param>
        /// <param name="action">Action to be executed at completion of semantic analysis of a
        /// <see cref="SyntaxNode"/>.</param>
        /// <param name="syntaxKinds">The kinds of syntax that should be analyzed.</param>
        /// <typeparam name="TLanguageKindEnum">Enum type giving the syntax node kinds of the source language for which
        /// the action applies.</typeparam>
        public static void RegisterSyntaxNodeActionHonorExclusions<TLanguageKindEnum>(this CompilationStartAnalysisContext context, Action<SyntaxNodeAnalysisContext, StyleCopSettings> action, ImmutableArray<TLanguageKindEnum> syntaxKinds)
            where TLanguageKindEnum : struct
        {
            Compilation compilation = context.Compilation;
            ConcurrentDictionary<SyntaxTree, bool> cache = GetOrCreateGeneratedDocumentCache(compilation);
            StrongBox<StyleCopSettings> settingsCache = GetOrCreateStyleCopSettingsCache(compilation);

            context.RegisterSyntaxNodeAction(
                c =>
                {
                    if (c.IsGenerated(cache))
                    {
                        return;
                    }

                    // Honor the containing document item's ExcludeFromStylecop=True
                    // MSBuild metadata, if analyzers have access to it.
                    //// TODO: code here

                    StyleCopSettings settings = settingsCache.Value;
                    if (settings == null)
                    {
                        StyleCopSettings updatedSettings = SettingsHelper.GetStyleCopSettings(c.Options, c.CancellationToken);
                        StyleCopSettings previous = Interlocked.CompareExchange(ref settingsCache.Value, updatedSettings, null);
                        settings = previous ?? updatedSettings;
                    }

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
