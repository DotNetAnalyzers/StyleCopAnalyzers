// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CodeAnalysis;
using CodeAnalysisTypeInfo = Microsoft.CodeAnalysis.TypeInfo;

internal static class TypeInfoExtensions
{
    private static readonly Func<CodeAnalysisTypeInfo, NullabilityInfo> ConvertedNullabilityAccessor;
    private static readonly Func<CodeAnalysisTypeInfo, NullabilityInfo> NullabilityAccessor;

    static TypeInfoExtensions()
    {
        ConvertedNullabilityAccessor = CreateNullabilityAccessor(nameof(ConvertedNullability));
        NullabilityAccessor = CreateNullabilityAccessor(nameof(Nullability));
    }

    public static NullabilityInfo ConvertedNullability(this CodeAnalysisTypeInfo typeInfo)
    {
        return ConvertedNullabilityAccessor(typeInfo);
    }

    public static NullabilityInfo Nullability(this CodeAnalysisTypeInfo typeInfo)
    {
        return NullabilityAccessor(typeInfo);
    }

    private static Func<CodeAnalysisTypeInfo, NullabilityInfo> CreateNullabilityAccessor(string propertyName)
    {
        var property = typeof(CodeAnalysisTypeInfo).GetTypeInfo().GetDeclaredProperty(propertyName);
        if (property == null)
        {
            return static _ => default;
        }

        var nullabilityInfoType = property.PropertyType;
        var annotationProperty = nullabilityInfoType.GetTypeInfo().GetDeclaredProperty(nameof(NullabilityInfo.Annotation));
        var flowStateProperty = nullabilityInfoType.GetTypeInfo().GetDeclaredProperty(nameof(NullabilityInfo.FlowState));
        if (annotationProperty == null || flowStateProperty == null)
        {
            throw new InvalidOperationException();
        }

        var typeInfoParameter = Expression.Parameter(typeof(CodeAnalysisTypeInfo), "typeInfo");
        var nullabilityInfoVariable = Expression.Variable(nullabilityInfoType, "nullabilityInfo");
        var constructor = typeof(NullabilityInfo).GetTypeInfo().DeclaredConstructors
            .Single(c => c.GetParameters().Length == 2);
        var body = Expression.Block(
            new[] { nullabilityInfoVariable },
            Expression.Assign(nullabilityInfoVariable, Expression.Property(typeInfoParameter, property)),
            Expression.New(
                constructor,
                Expression.Convert(
                    Expression.Convert(Expression.Property(nullabilityInfoVariable, annotationProperty), typeof(int)),
                    typeof(NullableAnnotation)),
                Expression.Convert(
                    Expression.Convert(Expression.Property(nullabilityInfoVariable, flowStateProperty), typeof(int)),
                    typeof(NullableFlowState))));

        return Expression.Lambda<Func<CodeAnalysisTypeInfo, NullabilityInfo>>(body, typeInfoParameter).Compile();
    }
}
