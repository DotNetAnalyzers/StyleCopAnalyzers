// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IFlowCaptureReferenceOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.FlowAnalysis.IFlowCaptureReferenceOperation";
        private static readonly Type WrappedType;
        private readonly IOperation operation;
        static IFlowCaptureReferenceOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IFlowCaptureReferenceOperationWrapper));
        }

        private IFlowCaptureReferenceOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public object Id => throw new NotImplementedException("Property 'IFlowCaptureReferenceOperation.Id' has unsupported type 'CaptureId'");

        public static explicit operator IFlowCaptureReferenceOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IFlowCaptureReferenceOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IFlowCaptureReferenceOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IFlowCaptureReferenceOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
