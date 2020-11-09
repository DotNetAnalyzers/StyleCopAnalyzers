// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ITupleOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ITupleOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> ElementsAccessor;
        private static readonly Func<IOperation, ITypeSymbol> NaturalTypeAccessor;
        private readonly IOperation operation;
        static ITupleOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ITupleOperationWrapper));
            ElementsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IOperation>>(WrappedType, nameof(Elements));
            NaturalTypeAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ITypeSymbol>(WrappedType, nameof(NaturalType));
        }

        private ITupleOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ImmutableArray<IOperation> Elements => ElementsAccessor(this.WrappedOperation);
        public ITypeSymbol NaturalType => NaturalTypeAccessor(this.WrappedOperation);
        public static ITupleOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ITupleOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
