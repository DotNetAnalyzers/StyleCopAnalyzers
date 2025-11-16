// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IParameterReferenceOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IParameterReferenceOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IParameterSymbol> ParameterAccessor;
        private readonly IOperation operation;
        static IParameterReferenceOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IParameterReferenceOperationWrapper));
            ParameterAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IParameterSymbol>(WrappedType, nameof(Parameter));
        }

        private IParameterReferenceOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IParameterSymbol Parameter => ParameterAccessor(this.WrappedOperation);

        public static explicit operator IParameterReferenceOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IParameterReferenceOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IParameterReferenceOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IParameterReferenceOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
