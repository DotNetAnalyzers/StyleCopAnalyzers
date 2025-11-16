// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IPatternOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IPatternOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ITypeSymbol> InputTypeAccessor;
        private static readonly Func<IOperation, ITypeSymbol> NarrowedTypeAccessor;
        private readonly IOperation operation;
        static IPatternOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IPatternOperationWrapper));
            InputTypeAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ITypeSymbol>(WrappedType, nameof(InputType));
            NarrowedTypeAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ITypeSymbol>(WrappedType, nameof(NarrowedType));
        }

        private IPatternOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ITypeSymbol InputType => InputTypeAccessor(this.WrappedOperation);
        public ITypeSymbol NarrowedType => NarrowedTypeAccessor(this.WrappedOperation);

        public static explicit operator IPatternOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IPatternOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IPatternOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IPatternOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }

        internal static IPatternOperationWrapper FromUpcast(IOperation operation)
        {
            return new IPatternOperationWrapper(operation);
        }
    }
}
