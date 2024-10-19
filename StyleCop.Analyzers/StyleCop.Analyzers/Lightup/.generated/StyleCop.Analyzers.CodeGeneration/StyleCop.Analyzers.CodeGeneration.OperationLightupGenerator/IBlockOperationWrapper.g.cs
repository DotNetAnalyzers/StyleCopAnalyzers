﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IBlockOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IBlockOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> OperationsAccessor;
        private static readonly Func<IOperation, ImmutableArray<ILocalSymbol>> LocalsAccessor;
        private readonly IOperation operation;
        static IBlockOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IBlockOperationWrapper));
            OperationsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IOperation>>(WrappedType, nameof(Operations));
            LocalsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<ILocalSymbol>>(WrappedType, nameof(Locals));
        }

        private IBlockOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ImmutableArray<IOperation> Operations => OperationsAccessor(this.WrappedOperation);
        public ImmutableArray<ILocalSymbol> Locals => LocalsAccessor(this.WrappedOperation);
        public static explicit operator IBlockOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IBlockOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IBlockOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IBlockOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
