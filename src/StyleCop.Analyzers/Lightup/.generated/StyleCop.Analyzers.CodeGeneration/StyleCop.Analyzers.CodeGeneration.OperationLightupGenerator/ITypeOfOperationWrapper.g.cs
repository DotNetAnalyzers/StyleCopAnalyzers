// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    internal readonly struct ITypeOfOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ITypeOfOperation";
        private static readonly Type? WrappedType;
        private static readonly Func<IOperation, ITypeSymbol> TypeOperandAccessor;
        private readonly IOperation? operation;
        static ITypeOfOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ITypeOfOperationWrapper));
            TypeOperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ITypeSymbol>(WrappedType, nameof(TypeOperand));
        }

        private ITypeOfOperationWrapper(IOperation? operation)
        {
            this.operation = operation;
        }

        public IOperation? WrappedOperation => this.operation;
        public ITypeSymbol? Type => this.operation!.Type;
        public ITypeSymbol TypeOperand => TypeOperandAccessor(this.operation!);

        public static explicit operator ITypeOfOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(ITypeOfOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static ITypeOfOperationWrapper FromOperation(IOperation? operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ITypeOfOperationWrapper(operation);
        }

        public static bool IsInstance([NotNullWhen(true)] IOperation? operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
