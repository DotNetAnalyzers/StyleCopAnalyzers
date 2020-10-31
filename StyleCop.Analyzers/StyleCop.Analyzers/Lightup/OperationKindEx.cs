// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;

    internal static class OperationKindEx
    {
        /// <summary>
        /// Indicates an <see cref="T:Microsoft.CodeAnalysis.Operations.IObjectCreationOperation"/>.
        /// </summary>
        public const OperationKind ObjectCreation = (OperationKind)36;

        /// <summary>
        /// Indicates an <see cref="T:Microsoft.CodeAnalysis.Operations.ITypeParameterObjectCreationOperation"/>.
        /// </summary>
        public const OperationKind TypeParameterObjectCreation = (OperationKind)37;
    }
}
