// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IObjectCreationOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IObjectCreationOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IMethodSymbol> ConstructorAccessor;
        private static readonly Func<IOperation, IOperation> InitializerAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> ArgumentsAccessor;
        private readonly IOperation operation;
        static IObjectCreationOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IObjectCreationOperationWrapper));
            ConstructorAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(Constructor));
            InitializerAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Initializer));
            ArgumentsAccessor = LightupHelpers.CreateOperationListPropertyAccessor<IOperation>(WrappedType, nameof(Arguments));
        }

        private IObjectCreationOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IMethodSymbol Constructor => ConstructorAccessor(this.WrappedOperation);
        public IObjectOrCollectionInitializerOperationWrapper Initializer => IObjectOrCollectionInitializerOperationWrapper.FromOperation(InitializerAccessor(this.WrappedOperation));
        public ImmutableArray<IOperation> Arguments => ArgumentsAccessor(this.WrappedOperation);
        public static explicit operator IObjectCreationOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IObjectCreationOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IObjectCreationOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IObjectCreationOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
