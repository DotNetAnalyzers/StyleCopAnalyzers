namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IDynamicInvocationOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IDynamicInvocationOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> OperationAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> ArgumentsAccessor;
        private readonly IOperation operation;
        static IDynamicInvocationOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IDynamicInvocationOperationWrapper));
            OperationAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Operation));
            ArgumentsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IOperation>>(WrappedType, nameof(Arguments));
        }

        private IDynamicInvocationOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Operation => OperationAccessor(this.WrappedOperation);
        public ImmutableArray<IOperation> Arguments => ArgumentsAccessor(this.WrappedOperation);
        public static IDynamicInvocationOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IDynamicInvocationOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}