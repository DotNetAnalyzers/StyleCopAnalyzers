// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IIncrementOrDecrementOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IIncrementOrDecrementOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, bool> IsPostfixAccessor;
        private static readonly Func<IOperation, bool> IsLiftedAccessor;
        private static readonly Func<IOperation, bool> IsCheckedAccessor;
        private static readonly Func<IOperation, IOperation> TargetAccessor;
        private static readonly Func<IOperation, IMethodSymbol> OperatorMethodAccessor;
        private readonly IOperation operation;
        static IIncrementOrDecrementOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IIncrementOrDecrementOperationWrapper));
            IsPostfixAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsPostfix));
            IsLiftedAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsLifted));
            IsCheckedAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsChecked));
            TargetAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Target));
            OperatorMethodAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(OperatorMethod));
        }

        private IIncrementOrDecrementOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public bool IsPostfix => IsPostfixAccessor(this.WrappedOperation);
        public bool IsLifted => IsLiftedAccessor(this.WrappedOperation);
        public bool IsChecked => IsCheckedAccessor(this.WrappedOperation);
        public IOperation Target => TargetAccessor(this.WrappedOperation);
        public IMethodSymbol OperatorMethod => OperatorMethodAccessor(this.WrappedOperation);

        public static explicit operator IIncrementOrDecrementOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IIncrementOrDecrementOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IIncrementOrDecrementOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IIncrementOrDecrementOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
