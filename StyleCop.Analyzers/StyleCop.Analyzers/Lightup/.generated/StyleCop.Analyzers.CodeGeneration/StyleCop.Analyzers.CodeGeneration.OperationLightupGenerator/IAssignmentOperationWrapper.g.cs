// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IAssignmentOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IAssignmentOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> TargetAccessor;
        private static readonly Func<IOperation, IOperation> ValueAccessor;
        private readonly IOperation operation;
        static IAssignmentOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IAssignmentOperationWrapper));
            TargetAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Target));
            ValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Value));
        }

        private IAssignmentOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Target => TargetAccessor(this.WrappedOperation);
        public IOperation Value => ValueAccessor(this.WrappedOperation);
        public static explicit operator IAssignmentOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IAssignmentOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IAssignmentOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IAssignmentOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }

        internal static IAssignmentOperationWrapper FromUpcast(IOperation operation)
        {
            return new IAssignmentOperationWrapper(operation);
        }
    }
}
