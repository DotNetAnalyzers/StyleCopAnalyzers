// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IInterpolatedStringOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IInterpolatedStringOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> PartsAccessor;
        private readonly IOperation operation;
        static IInterpolatedStringOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IInterpolatedStringOperationWrapper));
            PartsAccessor = LightupHelpers.CreateOperationListPropertyAccessor<IOperation>(WrappedType, nameof(Parts));
        }

        private IInterpolatedStringOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ImmutableArray<IOperation> Parts => PartsAccessor(this.WrappedOperation);

        public static explicit operator IInterpolatedStringOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IInterpolatedStringOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IInterpolatedStringOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IInterpolatedStringOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
