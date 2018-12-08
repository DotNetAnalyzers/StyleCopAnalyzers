// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class SpecialTypeHelper
    {
        private static ImmutableDictionary<SpecialType, PredefinedTypeSyntax> PredefinedSpecialTypes { get; } =
            new Dictionary<SpecialType, PredefinedTypeSyntax>
            {
                [SpecialType.System_Boolean] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                [SpecialType.System_Byte] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword)),
                [SpecialType.System_Char] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.CharKeyword)),
                [SpecialType.System_Decimal] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword)),
                [SpecialType.System_Double] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword)),
                [SpecialType.System_Int16] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ShortKeyword)),
                [SpecialType.System_Int32] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                [SpecialType.System_Int64] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword)),
                [SpecialType.System_Object] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                [SpecialType.System_SByte] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.SByteKeyword)),
                [SpecialType.System_Single] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)),
                [SpecialType.System_String] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                [SpecialType.System_UInt16] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword)),
                [SpecialType.System_UInt32] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntKeyword)),
                [SpecialType.System_UInt64] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ULongKeyword)),
            }.ToImmutableDictionary();

        public static bool IsPredefinedType(SpecialType specialType)
        {
            return PredefinedSpecialTypes.ContainsKey(specialType);
        }

        public static bool TryGetPredefinedType(SpecialType specialType, out PredefinedTypeSyntax predefinedTypeSyntax)
        {
            return PredefinedSpecialTypes.TryGetValue(specialType, out predefinedTypeSyntax);
        }
    }
}
