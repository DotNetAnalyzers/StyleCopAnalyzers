// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IPointerIndirectionReferenceOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IPointerIndirectionReferenceOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> PointerAccessor;
        private readonly IOperation operation;
        static IPointerIndirectionReferenceOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IPointerIndirectionReferenceOperationWrapper));
            PointerAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Pointer));
        }

        private IPointerIndirectionReferenceOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Pointer => PointerAccessor(this.WrappedOperation);
        public static IPointerIndirectionReferenceOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IPointerIndirectionReferenceOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
