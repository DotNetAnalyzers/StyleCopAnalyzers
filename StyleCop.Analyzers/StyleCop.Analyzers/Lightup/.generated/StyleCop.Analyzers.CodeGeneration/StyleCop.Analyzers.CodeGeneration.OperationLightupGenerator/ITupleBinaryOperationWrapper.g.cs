﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ITupleBinaryOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ITupleBinaryOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> LeftOperandAccessor;
        private static readonly Func<IOperation, IOperation> RightOperandAccessor;
        private readonly IOperation operation;
        static ITupleBinaryOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ITupleBinaryOperationWrapper));
            LeftOperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(LeftOperand));
            RightOperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(RightOperand));
        }

        private ITupleBinaryOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public object OperatorKind => throw new NotImplementedException("Property 'ITupleBinaryOperation.OperatorKind' has unsupported type 'BinaryOperatorKind'");
        public IOperation LeftOperand => LeftOperandAccessor(this.WrappedOperation);
        public IOperation RightOperand => RightOperandAccessor(this.WrappedOperation);
        public static explicit operator ITupleBinaryOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(ITupleBinaryOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static ITupleBinaryOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ITupleBinaryOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
