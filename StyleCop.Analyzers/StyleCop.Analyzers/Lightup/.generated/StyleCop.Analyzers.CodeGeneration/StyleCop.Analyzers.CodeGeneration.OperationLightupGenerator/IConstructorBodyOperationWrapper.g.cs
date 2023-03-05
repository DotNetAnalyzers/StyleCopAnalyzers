// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IConstructorBodyOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IConstructorBodyOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ImmutableArray<ILocalSymbol>> LocalsAccessor;
        private static readonly Func<IOperation, IOperation> InitializerAccessor;
        private readonly IOperation operation;
        static IConstructorBodyOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IConstructorBodyOperationWrapper));
            LocalsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<ILocalSymbol>>(WrappedType, nameof(Locals));
            InitializerAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Initializer));
        }

        private IConstructorBodyOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ImmutableArray<ILocalSymbol> Locals => LocalsAccessor(this.WrappedOperation);
        public IOperation Initializer => InitializerAccessor(this.WrappedOperation);
        public static explicit operator IConstructorBodyOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IConstructorBodyOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public IBlockOperationWrapper BlockBody => ((IMethodBodyBaseOperationWrapper)this).BlockBody;
        public IBlockOperationWrapper ExpressionBody => ((IMethodBodyBaseOperationWrapper)this).ExpressionBody;
        public static explicit operator IConstructorBodyOperationWrapper(IMethodBodyBaseOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IMethodBodyBaseOperationWrapper(IConstructorBodyOperationWrapper wrapper) => IMethodBodyBaseOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IConstructorBodyOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IConstructorBodyOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
