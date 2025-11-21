// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IVariableDeclarationOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IVariableDeclarationOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> DeclaratorsAccessor;
        private static readonly Func<IOperation, IOperation> InitializerAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> IgnoredDimensionsAccessor;
        private readonly IOperation operation;
        static IVariableDeclarationOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IVariableDeclarationOperationWrapper));
            DeclaratorsAccessor = LightupHelpers.CreateOperationListPropertyAccessor<IOperation>(WrappedType, nameof(Declarators));
            InitializerAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Initializer));
            IgnoredDimensionsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IOperation>>(WrappedType, nameof(IgnoredDimensions));
        }

        private IVariableDeclarationOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ImmutableArray<IOperation> Declarators => DeclaratorsAccessor(this.WrappedOperation);
        public IVariableInitializerOperationWrapper Initializer => IVariableInitializerOperationWrapper.FromOperation(InitializerAccessor(this.WrappedOperation));
        public ImmutableArray<IOperation> IgnoredDimensions => IgnoredDimensionsAccessor(this.WrappedOperation);
        public static explicit operator IVariableDeclarationOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IVariableDeclarationOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IVariableDeclarationOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IVariableDeclarationOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
