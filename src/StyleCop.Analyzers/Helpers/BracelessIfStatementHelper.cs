// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Settings.ObjectModel;

internal static class BracelessIfStatementHelper
{
    internal static bool IsAllowed(StatementSyntax statement, BracelessIfStatements option)
    {
        if (option == BracelessIfStatements.Off
            || !(statement.Parent is IfStatementSyntax ifStatement)
            || ifStatement.Else != null
            || ifStatement.Parent.IsKind(SyntaxKind.ElseClause))
        {
            return false;
        }

        switch (statement.Kind())
        {
        case SyntaxKind.BreakStatement:
        case SyntaxKind.ContinueStatement:
        case SyntaxKind.ReturnStatement:
        case SyntaxKind.ThrowStatement:
        case SyntaxKind.GotoStatement:
        case SyntaxKind.GotoCaseStatement:
        case SyntaxKind.GotoDefaultStatement:
        case SyntaxKind.YieldBreakStatement:
            break;

        default:
            return false;
        }

        var lineSpan = statement.GetLineSpan();
        if (lineSpan.StartLinePosition.Line != lineSpan.EndLinePosition.Line)
        {
            return false;
        }

        var headerEndLine = ifStatement.CloseParenToken.GetEndLine();
        if (ifStatement.IfKeyword.GetLine() != headerEndLine)
        {
            return false;
        }

        return option == BracelessIfStatements.SameLine
            ? lineSpan.StartLinePosition.Line == headerEndLine
            : lineSpan.StartLinePosition.Line == headerEndLine + 1;
    }
}
