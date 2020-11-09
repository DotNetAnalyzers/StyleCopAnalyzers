namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ICaseClauseOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ICaseClauseOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ILabelSymbol> LabelAccessor;
        private readonly IOperation operation;
        static ICaseClauseOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ICaseClauseOperationWrapper));
            LabelAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ILabelSymbol>(WrappedType, nameof(Label));
        }

        private ICaseClauseOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public object CaseKind => throw new NotImplementedException("Property 'ICaseClauseOperation.CaseKind' has unsupported type 'CaseKind'");
        public ILabelSymbol Label => LabelAccessor(this.WrappedOperation);
        public static ICaseClauseOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ICaseClauseOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }

        internal static ICaseClauseOperationWrapper FromUpcast(IOperation operation)
        {
            return new ICaseClauseOperationWrapper(operation);
        }
    }
}