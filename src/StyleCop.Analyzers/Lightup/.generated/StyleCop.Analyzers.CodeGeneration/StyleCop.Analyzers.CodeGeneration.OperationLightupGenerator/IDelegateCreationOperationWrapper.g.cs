// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    internal readonly struct IDelegateCreationOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IDelegateCreationOperation";
        private static readonly Type? WrappedType;
        private static readonly Func<IOperation, IOperation> TargetAccessor;
        private readonly IOperation? operation;
        static IDelegateCreationOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IDelegateCreationOperationWrapper));
            TargetAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Target));
        }

        private IDelegateCreationOperationWrapper(IOperation? operation)
        {
            this.operation = operation;
        }

        public IOperation? WrappedOperation => this.operation;
        public ITypeSymbol? Type => this.operation!.Type;
        public IOperation Target => TargetAccessor(this.operation!);

        public static explicit operator IDelegateCreationOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IDelegateCreationOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IDelegateCreationOperationWrapper FromOperation(IOperation? operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IDelegateCreationOperationWrapper(operation);
        }

        public static bool IsInstance([NotNullWhen(true)] IOperation? operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
