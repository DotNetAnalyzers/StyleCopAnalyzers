namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IForToLoopOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IForToLoopOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> LoopControlVariableAccessor;
        private static readonly Func<IOperation, IOperation> InitialValueAccessor;
        private static readonly Func<IOperation, IOperation> LimitValueAccessor;
        private static readonly Func<IOperation, IOperation> StepValueAccessor;
        private static readonly Func<IOperation, bool> IsCheckedAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> NextVariablesAccessor;
        private readonly IOperation operation;
        static IForToLoopOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IForToLoopOperationWrapper));
            LoopControlVariableAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(LoopControlVariable));
            InitialValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(InitialValue));
            LimitValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(LimitValue));
            StepValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(StepValue));
            IsCheckedAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsChecked));
            NextVariablesAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IOperation>>(WrappedType, nameof(NextVariables));
        }

        private IForToLoopOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation LoopControlVariable => LoopControlVariableAccessor(this.WrappedOperation);
        public IOperation InitialValue => InitialValueAccessor(this.WrappedOperation);
        public IOperation LimitValue => LimitValueAccessor(this.WrappedOperation);
        public IOperation StepValue => StepValueAccessor(this.WrappedOperation);
        public bool IsChecked => IsCheckedAccessor(this.WrappedOperation);
        public ImmutableArray<IOperation> NextVariables => NextVariablesAccessor(this.WrappedOperation);
        public object LoopKind => ((ILoopOperationWrapper)this).LoopKind;
        public IOperation Body => ((ILoopOperationWrapper)this).Body;
        public ImmutableArray<ILocalSymbol> Locals => ((ILoopOperationWrapper)this).Locals;
        public ILabelSymbol ContinueLabel => ((ILoopOperationWrapper)this).ContinueLabel;
        public ILabelSymbol ExitLabel => ((ILoopOperationWrapper)this).ExitLabel;
        public static explicit operator IForToLoopOperationWrapper(ILoopOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator ILoopOperationWrapper(IForToLoopOperationWrapper wrapper) => ILoopOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IForToLoopOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IForToLoopOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}