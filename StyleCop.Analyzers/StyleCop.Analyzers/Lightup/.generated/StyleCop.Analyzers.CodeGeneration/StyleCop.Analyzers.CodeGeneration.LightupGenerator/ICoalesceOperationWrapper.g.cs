namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ICoalesceOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ICoalesceOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> ValueAccessor;
        private static readonly Func<IOperation, IOperation> WhenNullAccessor;
        private readonly IOperation operation;
        static ICoalesceOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ICoalesceOperationWrapper));
            ValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Value));
            WhenNullAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(WhenNull));
        }

        private ICoalesceOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Value => ValueAccessor(this.WrappedOperation);
        public IOperation WhenNull => WhenNullAccessor(this.WrappedOperation);
        public object ValueConversion => throw new NotImplementedException("Property 'ICoalesceOperation.ValueConversion' has unsupported type 'CommonConversion'");
        public static ICoalesceOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ICoalesceOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}