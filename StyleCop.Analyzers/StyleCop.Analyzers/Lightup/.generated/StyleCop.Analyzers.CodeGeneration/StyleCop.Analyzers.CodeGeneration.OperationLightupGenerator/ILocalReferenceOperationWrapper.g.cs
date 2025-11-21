// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ILocalReferenceOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ILocalReferenceOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ILocalSymbol> LocalAccessor;
        private static readonly Func<IOperation, bool> IsDeclarationAccessor;
        private readonly IOperation operation;
        static ILocalReferenceOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ILocalReferenceOperationWrapper));
            LocalAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ILocalSymbol>(WrappedType, nameof(Local));
            IsDeclarationAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsDeclaration));
        }

        private ILocalReferenceOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ILocalSymbol Local => LocalAccessor(this.WrappedOperation);
        public bool IsDeclaration => IsDeclarationAccessor(this.WrappedOperation);
        public static explicit operator ILocalReferenceOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(ILocalReferenceOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static ILocalReferenceOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ILocalReferenceOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
