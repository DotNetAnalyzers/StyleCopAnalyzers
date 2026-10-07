// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal static class LightupHelpers
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, bool>> SupportedObjectWrappers
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, bool>>();

        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<SyntaxKind, bool>> SupportedSyntaxWrappers
            = new ConcurrentDictionary<Type, ConcurrentDictionary<SyntaxKind, bool>>();

        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<OperationKind, bool>> SupportedOperationWrappers
            = new ConcurrentDictionary<Type, ConcurrentDictionary<OperationKind, bool>>();

        public static bool SupportsCSharp7 { get; }
            = Enum.GetNames(typeof(LanguageVersion)).Contains(nameof(LanguageVersionEx.CSharp7));

        public static bool SupportsCSharp71 { get; }
            = Enum.GetNames(typeof(LanguageVersion)).Contains(nameof(LanguageVersionEx.CSharp7_1));

        public static bool SupportsCSharp72 { get; }
            = Enum.GetNames(typeof(LanguageVersion)).Contains(nameof(LanguageVersionEx.CSharp7_2));

        public static bool SupportsCSharp73 { get; }
            = Enum.GetNames(typeof(LanguageVersion)).Contains(nameof(LanguageVersionEx.CSharp7_3));

        public static bool SupportsCSharp8 { get; }
            = Enum.GetNames(typeof(LanguageVersion)).Contains(nameof(LanguageVersionEx.CSharp8));

        public static bool SupportsCSharp9 { get; }
            = Enum.GetNames(typeof(LanguageVersion)).Contains(nameof(LanguageVersionEx.CSharp9));

        public static bool SupportsCSharp10 { get; }
            = Enum.GetNames(typeof(LanguageVersion)).Contains(nameof(LanguageVersionEx.CSharp10));

        public static bool SupportsCSharp11 { get; }
            = Enum.GetNames(typeof(LanguageVersion)).Contains(nameof(LanguageVersionEx.CSharp11));

        public static bool SupportsCSharp12 { get; }
            = Enum.GetNames(typeof(LanguageVersion)).Contains(nameof(LanguageVersionEx.CSharp12));

        public static bool SupportsCSharp13 { get; }
            = Enum.GetNames(typeof(LanguageVersion)).Contains(nameof(LanguageVersionEx.CSharp13));

        public static bool SupportsCSharp14 { get; }
            = Enum.GetNames(typeof(LanguageVersion)).Contains(nameof(LanguageVersionEx.CSharp14));

        public static bool SupportsIOperation => SupportsCSharp73;

        internal static bool CanWrapObject(object obj, Type underlyingType)
        {
            if (obj == null)
            {
                // The wrappers support a null instance
                return true;
            }

            if (underlyingType == null)
            {
                // The current runtime doesn't define the target type of the conversion, so no instance of it can exist
                return false;
            }

            ConcurrentDictionary<Type, bool> wrappedObject = SupportedObjectWrappers.GetOrAdd(underlyingType, static _ => new ConcurrentDictionary<Type, bool>());

            // Avoid creating a delegate and capture class
            if (!wrappedObject.TryGetValue(obj.GetType(), out var canCast))
            {
                canCast = underlyingType.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());
                wrappedObject.TryAdd(obj.GetType(), canCast);
            }

            return canCast;
        }

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

            ConcurrentDictionary<SyntaxKind, bool> wrappedSyntax = SupportedSyntaxWrappers.GetOrAdd(underlyingType, static _ => new ConcurrentDictionary<SyntaxKind, bool>());

            // Avoid creating a delegate and capture class
            if (!wrappedSyntax.TryGetValue(node.Kind(), out var canCast))
            {
                canCast = underlyingType.GetTypeInfo().IsAssignableFrom(node.GetType().GetTypeInfo());
                wrappedSyntax.TryAdd(node.Kind(), canCast);
            }

            return canCast;
        }

        internal static bool CanWrapOperation(IOperation operation, Type underlyingType)
        {
            if (operation == null)
            {
                // The wrappers support a null instance
                return true;
            }

            if (underlyingType == null)
            {
                // The current runtime doesn't define the target type of the conversion, so no instance of it can exist
                return false;
            }

            ConcurrentDictionary<OperationKind, bool> wrappedSyntax = SupportedOperationWrappers.GetOrAdd(underlyingType, static _ => new ConcurrentDictionary<OperationKind, bool>());

            // Avoid creating a delegate and capture class
            if (!wrappedSyntax.TryGetValue(operation.Kind, out var canCast))
            {
                canCast = underlyingType.GetTypeInfo().IsAssignableFrom(operation.GetType().GetTypeInfo());
                wrappedSyntax.TryAdd(operation.Kind, canCast);
            }

            return canCast;
        }

        internal static Func<TOperation, TProperty> CreateOperationPropertyAccessor<TOperation, TProperty>(Type type, string propertyName)
        {
            TProperty FallbackAccessor(TOperation syntax)
            {
                if (syntax == null)
                {
                    // Unlike an extension method which would throw ArgumentNullException here, the light-up
                    // behavior needs to match behavior of the underlying property.
                    throw new NullReferenceException();
                }

                return default;
            }

            if (type == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(TOperation).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(TProperty).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var operationParameter = Expression.Parameter(typeof(TOperation), "operation");
            Expression instance =
                type.GetTypeInfo().IsAssignableFrom(typeof(TOperation).GetTypeInfo())
                ? (Expression)operationParameter
                : Expression.Convert(operationParameter, type);

            Expression<Func<TOperation, TProperty>> expression =
                Expression.Lambda<Func<TOperation, TProperty>>(
                    Expression.Call(instance, property.GetMethod),
                    operationParameter);
            return expression.Compile();
        }

        internal static Func<TOperation, ImmutableArray<IOperation>> CreateOperationListPropertyAccessor<TOperation>(Type type, string propertyName)
        {
            ImmutableArray<IOperation> FallbackAccessor(TOperation syntax)
            {
                if (syntax == null)
                {
                    // Unlike an extension method which would throw ArgumentNullException here, the light-up
                    // behavior needs to match behavior of the underlying property.
                    throw new NullReferenceException();
                }

                return ImmutableArray<IOperation>.Empty;
            }

            if (type == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(TOperation).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null)
            {
                return FallbackAccessor;
            }

            if (property.PropertyType.GetGenericTypeDefinition() != typeof(ImmutableArray<>))
            {
                throw new InvalidOperationException();
            }

            var propertyOperationType = property.PropertyType.GenericTypeArguments[0];

            if (!typeof(IOperation).GetTypeInfo().IsAssignableFrom(propertyOperationType.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var syntaxParameter = Expression.Parameter(typeof(TOperation), "syntax");
            Expression instance =
                type.GetTypeInfo().IsAssignableFrom(typeof(TOperation).GetTypeInfo())
                ? (Expression)syntaxParameter
                : Expression.Convert(syntaxParameter, type);
            Expression propertyAccess = Expression.Call(instance, property.GetMethod);

            var unboundCastUpMethod = typeof(ImmutableArray<IOperation>).GetTypeInfo().GetDeclaredMethod(nameof(ImmutableArray<IOperation>.CastUp));
            var boundCastUpMethod = unboundCastUpMethod.MakeGenericMethod(propertyOperationType);

            Expression<Func<TOperation, ImmutableArray<IOperation>>> expression =
                Expression.Lambda<Func<TOperation, ImmutableArray<IOperation>>>(
                    Expression.Call(boundCastUpMethod, propertyAccess),
                    syntaxParameter);
            return expression.Compile();
        }

        internal static Func<TProperty> CreateStaticPropertyAccessor<TProperty>(Type type, string propertyName)
        {
            static TProperty FallbackAccessor()
            {
                return default;
            }

            if (type == null)
            {
                return FallbackAccessor;
            }

            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(TProperty).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            Expression<Func<TProperty>> expression =
                Expression.Lambda<Func<TProperty>>(
                    Expression.Call(null, property.GetMethod));
            return expression.Compile();
        }

        internal static Func<TSyntax, TProperty> CreateSyntaxPropertyAccessor<TSyntax, TProperty>(Type type, string propertyName)
        {
            TProperty FallbackAccessor(TSyntax syntax)
            {
                if (syntax == null)
                {
                    // Unlike an extension method which would throw ArgumentNullException here, the light-up
                    // behavior needs to match behavior of the underlying property.
                    throw new NullReferenceException();
                }

                return default;
            }

            if (type == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(TSyntax).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null)
            {
                return FallbackAccessor;
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

        internal static Func<TSyntax, TArg, TProperty> CreateSyntaxPropertyAccessor<TSyntax, TArg, TProperty>(Type type, Type argumentType, string accessorMethodName)
        {
            static TProperty FallbackAccessor(TSyntax syntax, TArg argument)
            {
                if (syntax == null)
                {
                    // Unlike an extension method which would throw ArgumentNullException here, the light-up
                    // behavior needs to match behavior of the underlying property.
                    throw new NullReferenceException();
                }

                return default;
            }

            if (type == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(TSyntax).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            if (!typeof(TArg).GetTypeInfo().IsAssignableFrom(argumentType.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var methods = type.GetTypeInfo().GetDeclaredMethods(accessorMethodName);
            MethodInfo method = null;
            foreach (var candidate in methods)
            {
                var parameters = candidate.GetParameters();
                if (parameters.Length != 1)
                {
                    continue;
                }

                if (Equals(argumentType, parameters[0].ParameterType))
                {
                    method = candidate;
                    break;
                }
            }

            if (method == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(TProperty).GetTypeInfo().IsAssignableFrom(method.ReturnType.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var syntaxParameter = Expression.Parameter(typeof(TSyntax), "syntax");
            var argParameter = Expression.Parameter(typeof(TArg), "arg");
            Expression instance =
                type.GetTypeInfo().IsAssignableFrom(typeof(TSyntax).GetTypeInfo())
                ? (Expression)syntaxParameter
                : Expression.Convert(syntaxParameter, type);
            Expression argument =
                argumentType.GetTypeInfo().IsAssignableFrom(typeof(TArg).GetTypeInfo())
                ? (Expression)argParameter
                : Expression.Convert(argParameter, argumentType);

            Expression<Func<TSyntax, TArg, TProperty>> expression =
                Expression.Lambda<Func<TSyntax, TArg, TProperty>>(
                    Expression.Call(instance, method, argument),
                    syntaxParameter,
                    argParameter);
            return expression.Compile();
        }

        internal static TryGetValueAccessor<TSyntax, TKey, TValue> CreateTryGetValueAccessor<TSyntax, TKey, TValue>(Type type, Type keyType, string methodName)
        {
            static bool FallbackAccessor(TSyntax syntax, TKey key, out TValue value)
            {
                if (syntax == null)
                {
                    // Unlike an extension method which would throw ArgumentNullException here, the light-up
                    // behavior needs to match behavior of the underlying property.
                    throw new NullReferenceException();
                }

                value = default;
                return false;
            }

            if (type == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(TSyntax).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            if (!typeof(TKey).GetTypeInfo().IsAssignableFrom(keyType.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var methods = type.GetTypeInfo().GetDeclaredMethods(methodName);
            MethodInfo method = null;
            foreach (var candidate in methods)
            {
                var parameters = candidate.GetParameters();
                if (parameters.Length != 2)
                {
                    continue;
                }

                if (Equals(keyType, parameters[0].ParameterType)
                    && Equals(typeof(TValue).MakeByRefType(), parameters[1].ParameterType))
                {
                    method = candidate;
                    break;
                }
            }

            if (method == null)
            {
                return FallbackAccessor;
            }

            if (method.ReturnType != typeof(bool))
            {
                throw new InvalidOperationException();
            }

            var syntaxParameter = Expression.Parameter(typeof(TSyntax), "syntax");
            var keyParameter = Expression.Parameter(typeof(TKey), "key");
            var valueParameter = Expression.Parameter(typeof(TValue).MakeByRefType(), "value");
            Expression instance =
                type.GetTypeInfo().IsAssignableFrom(typeof(TSyntax).GetTypeInfo())
                ? (Expression)syntaxParameter
                : Expression.Convert(syntaxParameter, type);
            Expression key =
                keyType.GetTypeInfo().IsAssignableFrom(typeof(TKey).GetTypeInfo())
                ? (Expression)keyParameter
                : Expression.Convert(keyParameter, keyType);

            Expression<TryGetValueAccessor<TSyntax, TKey, TValue>> expression =
                Expression.Lambda<TryGetValueAccessor<TSyntax, TKey, TValue>>(
                    Expression.Call(instance, method, key, valueParameter),
                    syntaxParameter,
                    keyParameter,
                    valueParameter);
            return expression.Compile();
        }

        internal static Func<T, TArg1, TArg2, ImmutableArrayWrapper<TResult>> CreateImmutableArrayMethodAccessor<T, TArg1, TArg2, TResult>(Type type, Type argumentType1, Type argumentType2, string methodName)
        {
            ImmutableArrayWrapper<TResult> FallbackAccessor(T instance, TArg1 arg1, TArg2 arg2)
            {
                if (instance == null)
                {
                    // Unlike an extension method which would throw ArgumentNullException here, the light-up
                    // behavior needs to match behavior of the underlying property.
                    throw new NullReferenceException();
                }

                return ImmutableArrayWrapper<TResult>.UnsupportedDefault;
            }

            if (type == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            if (!typeof(TArg1).GetTypeInfo().IsAssignableFrom(argumentType1.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            if (!typeof(TArg2).GetTypeInfo().IsAssignableFrom(argumentType2.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var methods = type.GetTypeInfo().GetDeclaredMethods(methodName);
            MethodInfo method = null;
            foreach (var candidate in methods)
            {
                var parameters = candidate.GetParameters();
                if (parameters.Length != 2)
                {
                    continue;
                }

                if (Equals(argumentType1, parameters[0].ParameterType)
                    && Equals(argumentType2, parameters[1].ParameterType))
                {
                    method = candidate;
                    break;
                }
            }

            if (method == null)
            {
                return FallbackAccessor;
            }

            if (method.ReturnType.GetGenericTypeDefinition() != typeof(ImmutableArray<>))
            {
                throw new InvalidOperationException();
            }

            var methodResultType = method.ReturnType.GenericTypeArguments[0];
            if (!ValidatePropertyType(typeof(TResult), methodResultType))
            {
                throw new InvalidOperationException();
            }

            var instanceParameter = Expression.Parameter(typeof(T), "instance");
            var arg1Parameter = Expression.Parameter(typeof(TArg1), "arg1");
            var arg2Parameter = Expression.Parameter(typeof(TArg2), "arg2");
            Expression instance =
                type.GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo())
                ? (Expression)instanceParameter
                : Expression.Convert(instanceParameter, type);
            Expression argument1 =
                argumentType1.GetTypeInfo().IsAssignableFrom(typeof(TArg1).GetTypeInfo())
                ? (Expression)arg1Parameter
                : Expression.Convert(arg1Parameter, argumentType1);
            Expression argument2 =
                argumentType2.GetTypeInfo().IsAssignableFrom(typeof(TArg2).GetTypeInfo())
                ? (Expression)arg2Parameter
                : Expression.Convert(arg2Parameter, argumentType2);

            Expression methodAccess = Expression.Call(instance, method, argument1, argument2);

            var unboundWrapperType = typeof(ImmutableArrayWrapper<>.AutoWrapImmutableArray<>);
            var boundWrapperType = unboundWrapperType.MakeGenericType(typeof(TResult), methodResultType);
            var constructorInfo = boundWrapperType.GetTypeInfo().DeclaredConstructors.Single(constructor => constructor.GetParameters().Length == 1);

            Expression<Func<T, TArg1, TArg2, ImmutableArrayWrapper<TResult>>> expression =
                Expression.Lambda<Func<T, TArg1, TArg2, ImmutableArrayWrapper<TResult>>>(
                    Expression.New(constructorInfo, methodAccess),
                    instanceParameter,
                    arg1Parameter,
                    arg2Parameter);
            return expression.Compile();
        }

        internal static Func<TSyntax, SeparatedSyntaxListWrapper<TProperty>> CreateSeparatedSyntaxListPropertyAccessor<TSyntax, TProperty>(Type type, string propertyName)
        {
            SeparatedSyntaxListWrapper<TProperty> FallbackAccessor(TSyntax syntax)
            {
                if (syntax == null)
                {
                    // Unlike an extension method which would throw ArgumentNullException here, the light-up
                    // behavior needs to match behavior of the underlying property.
                    throw new NullReferenceException();
                }

                return SeparatedSyntaxListWrapper<TProperty>.UnsupportedEmpty;
            }

            if (type == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(TSyntax).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null)
            {
                return FallbackAccessor;
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
            var constructorInfo = boundWrapperType.GetTypeInfo().DeclaredConstructors.Single(constructor => constructor.GetParameters().Length == 1);

            Expression<Func<TSyntax, SeparatedSyntaxListWrapper<TProperty>>> expression =
                Expression.Lambda<Func<TSyntax, SeparatedSyntaxListWrapper<TProperty>>>(
                    Expression.New(constructorInfo, propertyAccess),
                    syntaxParameter);
            return expression.Compile();
        }

        internal static Func<TSyntax, TProperty, TSyntax> CreateSyntaxWithPropertyAccessor<TSyntax, TProperty>(Type type, string propertyName)
        {
            TSyntax FallbackAccessor(TSyntax syntax, TProperty newValue)
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
            }

            if (type == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(TSyntax).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(TProperty).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var methodInfo = type.GetTypeInfo().GetDeclaredMethods("With" + propertyName)
                .SingleOrDefault(m => !m.IsStatic && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.Equals(property.PropertyType));
            if (methodInfo is null)
            {
                return FallbackAccessor;
            }

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
            TSyntax FallbackAccessor(TSyntax syntax, SeparatedSyntaxListWrapper<TProperty> newValue)
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
            }

            if (type == null)
            {
                return FallbackAccessor;
            }

            if (!typeof(TSyntax).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new InvalidOperationException();
            }

            var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            if (property == null)
            {
                return FallbackAccessor;
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
            var requiredType = SyntaxWrapperHelper.GetWrappedType(returnType)
                ?? OperationWrapperHelper.GetWrappedType(returnType)
                ?? WrapperHelper.GetWrappedType(returnType)
                ?? returnType;
            return requiredType == actualType;
        }
    }
}
