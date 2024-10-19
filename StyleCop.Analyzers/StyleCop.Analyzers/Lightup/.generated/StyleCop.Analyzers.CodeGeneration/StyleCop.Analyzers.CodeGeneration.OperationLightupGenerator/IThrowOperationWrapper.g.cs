﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IThrowOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IThrowOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> ExceptionAccessor;
        private readonly IOperation operation;
        static IThrowOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IThrowOperationWrapper));
            ExceptionAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Exception));
        }

        private IThrowOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Exception => ExceptionAccessor(this.WrappedOperation);
        public static explicit operator IThrowOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IThrowOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IThrowOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IThrowOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
