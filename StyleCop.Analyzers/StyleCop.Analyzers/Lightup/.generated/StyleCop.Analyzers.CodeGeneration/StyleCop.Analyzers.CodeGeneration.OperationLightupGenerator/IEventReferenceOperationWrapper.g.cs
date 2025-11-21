// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IEventReferenceOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IEventReferenceOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IEventSymbol> EventAccessor;
        private readonly IOperation operation;
        static IEventReferenceOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IEventReferenceOperationWrapper));
            EventAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IEventSymbol>(WrappedType, nameof(Event));
        }

        private IEventReferenceOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IEventSymbol Event => EventAccessor(this.WrappedOperation);
        public static explicit operator IEventReferenceOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IEventReferenceOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public IOperation Instance => ((IMemberReferenceOperationWrapper)this).Instance;
        public ISymbol Member => ((IMemberReferenceOperationWrapper)this).Member;
        public static explicit operator IEventReferenceOperationWrapper(IMemberReferenceOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IMemberReferenceOperationWrapper(IEventReferenceOperationWrapper wrapper) => IMemberReferenceOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IEventReferenceOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IEventReferenceOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
