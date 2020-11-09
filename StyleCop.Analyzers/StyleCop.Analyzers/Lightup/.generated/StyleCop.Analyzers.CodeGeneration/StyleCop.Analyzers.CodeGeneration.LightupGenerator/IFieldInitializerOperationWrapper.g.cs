// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IFieldInitializerOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IFieldInitializerOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ImmutableArray<IFieldSymbol>> InitializedFieldsAccessor;
        private readonly IOperation operation;
        static IFieldInitializerOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IFieldInitializerOperationWrapper));
            InitializedFieldsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IFieldSymbol>>(WrappedType, nameof(InitializedFields));
        }

        private IFieldInitializerOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ImmutableArray<IFieldSymbol> InitializedFields => InitializedFieldsAccessor(this.WrappedOperation);
        public ImmutableArray<ILocalSymbol> Locals => ((ISymbolInitializerOperationWrapper)this).Locals;
        public IOperation Value => ((ISymbolInitializerOperationWrapper)this).Value;
        public static explicit operator IFieldInitializerOperationWrapper(ISymbolInitializerOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator ISymbolInitializerOperationWrapper(IFieldInitializerOperationWrapper wrapper) => ISymbolInitializerOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IFieldInitializerOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IFieldInitializerOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
