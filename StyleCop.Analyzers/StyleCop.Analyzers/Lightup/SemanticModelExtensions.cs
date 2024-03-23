// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using Microsoft.CodeAnalysis;

    internal static class SemanticModelExtensions
    {
        private static readonly Func<SemanticModel, int, CancellationToken, ImmutableArray<IImportScopeWrapper>> GetImportScopesAccessor;

        static SemanticModelExtensions()
        {
            GetImportScopesAccessor = CreateGetImportScopesAccessor();
        }

        public static ImmutableArray<IImportScopeWrapper> GetImportScopes(this SemanticModel semanticModel, int position, CancellationToken cancellationToken = default)
        {
            return GetImportScopesAccessor(semanticModel, position, cancellationToken);
        }

        private static Func<SemanticModel, int, CancellationToken, ImmutableArray<IImportScopeWrapper>> CreateGetImportScopesAccessor()
        {
            var semanticModelType = typeof(SemanticModel);

            var codeAnalysisWorkspacesAssembly = semanticModelType.GetTypeInfo().Assembly;
            var nativeImportScopeType = codeAnalysisWorkspacesAssembly.GetType("Microsoft.CodeAnalysis.IImportScope");
            if (nativeImportScopeType == null)
            {
                return FallbackAccessor;
            }

            var method = semanticModelType.GetTypeInfo().GetDeclaredMethods("GetImportScopes").SingleOrDefault(IsCorrectGetImportScopesMethod);
            if (method == null)
            {
                // This should not happen, since this missing method and the type we successfully managed to retrieve above was added in the same Roslyn version
                return FallbackAccessor;
            }

            var importScopeWrapperFromObjectMethod = typeof(IImportScopeWrapper).GetTypeInfo().GetDeclaredMethod("FromObject");
            var nativeImportScopeArrayType = typeof(ImmutableArray<>).MakeGenericType(nativeImportScopeType);
            var nativeImportScopeArrayGetItemMethod = nativeImportScopeArrayType.GetTypeInfo().GetDeclaredMethod("get_Item");
            var wrapperImportScopeArrayType = typeof(ImmutableArray<IImportScopeWrapper>);
            var wrapperImportScopeArrayBuilderType = typeof(ImmutableArray<IImportScopeWrapper>.Builder);
            var wrapperImportScopeArrayBuilderAddMethod = wrapperImportScopeArrayBuilderType.GetTypeInfo().GetDeclaredMethod("Add");
            var wrapperImportScopeArrayBuilderToImmutableMethod = wrapperImportScopeArrayBuilderType.GetTypeInfo().GetDeclaredMethod("ToImmutable");
            var arrayCreateWrapperImportScopeBuilderMethod = typeof(ImmutableArray).GetTypeInfo().GetDeclaredMethods("CreateBuilder").Single(IsCorrectCreateBuilderMethod).MakeGenericMethod(typeof(IImportScopeWrapper));

            var semanticModelParameter = Expression.Parameter(typeof(SemanticModel), "semanticModel");
            var positionParameter = Expression.Parameter(typeof(int), "position");
            var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

            var nativeImportScopesVariable = Expression.Variable(nativeImportScopeArrayType, "nativeImportScopes");
            var nativeImportScopeVariable = Expression.Variable(nativeImportScopeType, "nativeImportScope");
            var countVariable = Expression.Variable(typeof(int), "count");
            var indexVariable = Expression.Variable(typeof(int), "index");
            var wrapperImportScopesBuilderVariable = Expression.Variable(wrapperImportScopeArrayBuilderType, "wrapperImportScopesBuilder");
            var wrapperImportScopesVariable = Expression.Variable(wrapperImportScopeArrayType, "wrapperImoprtScopes");
            var wrapperImportScopeVariable = Expression.Variable(typeof(IImportScopeWrapper), "wrapperImportScope");

            var breakLabel = Expression.Label("break");
            var block = Expression.Block(
                new[] { nativeImportScopesVariable, countVariable, indexVariable, wrapperImportScopesBuilderVariable, wrapperImportScopesVariable },
                //// nativeImportScopes = semanticModel.GetImportScopes(position, cancellationToken);
                Expression.Assign(
                    nativeImportScopesVariable,
                    Expression.Call(
                        semanticModelParameter,
                        method,
                        new[] { positionParameter, cancellationTokenParameter })),
                //// index = 0;
                Expression.Assign(indexVariable, Expression.Constant(0)),
                //// count = nativeImportScopes.Length;
                Expression.Assign(
                    countVariable,
                    Expression.Property(nativeImportScopesVariable, "Length")),
                //// wrapperImportScopesBuilder = ImmutableArray.CreateBuilder<IImportScopeWrapper>();
                Expression.Assign(
                    wrapperImportScopesBuilderVariable,
                    Expression.Call(null, arrayCreateWrapperImportScopeBuilderMethod)),
                Expression.Loop(
                    Expression.Block(
                        new[] { nativeImportScopeVariable, wrapperImportScopeVariable },
                        //// if (index >= count) break;
                        Expression.IfThen(
                            Expression.GreaterThanOrEqual(indexVariable, countVariable),
                            Expression.Break(breakLabel)),
                        //// nativeImportScope = nativeImportScopes[index];
                        Expression.Assign(
                            nativeImportScopeVariable,
                            Expression.Call(
                                nativeImportScopesVariable,
                                nativeImportScopeArrayGetItemMethod,
                                indexVariable)),
                        //// wrapperImportScope = IImportScopeWrapper.FromObject(nativeImportScope);
                        Expression.Assign(
                            wrapperImportScopeVariable,
                            Expression.Call(
                                null,
                                importScopeWrapperFromObjectMethod,
                                nativeImportScopeVariable)),
                        //// wrapperImportScopesBuilder.Add(wrapperImportScope);
                        Expression.Call(
                            wrapperImportScopesBuilderVariable,
                            wrapperImportScopeArrayBuilderAddMethod,
                            new[] { wrapperImportScopeVariable }),
                        //// index++;
                        Expression.PostIncrementAssign(indexVariable)),
                    breakLabel),
                //// wrapperImportScopes = wrapperImportScopesBuilder.ToImmutable();
                Expression.Assign(
                    wrapperImportScopesVariable,
                    Expression.Call(
                        wrapperImportScopesBuilderVariable,
                        wrapperImportScopeArrayBuilderToImmutableMethod)));

            var lambda = Expression.Lambda<Func<SemanticModel, int, CancellationToken, ImmutableArray<IImportScopeWrapper>>>(
                block,
                new[] { semanticModelParameter, positionParameter, cancellationTokenParameter });

            var accessor = lambda.Compile();
            return accessor;

            static bool IsCorrectGetImportScopesMethod(MethodInfo method)
            {
                var parameters = method.GetParameters();
                return parameters.Length == 2
                    && parameters[0].ParameterType == typeof(int)
                    && parameters[1].ParameterType == typeof(CancellationToken);
            }

            static bool IsCorrectCreateBuilderMethod(MethodInfo method)
            {
                var parameters = method.GetParameters();
                return parameters.Length == 0;
            }

            static ImmutableArray<IImportScopeWrapper> FallbackAccessor(SemanticModel semanticModel, int position, CancellationToken cancellationToken)
            {
                return ImmutableArray<IImportScopeWrapper>.Empty;
            }
        }
    }
}
