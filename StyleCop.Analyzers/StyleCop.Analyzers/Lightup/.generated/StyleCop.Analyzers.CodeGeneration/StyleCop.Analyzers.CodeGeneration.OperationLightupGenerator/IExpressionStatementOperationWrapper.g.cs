// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IExpressionStatementOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IExpressionStatementOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> OperationAccessor;
        private readonly IOperation operation;
        static IExpressionStatementOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IExpressionStatementOperationWrapper));
            OperationAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Operation));
        }

        private IExpressionStatementOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Operation => OperationAccessor(this.WrappedOperation);

        public static explicit operator IExpressionStatementOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IExpressionStatementOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IExpressionStatementOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IExpressionStatementOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
