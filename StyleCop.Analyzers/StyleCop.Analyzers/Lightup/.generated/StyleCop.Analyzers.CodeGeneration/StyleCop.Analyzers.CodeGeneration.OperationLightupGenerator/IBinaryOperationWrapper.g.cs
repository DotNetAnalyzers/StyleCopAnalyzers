// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IBinaryOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IBinaryOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> LeftOperandAccessor;
        private static readonly Func<IOperation, IOperation> RightOperandAccessor;
        private static readonly Func<IOperation, bool> IsLiftedAccessor;
        private static readonly Func<IOperation, bool> IsCheckedAccessor;
        private static readonly Func<IOperation, bool> IsCompareTextAccessor;
        private static readonly Func<IOperation, IMethodSymbol> OperatorMethodAccessor;
        private readonly IOperation operation;
        static IBinaryOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IBinaryOperationWrapper));
            LeftOperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(LeftOperand));
            RightOperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(RightOperand));
            IsLiftedAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsLifted));
            IsCheckedAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsChecked));
            IsCompareTextAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsCompareText));
            OperatorMethodAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(OperatorMethod));
        }

        private IBinaryOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public object OperatorKind => throw new NotImplementedException("Property 'IBinaryOperation.OperatorKind' has unsupported type 'BinaryOperatorKind'");
        public IOperation LeftOperand => LeftOperandAccessor(this.WrappedOperation);
        public IOperation RightOperand => RightOperandAccessor(this.WrappedOperation);
        public bool IsLifted => IsLiftedAccessor(this.WrappedOperation);
        public bool IsChecked => IsCheckedAccessor(this.WrappedOperation);
        public bool IsCompareText => IsCompareTextAccessor(this.WrappedOperation);
        public IMethodSymbol OperatorMethod => OperatorMethodAccessor(this.WrappedOperation);
        public static explicit operator IBinaryOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IBinaryOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IBinaryOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IBinaryOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
