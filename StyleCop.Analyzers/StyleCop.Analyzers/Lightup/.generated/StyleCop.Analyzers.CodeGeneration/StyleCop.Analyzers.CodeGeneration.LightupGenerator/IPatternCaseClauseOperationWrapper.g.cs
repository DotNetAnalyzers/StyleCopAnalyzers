namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IPatternCaseClauseOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IPatternCaseClauseOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ILabelSymbol> LabelAccessor;
        private static readonly Func<IOperation, IOperation> PatternAccessor;
        private static readonly Func<IOperation, IOperation> GuardAccessor;
        private readonly IOperation operation;
        static IPatternCaseClauseOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IPatternCaseClauseOperationWrapper));
            LabelAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ILabelSymbol>(WrappedType, nameof(Label));
            PatternAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Pattern));
            GuardAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Guard));
        }

        private IPatternCaseClauseOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ILabelSymbol Label => LabelAccessor(this.WrappedOperation);
        public IPatternOperationWrapper Pattern => IPatternOperationWrapper.FromOperation(PatternAccessor(this.WrappedOperation));
        public IOperation Guard => GuardAccessor(this.WrappedOperation);
        public object CaseKind => ((ICaseClauseOperationWrapper)this).CaseKind;
        public static explicit operator IPatternCaseClauseOperationWrapper(ICaseClauseOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator ICaseClauseOperationWrapper(IPatternCaseClauseOperationWrapper wrapper) => ICaseClauseOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IPatternCaseClauseOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IPatternCaseClauseOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}