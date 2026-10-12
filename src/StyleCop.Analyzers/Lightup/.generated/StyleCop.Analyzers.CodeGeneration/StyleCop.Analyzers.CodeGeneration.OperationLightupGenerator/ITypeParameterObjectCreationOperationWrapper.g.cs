// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ITypeParameterObjectCreationOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ITypeParameterObjectCreationOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> InitializerAccessor;
        private readonly IOperation operation;
        static ITypeParameterObjectCreationOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ITypeParameterObjectCreationOperationWrapper));
            InitializerAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Initializer));
        }

        private ITypeParameterObjectCreationOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IObjectOrCollectionInitializerOperationWrapper Initializer => IObjectOrCollectionInitializerOperationWrapper.FromOperation(InitializerAccessor(this.WrappedOperation));

        public static explicit operator ITypeParameterObjectCreationOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(ITypeParameterObjectCreationOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static ITypeParameterObjectCreationOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ITypeParameterObjectCreationOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
