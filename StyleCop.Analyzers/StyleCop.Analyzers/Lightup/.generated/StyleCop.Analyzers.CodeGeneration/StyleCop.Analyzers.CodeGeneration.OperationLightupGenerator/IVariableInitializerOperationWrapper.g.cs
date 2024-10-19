// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IVariableInitializerOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IVariableInitializerOperation";
        private static readonly Type WrappedType;
        private readonly IOperation operation;
        static IVariableInitializerOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IVariableInitializerOperationWrapper));
        }

        private IVariableInitializerOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public static explicit operator IVariableInitializerOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IVariableInitializerOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public ImmutableArray<ILocalSymbol> Locals => ((ISymbolInitializerOperationWrapper)this).Locals;
        public IOperation Value => ((ISymbolInitializerOperationWrapper)this).Value;
        public static explicit operator IVariableInitializerOperationWrapper(ISymbolInitializerOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator ISymbolInitializerOperationWrapper(IVariableInitializerOperationWrapper wrapper) => ISymbolInitializerOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IVariableInitializerOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IVariableInitializerOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
