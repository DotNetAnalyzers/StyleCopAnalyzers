// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup;

using System;
using Microsoft.CodeAnalysis;
using CodeAnalysisTypeInfo = Microsoft.CodeAnalysis.TypeInfo;

internal static class TypeInfoExtensions
{
    private static readonly Func<CodeAnalysisTypeInfo, NullabilityInfo> ConvertedNullabilityAccessor;
    private static readonly Func<CodeAnalysisTypeInfo, NullabilityInfo> NullabilityAccessor;

    static TypeInfoExtensions()
    {
        ConvertedNullabilityAccessor = LightupHelpers.CreateNullabilityInfoAccessor<CodeAnalysisTypeInfo>(typeof(CodeAnalysisTypeInfo), nameof(ConvertedNullability));
        NullabilityAccessor = LightupHelpers.CreateNullabilityInfoAccessor<CodeAnalysisTypeInfo>(typeof(CodeAnalysisTypeInfo), nameof(Nullability));
    }

    public static NullabilityInfo ConvertedNullability(this CodeAnalysisTypeInfo typeInfo)
    {
        return ConvertedNullabilityAccessor(typeInfo);
    }

    public static NullabilityInfo Nullability(this CodeAnalysisTypeInfo typeInfo)
    {
        return NullabilityAccessor(typeInfo);
    }
}
