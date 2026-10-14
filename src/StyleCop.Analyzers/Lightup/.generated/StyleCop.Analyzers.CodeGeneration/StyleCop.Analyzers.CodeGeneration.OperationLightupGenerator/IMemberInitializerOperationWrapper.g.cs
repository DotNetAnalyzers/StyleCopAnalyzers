// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    internal readonly struct IMemberInitializerOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IMemberInitializerOperation";
        private static readonly Type? WrappedType;
        private static readonly Func<IOperation, IOperation> InitializedMemberAccessor;
        private static readonly Func<IOperation, IOperation> InitializerAccessor;
        private readonly IOperation? operation;
        static IMemberInitializerOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IMemberInitializerOperationWrapper));
            InitializedMemberAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(InitializedMember));
            InitializerAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Initializer));
        }

        private IMemberInitializerOperationWrapper(IOperation? operation)
        {
            this.operation = operation;
        }

        public IOperation? WrappedOperation => this.operation;
        public ITypeSymbol? Type => this.operation!.Type;
        public IOperation InitializedMember => InitializedMemberAccessor(this.operation!);
        public IObjectOrCollectionInitializerOperationWrapper Initializer => IObjectOrCollectionInitializerOperationWrapper.FromOperation(InitializerAccessor(this.operation!));

        public static explicit operator IMemberInitializerOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IMemberInitializerOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IMemberInitializerOperationWrapper FromOperation(IOperation? operation)
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

        public static bool IsInstance([NotNullWhen(true)] IOperation? operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
