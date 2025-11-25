// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class WrapperHelper
    {
        private static readonly ImmutableDictionary<Type, Type> WrappedTypes;
        private static ImmutableDictionary<Type, Func<object, object>> wrapInstance = ImmutableDictionary<Type, Func<object, object>>.Empty;

        static WrapperHelper()
        {
            var codeAnalysisAssembly = typeof(SyntaxNode).GetTypeInfo().Assembly;
            var builder = ImmutableDictionary.CreateBuilder<Type, Type>();

            builder.Add(typeof(AnalyzerConfigOptionsProviderWrapper), codeAnalysisAssembly.GetType(AnalyzerConfigOptionsProviderWrapper.WrappedTypeName));
            builder.Add(typeof(AnalyzerConfigOptionsWrapper), codeAnalysisAssembly.GetType(AnalyzerConfigOptionsWrapper.WrappedTypeName));
            builder.Add(typeof(IImportScopeWrapper), codeAnalysisAssembly.GetType(IImportScopeWrapper.WrappedTypeName));

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

        internal static object Wrap(object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var wrapperMethod = ImmutableInterlocked.GetOrAdd(
                ref wrapInstance,
                value.GetType(),
                static type =>
                {
                    foreach (var pair in WrappedTypes)
                    {
                        if (pair.Value.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                        {
                            var fromObjectMethod = pair.Key.GetTypeInfo().GetDeclaredMethod("FromObject");
                            var parameterType = fromObjectMethod.GetParameters()[0].ParameterType;
                            var instanceParameter = Expression.Parameter(fromObjectMethod.GetParameters()[0].ParameterType, "instance");
                            Expression value =
                                fromObjectMethod.GetParameters()[0].ParameterType.GetTypeInfo().IsAssignableFrom(typeof(object).GetTypeInfo())
                                ? (Expression)instanceParameter
                                : Expression.Convert(instanceParameter, parameterType);
                            Expression<Func<object, object>> expression =
                                Expression.Lambda<Func<object, object>>(
                                    Expression.Convert(Expression.Call(null, fromObjectMethod, instanceParameter), typeof(object)),
                                    instanceParameter);
                            return expression.Compile();
                        }
                    }

                    return _ => throw new NotSupportedException();
                });

            return wrapperMethod(value);
        }
    }
}
