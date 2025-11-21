// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IMethodBodyBaseOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IMethodBodyBaseOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> BlockBodyAccessor;
        private static readonly Func<IOperation, IOperation> ExpressionBodyAccessor;
        private readonly IOperation operation;
        static IMethodBodyBaseOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IMethodBodyBaseOperationWrapper));
            BlockBodyAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(BlockBody));
            ExpressionBodyAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(ExpressionBody));
        }

        private IMethodBodyBaseOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IBlockOperationWrapper BlockBody => IBlockOperationWrapper.FromOperation(BlockBodyAccessor(this.WrappedOperation));
        public IBlockOperationWrapper ExpressionBody => IBlockOperationWrapper.FromOperation(ExpressionBodyAccessor(this.WrappedOperation));
        public static explicit operator IMethodBodyBaseOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IMethodBodyBaseOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IMethodBodyBaseOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IMethodBodyBaseOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }

        internal static IMethodBodyBaseOperationWrapper FromUpcast(IOperation operation)
        {
            return new IMethodBodyBaseOperationWrapper(operation);
        }
    }
}
