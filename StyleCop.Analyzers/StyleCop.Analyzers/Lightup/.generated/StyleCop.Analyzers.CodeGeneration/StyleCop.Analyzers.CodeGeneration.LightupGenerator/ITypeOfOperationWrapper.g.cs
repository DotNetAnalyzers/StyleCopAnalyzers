namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ITypeOfOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ITypeOfOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ITypeSymbol> TypeOperandAccessor;
        private readonly IOperation operation;
        static ITypeOfOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ITypeOfOperationWrapper));
            TypeOperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ITypeSymbol>(WrappedType, nameof(TypeOperand));
        }

        private ITypeOfOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ITypeSymbol TypeOperand => TypeOperandAccessor(this.WrappedOperation);
        public static ITypeOfOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ITypeOfOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}