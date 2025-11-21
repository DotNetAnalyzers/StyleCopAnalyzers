// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IStaticLocalInitializationSemaphoreOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.FlowAnalysis.IStaticLocalInitializationSemaphoreOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ILocalSymbol> LocalAccessor;
        private readonly IOperation operation;
        static IStaticLocalInitializationSemaphoreOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IStaticLocalInitializationSemaphoreOperationWrapper));
            LocalAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ILocalSymbol>(WrappedType, nameof(Local));
        }

        private IStaticLocalInitializationSemaphoreOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ILocalSymbol Local => LocalAccessor(this.WrappedOperation);
        public static explicit operator IStaticLocalInitializationSemaphoreOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IStaticLocalInitializationSemaphoreOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IStaticLocalInitializationSemaphoreOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IStaticLocalInitializationSemaphoreOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
