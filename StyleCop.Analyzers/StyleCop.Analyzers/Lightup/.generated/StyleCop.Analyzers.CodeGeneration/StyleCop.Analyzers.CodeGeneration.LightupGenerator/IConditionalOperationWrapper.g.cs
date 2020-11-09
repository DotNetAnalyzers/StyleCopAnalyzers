namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IConditionalOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IConditionalOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> ConditionAccessor;
        private static readonly Func<IOperation, IOperation> WhenTrueAccessor;
        private static readonly Func<IOperation, IOperation> WhenFalseAccessor;
        private static readonly Func<IOperation, bool> IsRefAccessor;
        private readonly IOperation operation;
        static IConditionalOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IConditionalOperationWrapper));
            ConditionAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Condition));
            WhenTrueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(WhenTrue));
            WhenFalseAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(WhenFalse));
            IsRefAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsRef));
        }

        private IConditionalOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Condition => ConditionAccessor(this.WrappedOperation);
        public IOperation WhenTrue => WhenTrueAccessor(this.WrappedOperation);
        public IOperation WhenFalse => WhenFalseAccessor(this.WrappedOperation);
        public bool IsRef => IsRefAccessor(this.WrappedOperation);
        public static IConditionalOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IConditionalOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}