namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IDefaultCaseClauseOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IDefaultCaseClauseOperation";
        private static readonly Type WrappedType;
        private readonly IOperation operation;
        static IDefaultCaseClauseOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IDefaultCaseClauseOperationWrapper));
        }

        private IDefaultCaseClauseOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public object CaseKind => ((ICaseClauseOperationWrapper)this).CaseKind;
        public ILabelSymbol Label => ((ICaseClauseOperationWrapper)this).Label;
        public static explicit operator IDefaultCaseClauseOperationWrapper(ICaseClauseOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator ICaseClauseOperationWrapper(IDefaultCaseClauseOperationWrapper wrapper) => ICaseClauseOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IDefaultCaseClauseOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IDefaultCaseClauseOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}