// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IInterpolationOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IInterpolationOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> ExpressionAccessor;
        private static readonly Func<IOperation, IOperation> AlignmentAccessor;
        private static readonly Func<IOperation, IOperation> FormatStringAccessor;
        private readonly IOperation operation;
        static IInterpolationOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IInterpolationOperationWrapper));
            ExpressionAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Expression));
            AlignmentAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Alignment));
            FormatStringAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(FormatString));
        }

        private IInterpolationOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Expression => ExpressionAccessor(this.WrappedOperation);
        public IOperation Alignment => AlignmentAccessor(this.WrappedOperation);
        public IOperation FormatString => FormatStringAccessor(this.WrappedOperation);

        public static explicit operator IInterpolationOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IInterpolationOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static explicit operator IInterpolationOperationWrapper(IInterpolatedStringContentOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IInterpolatedStringContentOperationWrapper(IInterpolationOperationWrapper wrapper) => IInterpolatedStringContentOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IInterpolationOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IInterpolationOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
