namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ICatchClauseOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ICatchClauseOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> ExceptionDeclarationOrExpressionAccessor;
        private static readonly Func<IOperation, ITypeSymbol> ExceptionTypeAccessor;
        private static readonly Func<IOperation, ImmutableArray<ILocalSymbol>> LocalsAccessor;
        private static readonly Func<IOperation, IOperation> FilterAccessor;
        private static readonly Func<IOperation, IOperation> HandlerAccessor;
        private readonly IOperation operation;
        static ICatchClauseOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ICatchClauseOperationWrapper));
            ExceptionDeclarationOrExpressionAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(ExceptionDeclarationOrExpression));
            ExceptionTypeAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ITypeSymbol>(WrappedType, nameof(ExceptionType));
            LocalsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<ILocalSymbol>>(WrappedType, nameof(Locals));
            FilterAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Filter));
            HandlerAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Handler));
        }

        private ICatchClauseOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation ExceptionDeclarationOrExpression => ExceptionDeclarationOrExpressionAccessor(this.WrappedOperation);
        public ITypeSymbol ExceptionType => ExceptionTypeAccessor(this.WrappedOperation);
        public ImmutableArray<ILocalSymbol> Locals => LocalsAccessor(this.WrappedOperation);
        public IOperation Filter => FilterAccessor(this.WrappedOperation);
        public IBlockOperationWrapper Handler => IBlockOperationWrapper.FromOperation(HandlerAccessor(this.WrappedOperation));
        public static ICatchClauseOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ICatchClauseOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}