﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IDiscardPatternOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IDiscardPatternOperation";
        private static readonly Type WrappedType;
        private readonly IOperation operation;
        static IDiscardPatternOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IDiscardPatternOperationWrapper));
        }

        private IDiscardPatternOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public static explicit operator IDiscardPatternOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IDiscardPatternOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public ITypeSymbol InputType => ((IPatternOperationWrapper)this).InputType;
        public ITypeSymbol NarrowedType => ((IPatternOperationWrapper)this).NarrowedType;
        public static explicit operator IDiscardPatternOperationWrapper(IPatternOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IPatternOperationWrapper(IDiscardPatternOperationWrapper wrapper) => IPatternOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IDiscardPatternOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IDiscardPatternOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
