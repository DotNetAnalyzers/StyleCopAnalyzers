// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IParameterInitializerOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IParameterInitializerOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IParameterSymbol> ParameterAccessor;
        private readonly IOperation operation;
        static IParameterInitializerOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IParameterInitializerOperationWrapper));
            ParameterAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IParameterSymbol>(WrappedType, nameof(Parameter));
        }

        private IParameterInitializerOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IParameterSymbol Parameter => ParameterAccessor(this.WrappedOperation);
        public ImmutableArray<ILocalSymbol> Locals => ((ISymbolInitializerOperationWrapper)this).Locals;
        public IOperation Value => ((ISymbolInitializerOperationWrapper)this).Value;
        public static explicit operator IParameterInitializerOperationWrapper(ISymbolInitializerOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator ISymbolInitializerOperationWrapper(IParameterInitializerOperationWrapper wrapper) => ISymbolInitializerOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IParameterInitializerOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IParameterInitializerOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
