namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IWhileLoopOperationWrapper : IOperationWrapper
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