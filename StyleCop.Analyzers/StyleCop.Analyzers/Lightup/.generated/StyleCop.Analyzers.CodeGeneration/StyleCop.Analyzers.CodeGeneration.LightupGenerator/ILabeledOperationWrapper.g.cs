// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ILabeledOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ILabeledOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ILabelSymbol> LabelAccessor;
        private static readonly Func<IOperation, IOperation> OperationAccessor;
        private readonly IOperation operation;
        static ILabeledOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ILabeledOperationWrapper));
            LabelAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ILabelSymbol>(WrappedType, nameof(Label));
            OperationAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Operation));
        }

        private ILabeledOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ILabelSymbol Label => LabelAccessor(this.WrappedOperation);
        public IOperation Operation => OperationAccessor(this.WrappedOperation);
        public static ILabeledOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ILabeledOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
