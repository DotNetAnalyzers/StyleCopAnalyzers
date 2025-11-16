// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ITypePatternOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ITypePatternOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ITypeSymbol> MatchedTypeAccessor;
        private readonly IOperation operation;
        static ITypePatternOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ITypePatternOperationWrapper));
            MatchedTypeAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ITypeSymbol>(WrappedType, nameof(MatchedType));
        }

        private ITypePatternOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ITypeSymbol MatchedType => MatchedTypeAccessor(this.WrappedOperation);

        public static explicit operator ITypePatternOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(ITypePatternOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public ITypeSymbol InputType => ((IPatternOperationWrapper)this).InputType;
        public ITypeSymbol NarrowedType => ((IPatternOperationWrapper)this).NarrowedType;

        public static explicit operator ITypePatternOperationWrapper(IPatternOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IPatternOperationWrapper(ITypePatternOperationWrapper wrapper) => IPatternOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static ITypePatternOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ITypePatternOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
