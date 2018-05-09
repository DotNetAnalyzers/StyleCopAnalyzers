// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class LightupHelpers
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<SyntaxKind, bool>> SupportedWrappers
            = new ConcurrentDictionary<Type, ConcurrentDictionary<SyntaxKind, bool>>();

        public static bool SupportsCSharp7 { get; }
            = Enum.GetNames(typeof(SyntaxKind)).Contains(nameof(SyntaxKindEx.IsPatternExpression));

        internal static bool CanWrapNode(SyntaxNode node, Type underlyingType)
        {
            if (node == null)
            {
                // The wrappers support a null instance
                return true;
            }

            if (underlyingType == null)
            {
                // The current runtime doesn't define the target type of the conversion, so no instance of it can exist
                return false;
            }

            ConcurrentDictionary<SyntaxKind, bool> wrappedSyntax = SupportedWrappers.GetOrAdd(underlyingType, _ => new ConcurrentDictionary<SyntaxKind, bool>());

            // Avoid creating the delegate if the value already exists
            bool canCast;
            if (!wrappedSyntax.TryGetValue(node.Kind(), out canCast))
            {
                canCast = wrappedSyntax.GetOrAdd(
                    node.Kind(),
                    kind => underlyingType.GetTypeInfo().IsAssignableFrom(node.GetType().GetTypeInfo()));
            }

            return canCast;
        }

        internal static Func<TSyntax, TProperty> CreateSyntaxPropertyAccessor<TSyntax, TProperty>(Type type, string propertyName)
        {
            Func<TSyntax, TProperty> fallbackAccessor =
                syntax =>
                {
                    if (syntax == null)
                    {
                        // Unlike an extension method which would throw ArgumentNullException here, the light-up
                        // behavior needs to match behavior of the underlying property.
                        throw new NullReferenceException();
                    }

                    return default(TProperty);
                };

            if (type == null)
            {
                return fallbackAccessor;
            }

            if (!typeof(TSyntax).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null)
            {
                return fallbackAccessor;
            }

            if (!typeof(TProperty).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var syntaxParameter = Expression.Parameter(typeof(TSyntax), "syntax");
            Expression instance =
                type.GetTypeInfo().IsAssignableFrom(typeof(TSyntax).GetTypeInfo())
                ? (Expression)syntaxParameter
                : Expression.Convert(syntaxParameter, type);

            Expression<Func<TSyntax, TProperty>> expression =
                Expression.Lambda<Func<TSyntax, TProperty>>(
                    Expression.Call(instance, property.GetMethod),
                    syntaxParameter);
            return expression.Compile();
        }

        internal static Func<TSyntax, SeparatedSyntaxListWrapper<TProperty>> CreateSeparatedSyntaxListPropertyAccessor<TSyntax, TProperty>(Type type, string propertyName)
        {
            Func<TSyntax, SeparatedSyntaxListWrapper<TProperty>> fallbackAccessor =
                syntax =>
                {
                    if (syntax == null)
                    {
                        // Unlike an extension method which would throw ArgumentNullException here, the light-up
                        // behavior needs to match behavior of the underlying property.
                        throw new NullReferenceException();
                    }

                    return SeparatedSyntaxListWrapper<TProperty>.UnsupportedEmpty;
                };

            if (type == null)
            {
                return fallbackAccessor;
            }

            if (!typeof(TSyntax).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null)
            {
                return fallbackAccessor;
            }

            if (property.PropertyType.GetGenericTypeDefinition() != typeof(SeparatedSyntaxList<>))
            {
                throw new InvalidOperationException();
            }

            var propertySyntaxType = property.PropertyType.GenericTypeArguments[0];

            if (!ValidatePropertyType(typeof(TProperty), propertySyntaxType))
            {
                throw new InvalidOperationException();
            }

            var syntaxParameter = Expression.Parameter(typeof(TSyntax), "syntax");
            Expression instance =
                type.GetTypeInfo().IsAssignableFrom(typeof(TSyntax).GetTypeInfo())
                ? (Expression)syntaxParameter
                : Expression.Convert(syntaxParameter, type);
            Expression propertyAccess = Expression.Call(instance, property.GetMethod);

            var unboundWrapperType = typeof(SeparatedSyntaxListWrapper<>.AutoWrapSeparatedSyntaxList<>);
            var boundWrapperType = unboundWrapperType.MakeGenericType(typeof(TProperty), propertySyntaxType);
            var constructorInfo = boundWrapperType.GetTypeInfo().DeclaredConstructors.Single();

            Expression<Func<TSyntax, SeparatedSyntaxListWrapper<TProperty>>> expression =
                Expression.Lambda<Func<TSyntax, SeparatedSyntaxListWrapper<TProperty>>>(
                    Expression.New(constructorInfo, propertyAccess),
                    syntaxParameter);
            return expression.Compile();
        }

        internal static Func<TSyntax, TProperty, TSyntax> CreateSyntaxWithPropertyAccessor<TSyntax, TProperty>(Type type, string propertyName)
        {
            Func<TSyntax, TProperty, TSyntax> fallbackAccessor =
                (syntax, newValue) =>
                {
                    if (syntax == null)
                    {
                        // Unlike an extension method which would throw ArgumentNullException here, the light-up
                        // behavior needs to match behavior of the underlying property.
                        throw new NullReferenceException();
                    }

                    if (Equals(newValue, default(TProperty)))
                    {
                        return syntax;
                    }

                    throw new NotSupportedException();
                };

            if (type == null)
            {
                return fallbackAccessor;
            }

            if (!typeof(TSyntax).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null)
            {
                return fallbackAccessor;
            }

            if (!typeof(TProperty).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var methodInfo = type.GetTypeInfo().GetDeclaredMethods("With" + propertyName)
                .Single(m => !m.IsStatic && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.Equals(property.PropertyType));

            var syntaxParameter = Expression.Parameter(typeof(TSyntax), "syntax");
            var valueParameter = Expression.Parameter(typeof(TProperty), methodInfo.GetParameters()[0].Name);
            Expression instance =
                type.GetTypeInfo().IsAssignableFrom(typeof(TSyntax).GetTypeInfo())
                ? (Expression)syntaxParameter
                : Expression.Convert(syntaxParameter, type);
            Expression value =
                property.PropertyType.GetTypeInfo().IsAssignableFrom(typeof(TProperty).GetTypeInfo())
                ? (Expression)valueParameter
                : Expression.Convert(valueParameter, property.PropertyType);

            Expression<Func<TSyntax, TProperty, TSyntax>> expression =
                Expression.Lambda<Func<TSyntax, TProperty, TSyntax>>(
                    Expression.Call(instance, methodInfo, value),
                    syntaxParameter,
                    valueParameter);
            return expression.Compile();
        }

        internal static Func<TSyntax, SeparatedSyntaxListWrapper<TProperty>, TSyntax> CreateSeparatedSyntaxListWithPropertyAccessor<TSyntax, TProperty>(Type type, string propertyName)
        {
            Func<TSyntax, SeparatedSyntaxListWrapper<TProperty>, TSyntax> fallbackAccessor =
                (syntax, newValue) =>
                {
                    if (syntax == null)
                    {
                        // Unlike an extension method which would throw ArgumentNullException here, the light-up
                        // behavior needs to match behavior of the underlying property.
                        throw new NullReferenceException();
                    }

                    if (newValue is null)
                    {
                        return syntax;
                    }

                    throw new NotSupportedException();
                };

            if (type == null)
            {
                return fallbackAccessor;
            }

            if (!typeof(TSyntax).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null)
            {
                return fallbackAccessor;
            }

            if (property.PropertyType.GetGenericTypeDefinition() != typeof(SeparatedSyntaxList<>))
            {
                throw new InvalidOperationException();
            }

            var propertySyntaxType = property.PropertyType.GenericTypeArguments[0];

            if (!ValidatePropertyType(typeof(TProperty), propertySyntaxType))
            {
                throw new InvalidOperationException();
            }

            var methodInfo = type.GetTypeInfo().GetDeclaredMethods("With" + propertyName)
                .Single(m => !m.IsStatic && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.Equals(property.PropertyType));

            var syntaxParameter = Expression.Parameter(typeof(TSyntax), "syntax");
            var valueParameter = Expression.Parameter(typeof(SeparatedSyntaxListWrapper<TProperty>), methodInfo.GetParameters()[0].Name);
            Expression instance =
                type.GetTypeInfo().IsAssignableFrom(typeof(TSyntax).GetTypeInfo())
                ? (Expression)syntaxParameter
                : Expression.Convert(syntaxParameter, type);

            var underlyingListProperty = typeof(SeparatedSyntaxListWrapper<TProperty>).GetTypeInfo().GetDeclaredProperty(nameof(SeparatedSyntaxListWrapper<TProperty>.UnderlyingList));
            Expression value = Expression.Convert(
                Expression.Call(valueParameter, underlyingListProperty.GetMethod),
                property.PropertyType);

            Expression<Func<TSyntax, SeparatedSyntaxListWrapper<TProperty>, TSyntax>> expression =
                Expression.Lambda<Func<TSyntax, SeparatedSyntaxListWrapper<TProperty>, TSyntax>>(
                    Expression.Call(instance, methodInfo, value),
                    syntaxParameter,
                    valueParameter);
            return expression.Compile();
        }

        private static bool ValidatePropertyType(Type returnType, Type actualType)
        {
            var requiredType = WrapperHelper.GetWrappedType(returnType) ?? returnType;
            return requiredType == actualType;
        }
    }
}
