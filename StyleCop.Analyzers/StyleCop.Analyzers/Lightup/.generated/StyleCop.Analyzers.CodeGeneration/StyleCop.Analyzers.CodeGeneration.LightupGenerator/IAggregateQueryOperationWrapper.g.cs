namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IAggregateQueryOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IAggregateQueryOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> GroupAccessor;
        private static readonly Func<IOperation, IOperation> AggregationAccessor;
        private readonly IOperation operation;
        static IAggregateQueryOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IAggregateQueryOperationWrapper));
            GroupAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Group));
            AggregationAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Aggregation));
        }

        private IAggregateQueryOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Group => GroupAccessor(this.WrappedOperation);
        public IOperation Aggregation => AggregationAccessor(this.WrappedOperation);
        public static IAggregateQueryOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IAggregateQueryOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}