// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IConversionOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IConversionOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> OperandAccessor;
        private static readonly Func<IOperation, IMethodSymbol> OperatorMethodAccessor;
        private static readonly Func<IOperation, bool> IsTryCastAccessor;
        private static readonly Func<IOperation, bool> IsCheckedAccessor;
        private readonly IOperation operation;
        static IConversionOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IConversionOperationWrapper));
            OperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Operand));
            OperatorMethodAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(OperatorMethod));
            IsTryCastAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsTryCast));
            IsCheckedAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsChecked));
        }

        private IConversionOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Operand => OperandAccessor(this.WrappedOperation);
        public IMethodSymbol OperatorMethod => OperatorMethodAccessor(this.WrappedOperation);
        public object Conversion => throw new NotImplementedException("Property 'IConversionOperation.Conversion' has unsupported type 'CommonConversion'");
        public bool IsTryCast => IsTryCastAccessor(this.WrappedOperation);
        public bool IsChecked => IsCheckedAccessor(this.WrappedOperation);
        public static explicit operator IConversionOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IConversionOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IConversionOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IConversionOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
