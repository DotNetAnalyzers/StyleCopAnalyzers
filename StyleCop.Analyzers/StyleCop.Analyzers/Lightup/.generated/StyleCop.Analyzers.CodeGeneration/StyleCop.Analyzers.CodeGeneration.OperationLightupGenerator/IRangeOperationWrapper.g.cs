// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IRangeOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IRangeOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> LeftOperandAccessor;
        private static readonly Func<IOperation, IOperation> RightOperandAccessor;
        private static readonly Func<IOperation, bool> IsLiftedAccessor;
        private static readonly Func<IOperation, IMethodSymbol> MethodAccessor;
        private readonly IOperation operation;
        static IRangeOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IRangeOperationWrapper));
            LeftOperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(LeftOperand));
            RightOperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(RightOperand));
            IsLiftedAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsLifted));
            MethodAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(Method));
        }

        private IRangeOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation LeftOperand => LeftOperandAccessor(this.WrappedOperation);
        public IOperation RightOperand => RightOperandAccessor(this.WrappedOperation);
        public bool IsLifted => IsLiftedAccessor(this.WrappedOperation);
        public IMethodSymbol Method => MethodAccessor(this.WrappedOperation);
        public static explicit operator IRangeOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IRangeOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IRangeOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IRangeOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
