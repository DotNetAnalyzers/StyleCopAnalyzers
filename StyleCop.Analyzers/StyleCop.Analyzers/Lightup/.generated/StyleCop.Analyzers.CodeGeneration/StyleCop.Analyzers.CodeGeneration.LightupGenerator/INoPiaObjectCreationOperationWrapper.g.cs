// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct INoPiaObjectCreationOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.INoPiaObjectCreationOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> InitializerAccessor;
        private readonly IOperation operation;
        static INoPiaObjectCreationOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(INoPiaObjectCreationOperationWrapper));
            InitializerAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Initializer));
        }

        private INoPiaObjectCreationOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IObjectOrCollectionInitializerOperationWrapper Initializer => IObjectOrCollectionInitializerOperationWrapper.FromOperation(InitializerAccessor(this.WrappedOperation));
        public static INoPiaObjectCreationOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new INoPiaObjectCreationOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
