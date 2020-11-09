// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IMemberInitializerOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IMemberInitializerOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> InitializedMemberAccessor;
        private static readonly Func<IOperation, IOperation> InitializerAccessor;
        private readonly IOperation operation;
        static IMemberInitializerOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IMemberInitializerOperationWrapper));
            InitializedMemberAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(InitializedMember));
            InitializerAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Initializer));
        }

        private IMemberInitializerOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation InitializedMember => InitializedMemberAccessor(this.WrappedOperation);
        public IObjectOrCollectionInitializerOperationWrapper Initializer => IObjectOrCollectionInitializerOperationWrapper.FromOperation(InitializerAccessor(this.WrappedOperation));
        public static IMemberInitializerOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IMemberInitializerOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
