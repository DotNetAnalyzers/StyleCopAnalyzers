// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IIsTypeOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IIsTypeOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> ValueOperandAccessor;
        private static readonly Func<IOperation, ITypeSymbol> TypeOperandAccessor;
        private static readonly Func<IOperation, bool> IsNegatedAccessor;
        private readonly IOperation operation;
        static IIsTypeOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IIsTypeOperationWrapper));
            ValueOperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(ValueOperand));
            TypeOperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ITypeSymbol>(WrappedType, nameof(TypeOperand));
            IsNegatedAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsNegated));
        }

        private IIsTypeOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation ValueOperand => ValueOperandAccessor(this.WrappedOperation);
        public ITypeSymbol TypeOperand => TypeOperandAccessor(this.WrappedOperation);
        public bool IsNegated => IsNegatedAccessor(this.WrappedOperation);
        public static explicit operator IIsTypeOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IIsTypeOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IIsTypeOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IIsTypeOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
