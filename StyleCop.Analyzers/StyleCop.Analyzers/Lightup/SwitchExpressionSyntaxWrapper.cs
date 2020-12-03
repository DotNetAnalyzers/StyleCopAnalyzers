// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct SwitchExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        public SwitchExpressionSyntaxWrapper AddArms(params SwitchExpressionArmSyntaxWrapper[] arms)
        {
            return new SwitchExpressionSyntaxWrapper(this.WithArms(this.Arms.AddRange(arms)));
        }
    }
}
