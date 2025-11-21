// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IFlowCaptureOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.FlowAnalysis.IFlowCaptureOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> ValueAccessor;
        private readonly IOperation operation;
        static IFlowCaptureOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IFlowCaptureOperationWrapper));
            ValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Value));
        }

        private IFlowCaptureOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public object Id => throw new NotImplementedException("Property 'IFlowCaptureOperation.Id' has unsupported type 'CaptureId'");
        public IOperation Value => ValueAccessor(this.WrappedOperation);
        public static explicit operator IFlowCaptureOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IFlowCaptureOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IFlowCaptureOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IFlowCaptureOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
