// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ITryOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ITryOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> BodyAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> CatchesAccessor;
        private static readonly Func<IOperation, IOperation> FinallyAccessor;
        private static readonly Func<IOperation, ILabelSymbol> ExitLabelAccessor;
        private readonly IOperation operation;
        static ITryOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ITryOperationWrapper));
            BodyAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Body));
            CatchesAccessor = LightupHelpers.CreateOperationListPropertyAccessor<IOperation>(WrappedType, nameof(Catches));
            FinallyAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Finally));
            ExitLabelAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ILabelSymbol>(WrappedType, nameof(ExitLabel));
        }

        private ITryOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IBlockOperationWrapper Body => IBlockOperationWrapper.FromOperation(BodyAccessor(this.WrappedOperation));
        public ImmutableArray<IOperation> Catches => CatchesAccessor(this.WrappedOperation);
        public IBlockOperationWrapper Finally => IBlockOperationWrapper.FromOperation(FinallyAccessor(this.WrappedOperation));
        public ILabelSymbol ExitLabel => ExitLabelAccessor(this.WrappedOperation);
        public static ITryOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ITryOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
