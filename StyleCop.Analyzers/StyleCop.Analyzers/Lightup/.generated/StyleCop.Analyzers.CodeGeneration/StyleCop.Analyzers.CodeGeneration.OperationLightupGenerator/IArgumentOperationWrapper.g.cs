// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IArgumentOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IArgumentOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IParameterSymbol> ParameterAccessor;
        private static readonly Func<IOperation, IOperation> ValueAccessor;
        private readonly IOperation operation;
        static IArgumentOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IArgumentOperationWrapper));
            ParameterAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IParameterSymbol>(WrappedType, nameof(Parameter));
            ValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Value));
        }

        private IArgumentOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public object ArgumentKind => throw new NotImplementedException("Property 'IArgumentOperation.ArgumentKind' has unsupported type 'ArgumentKind'");
        public IParameterSymbol Parameter => ParameterAccessor(this.WrappedOperation);
        public IOperation Value => ValueAccessor(this.WrappedOperation);
        public object InConversion => throw new NotImplementedException("Property 'IArgumentOperation.InConversion' has unsupported type 'CommonConversion'");
        public object OutConversion => throw new NotImplementedException("Property 'IArgumentOperation.OutConversion' has unsupported type 'CommonConversion'");
        public static explicit operator IArgumentOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IArgumentOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IArgumentOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IArgumentOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
