namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IInterpolatedStringTextOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IInterpolatedStringTextOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> TextAccessor;
        private readonly IOperation operation;
        static IInterpolatedStringTextOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IInterpolatedStringTextOperationWrapper));
            TextAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Text));
        }

        private IInterpolatedStringTextOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Text => TextAccessor(this.WrappedOperation);
        public static explicit operator IInterpolatedStringTextOperationWrapper(IInterpolatedStringContentOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IInterpolatedStringContentOperationWrapper(IInterpolatedStringTextOperationWrapper wrapper) => IInterpolatedStringContentOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IInterpolatedStringTextOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IInterpolatedStringTextOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}