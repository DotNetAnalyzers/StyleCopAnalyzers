// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IArrayInitializerOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IArrayInitializerOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> ElementValuesAccessor;
        private readonly IOperation operation;
        static IArrayInitializerOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IArrayInitializerOperationWrapper));
            ElementValuesAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IOperation>>(WrappedType, nameof(ElementValues));
        }

        private IArrayInitializerOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ImmutableArray<IOperation> ElementValues => ElementValuesAccessor(this.WrappedOperation);
        public static IArrayInitializerOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IArrayInitializerOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
