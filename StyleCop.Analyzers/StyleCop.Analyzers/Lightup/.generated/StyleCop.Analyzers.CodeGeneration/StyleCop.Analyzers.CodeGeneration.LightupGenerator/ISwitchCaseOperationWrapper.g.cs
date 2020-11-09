// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ISwitchCaseOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ISwitchCaseOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> ClausesAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> BodyAccessor;
        private static readonly Func<IOperation, ImmutableArray<ILocalSymbol>> LocalsAccessor;
        private readonly IOperation operation;
        static ISwitchCaseOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ISwitchCaseOperationWrapper));
            ClausesAccessor = LightupHelpers.CreateOperationListPropertyAccessor<IOperation>(WrappedType, nameof(Clauses));
            BodyAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IOperation>>(WrappedType, nameof(Body));
            LocalsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<ILocalSymbol>>(WrappedType, nameof(Locals));
        }

        private ISwitchCaseOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ImmutableArray<IOperation> Clauses => ClausesAccessor(this.WrappedOperation);
        public ImmutableArray<IOperation> Body => BodyAccessor(this.WrappedOperation);
        public ImmutableArray<ILocalSymbol> Locals => LocalsAccessor(this.WrappedOperation);
        public static ISwitchCaseOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ISwitchCaseOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
