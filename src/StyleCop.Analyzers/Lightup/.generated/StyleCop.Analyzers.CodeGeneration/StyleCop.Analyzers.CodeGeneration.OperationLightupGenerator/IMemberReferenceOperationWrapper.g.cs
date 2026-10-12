// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IMemberReferenceOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IMemberReferenceOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> InstanceAccessor;
        private static readonly Func<IOperation, ISymbol> MemberAccessor;
        private readonly IOperation operation;
        static IMemberReferenceOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IMemberReferenceOperationWrapper));
            InstanceAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Instance));
            MemberAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ISymbol>(WrappedType, nameof(Member));
        }

        private IMemberReferenceOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Instance => InstanceAccessor(this.WrappedOperation);
        public ISymbol Member => MemberAccessor(this.WrappedOperation);

        public static explicit operator IMemberReferenceOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IMemberReferenceOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IMemberReferenceOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IMemberReferenceOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }

        internal static IMemberReferenceOperationWrapper FromUpcast(IOperation operation)
        {
            return new IMemberReferenceOperationWrapper(operation);
        }
    }
}
