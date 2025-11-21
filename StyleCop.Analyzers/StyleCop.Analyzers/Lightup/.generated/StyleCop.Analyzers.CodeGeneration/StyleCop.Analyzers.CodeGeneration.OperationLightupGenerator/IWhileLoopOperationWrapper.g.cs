// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IWhileLoopOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IWhileLoopOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> ConditionAccessor;
        private static readonly Func<IOperation, bool> ConditionIsTopAccessor;
        private static readonly Func<IOperation, bool> ConditionIsUntilAccessor;
        private static readonly Func<IOperation, IOperation> IgnoredConditionAccessor;
        private readonly IOperation operation;
        static IWhileLoopOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IWhileLoopOperationWrapper));
            ConditionAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Condition));
            ConditionIsTopAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(ConditionIsTop));
            ConditionIsUntilAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(ConditionIsUntil));
            IgnoredConditionAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(IgnoredCondition));
        }

        private IWhileLoopOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Condition => ConditionAccessor(this.WrappedOperation);
        public bool ConditionIsTop => ConditionIsTopAccessor(this.WrappedOperation);
        public bool ConditionIsUntil => ConditionIsUntilAccessor(this.WrappedOperation);
        public IOperation IgnoredCondition => IgnoredConditionAccessor(this.WrappedOperation);
        public static explicit operator IWhileLoopOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IWhileLoopOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public object LoopKind => ((ILoopOperationWrapper)this).LoopKind;
        public IOperation Body => ((ILoopOperationWrapper)this).Body;
        public ImmutableArray<ILocalSymbol> Locals => ((ILoopOperationWrapper)this).Locals;
        public ILabelSymbol ContinueLabel => ((ILoopOperationWrapper)this).ContinueLabel;
        public ILabelSymbol ExitLabel => ((ILoopOperationWrapper)this).ExitLabel;
        public static explicit operator IWhileLoopOperationWrapper(ILoopOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator ILoopOperationWrapper(IWhileLoopOperationWrapper wrapper) => ILoopOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IWhileLoopOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IWhileLoopOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
