namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IBinaryPatternOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IBinaryPatternOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> LeftPatternAccessor;
        private static readonly Func<IOperation, IOperation> RightPatternAccessor;
        private readonly IOperation operation;
        static IBinaryPatternOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IBinaryPatternOperationWrapper));
            LeftPatternAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(LeftPattern));
            RightPatternAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(RightPattern));
        }

        private IBinaryPatternOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public object OperatorKind => throw new NotImplementedException("Property 'IBinaryPatternOperation.OperatorKind' has unsupported type 'BinaryOperatorKind'");
        public IPatternOperationWrapper LeftPattern => IPatternOperationWrapper.FromOperation(LeftPatternAccessor(this.WrappedOperation));
        public IPatternOperationWrapper RightPattern => IPatternOperationWrapper.FromOperation(RightPatternAccessor(this.WrappedOperation));
        public ITypeSymbol InputType => ((IPatternOperationWrapper)this).InputType;
        public ITypeSymbol NarrowedType => ((IPatternOperationWrapper)this).NarrowedType;
        public static explicit operator IBinaryPatternOperationWrapper(IPatternOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IPatternOperationWrapper(IBinaryPatternOperationWrapper wrapper) => IPatternOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IBinaryPatternOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IBinaryPatternOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}