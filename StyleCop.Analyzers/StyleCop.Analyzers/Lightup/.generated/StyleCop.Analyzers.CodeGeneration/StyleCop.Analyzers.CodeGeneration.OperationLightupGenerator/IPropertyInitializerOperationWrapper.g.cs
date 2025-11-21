// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IPropertyInitializerOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IPropertyInitializerOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ImmutableArray<IPropertySymbol>> InitializedPropertiesAccessor;
        private readonly IOperation operation;
        static IPropertyInitializerOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IPropertyInitializerOperationWrapper));
            InitializedPropertiesAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IPropertySymbol>>(WrappedType, nameof(InitializedProperties));
        }

        private IPropertyInitializerOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ImmutableArray<IPropertySymbol> InitializedProperties => InitializedPropertiesAccessor(this.WrappedOperation);
        public static explicit operator IPropertyInitializerOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IPropertyInitializerOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public ImmutableArray<ILocalSymbol> Locals => ((ISymbolInitializerOperationWrapper)this).Locals;
        public IOperation Value => ((ISymbolInitializerOperationWrapper)this).Value;
        public static explicit operator IPropertyInitializerOperationWrapper(ISymbolInitializerOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator ISymbolInitializerOperationWrapper(IPropertyInitializerOperationWrapper wrapper) => ISymbolInitializerOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IPropertyInitializerOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IPropertyInitializerOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
