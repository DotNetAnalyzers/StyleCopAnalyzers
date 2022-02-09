// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;

    internal interface IOperationWrapper
    {
        IOperation? WrappedOperation { get; }

        ////IOperationWrapper Parent { get; }

        ////OperationKind Kind { get; }

        ////SyntaxNode Syntax { get; }

        ITypeSymbol? Type { get; }

        ////Optional<object> ConstantValue { get; }

        ////IEnumerable<IOperationWrapper> Children { get; }

        ////string Language { get; }

        ////bool IsImplicit { get; }

        ////SemanticModel SemanticModel { get; }
    }
}
