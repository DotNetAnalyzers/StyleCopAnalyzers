// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ClassOrStructConstraintSyntaxExtensions
    {
        private static readonly Func<ClassOrStructConstraintSyntax, SyntaxToken> QuestionTokenAccessor;
        private static readonly Func<ClassOrStructConstraintSyntax, SyntaxToken, ClassOrStructConstraintSyntax> WithQuestionTokenAccessor;

        static ClassOrStructConstraintSyntaxExtensions()
        {
            QuestionTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ClassOrStructConstraintSyntax, SyntaxToken>(typeof(ClassOrStructConstraintSyntax), nameof(QuestionToken));
            WithQuestionTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ClassOrStructConstraintSyntax, SyntaxToken>(typeof(ClassOrStructConstraintSyntax), nameof(QuestionToken));
        }

        public static SyntaxToken QuestionToken(this ClassOrStructConstraintSyntax syntax)
        {
            return QuestionTokenAccessor(syntax);
        }

        public static ClassOrStructConstraintSyntax WithQuestionToken(this ClassOrStructConstraintSyntax syntax, SyntaxToken questionToken)
        {
            return WithQuestionTokenAccessor(syntax, questionToken);
        }
    }
}
