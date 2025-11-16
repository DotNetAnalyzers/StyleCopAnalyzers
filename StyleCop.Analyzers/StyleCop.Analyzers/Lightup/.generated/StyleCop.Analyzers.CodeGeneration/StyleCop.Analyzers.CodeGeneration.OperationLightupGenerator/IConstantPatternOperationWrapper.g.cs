// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IConstantPatternOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IConstantPatternOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> ValueAccessor;
        private readonly IOperation operation;
        static IConstantPatternOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IConstantPatternOperationWrapper));
            ValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Value));
        }

        private IConstantPatternOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Value => ValueAccessor(this.WrappedOperation);

        public static explicit operator IConstantPatternOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IConstantPatternOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public ITypeSymbol InputType => ((IPatternOperationWrapper)this).InputType;
        public ITypeSymbol NarrowedType => ((IPatternOperationWrapper)this).NarrowedType;

        public static explicit operator IConstantPatternOperationWrapper(IPatternOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IPatternOperationWrapper(IConstantPatternOperationWrapper wrapper) => IPatternOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IConstantPatternOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IConstantPatternOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
