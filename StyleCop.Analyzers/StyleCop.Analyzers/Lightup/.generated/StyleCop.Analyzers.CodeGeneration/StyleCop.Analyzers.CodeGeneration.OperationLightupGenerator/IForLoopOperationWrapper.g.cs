// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IForLoopOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IForLoopOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> BeforeAccessor;
        private static readonly Func<IOperation, ImmutableArray<ILocalSymbol>> ConditionLocalsAccessor;
        private static readonly Func<IOperation, IOperation> ConditionAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> AtLoopBottomAccessor;
        private readonly IOperation operation;
        static IForLoopOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IForLoopOperationWrapper));
            BeforeAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IOperation>>(WrappedType, nameof(Before));
            ConditionLocalsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<ILocalSymbol>>(WrappedType, nameof(ConditionLocals));
            ConditionAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Condition));
            AtLoopBottomAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IOperation>>(WrappedType, nameof(AtLoopBottom));
        }

        private IForLoopOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ImmutableArray<IOperation> Before => BeforeAccessor(this.WrappedOperation);
        public ImmutableArray<ILocalSymbol> ConditionLocals => ConditionLocalsAccessor(this.WrappedOperation);
        public IOperation Condition => ConditionAccessor(this.WrappedOperation);
        public ImmutableArray<IOperation> AtLoopBottom => AtLoopBottomAccessor(this.WrappedOperation);
        public static explicit operator IForLoopOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IForLoopOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public object LoopKind => ((ILoopOperationWrapper)this).LoopKind;
        public IOperation Body => ((ILoopOperationWrapper)this).Body;
        public ImmutableArray<ILocalSymbol> Locals => ((ILoopOperationWrapper)this).Locals;
        public ILabelSymbol ContinueLabel => ((ILoopOperationWrapper)this).ContinueLabel;
        public ILabelSymbol ExitLabel => ((ILoopOperationWrapper)this).ExitLabel;
        public static explicit operator IForLoopOperationWrapper(ILoopOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator ILoopOperationWrapper(IForLoopOperationWrapper wrapper) => ILoopOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IForLoopOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IForLoopOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
