namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IRangeCaseClauseOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IRangeCaseClauseOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> MinimumValueAccessor;
        private static readonly Func<IOperation, IOperation> MaximumValueAccessor;
        private readonly IOperation operation;
        static IRangeCaseClauseOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IRangeCaseClauseOperationWrapper));
            MinimumValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(MinimumValue));
            MaximumValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(MaximumValue));
        }

        private IRangeCaseClauseOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation MinimumValue => MinimumValueAccessor(this.WrappedOperation);
        public IOperation MaximumValue => MaximumValueAccessor(this.WrappedOperation);
        public object CaseKind => ((ICaseClauseOperationWrapper)this).CaseKind;
        public ILabelSymbol Label => ((ICaseClauseOperationWrapper)this).Label;
        public static explicit operator IRangeCaseClauseOperationWrapper(ICaseClauseOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator ICaseClauseOperationWrapper(IRangeCaseClauseOperationWrapper wrapper) => ICaseClauseOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IRangeCaseClauseOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IRangeCaseClauseOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}