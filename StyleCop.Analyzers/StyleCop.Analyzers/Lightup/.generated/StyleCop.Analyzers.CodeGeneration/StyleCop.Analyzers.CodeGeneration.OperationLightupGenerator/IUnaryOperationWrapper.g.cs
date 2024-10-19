// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IUnaryOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IUnaryOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> OperandAccessor;
        private static readonly Func<IOperation, bool> IsLiftedAccessor;
        private static readonly Func<IOperation, bool> IsCheckedAccessor;
        private static readonly Func<IOperation, IMethodSymbol> OperatorMethodAccessor;
        private readonly IOperation operation;
        static IUnaryOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IUnaryOperationWrapper));
            OperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Operand));
            IsLiftedAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsLifted));
            IsCheckedAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsChecked));
            OperatorMethodAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(OperatorMethod));
        }

        private IUnaryOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public object OperatorKind => throw new NotImplementedException("Property 'IUnaryOperation.OperatorKind' has unsupported type 'UnaryOperatorKind'");
        public IOperation Operand => OperandAccessor(this.WrappedOperation);
        public bool IsLifted => IsLiftedAccessor(this.WrappedOperation);
        public bool IsChecked => IsCheckedAccessor(this.WrappedOperation);
        public IMethodSymbol OperatorMethod => OperatorMethodAccessor(this.WrappedOperation);
        public static explicit operator IUnaryOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IUnaryOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IUnaryOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IUnaryOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
