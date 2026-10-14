// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    internal readonly struct IInterpolatedStringTextOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IInterpolatedStringTextOperation";
        private static readonly Type? WrappedType;
        private static readonly Func<IOperation, IOperation> TextAccessor;
        private readonly IOperation? operation;
        static IInterpolatedStringTextOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IInterpolatedStringTextOperationWrapper));
            TextAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Text));
        }

        private IInterpolatedStringTextOperationWrapper(IOperation? operation)
        {
            this.operation = operation;
        }

        public IOperation? WrappedOperation => this.operation;
        public ITypeSymbol? Type => this.operation!.Type;
        public IOperation Text => TextAccessor(this.operation!);

        public static explicit operator IInterpolatedStringTextOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IInterpolatedStringTextOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static explicit operator IInterpolatedStringTextOperationWrapper(IInterpolatedStringContentOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IInterpolatedStringContentOperationWrapper(IInterpolatedStringTextOperationWrapper wrapper) => IInterpolatedStringContentOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IInterpolatedStringTextOperationWrapper FromOperation(IOperation? operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IInterpolatedStringTextOperationWrapper(operation);
        }

        public static bool IsInstance([NotNullWhen(true)] IOperation? operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
