// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IPropertyReferenceOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IPropertyReferenceOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IPropertySymbol> PropertyAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> ArgumentsAccessor;
        private readonly IOperation operation;
        static IPropertyReferenceOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IPropertyReferenceOperationWrapper));
            PropertyAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IPropertySymbol>(WrappedType, nameof(Property));
            ArgumentsAccessor = LightupHelpers.CreateOperationListPropertyAccessor<IOperation>(WrappedType, nameof(Arguments));
        }

        private IPropertyReferenceOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IPropertySymbol Property => PropertyAccessor(this.WrappedOperation);
        public ImmutableArray<IOperation> Arguments => ArgumentsAccessor(this.WrappedOperation);
        public static explicit operator IPropertyReferenceOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IPropertyReferenceOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public IOperation Instance => ((IMemberReferenceOperationWrapper)this).Instance;
        public ISymbol Member => ((IMemberReferenceOperationWrapper)this).Member;
        public static explicit operator IPropertyReferenceOperationWrapper(IMemberReferenceOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IMemberReferenceOperationWrapper(IPropertyReferenceOperationWrapper wrapper) => IMemberReferenceOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IPropertyReferenceOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IPropertyReferenceOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
