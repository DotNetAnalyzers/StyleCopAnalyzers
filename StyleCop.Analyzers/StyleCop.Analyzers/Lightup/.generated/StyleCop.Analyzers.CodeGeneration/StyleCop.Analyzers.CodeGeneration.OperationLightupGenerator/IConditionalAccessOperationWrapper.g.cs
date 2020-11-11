// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IConditionalAccessOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IConditionalAccessOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> OperationAccessor;
        private static readonly Func<IOperation, IOperation> WhenNotNullAccessor;
        private readonly IOperation operation;
        static IConditionalAccessOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IConditionalAccessOperationWrapper));
            OperationAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Operation));
            WhenNotNullAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(WhenNotNull));
        }

        private IConditionalAccessOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Operation => OperationAccessor(this.WrappedOperation);
        public IOperation WhenNotNull => WhenNotNullAccessor(this.WrappedOperation);
        public static IConditionalAccessOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IConditionalAccessOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
