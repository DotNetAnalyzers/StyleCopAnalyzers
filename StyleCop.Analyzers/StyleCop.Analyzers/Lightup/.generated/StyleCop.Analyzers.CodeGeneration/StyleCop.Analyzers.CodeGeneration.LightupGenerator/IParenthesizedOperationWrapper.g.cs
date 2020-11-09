namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IParenthesizedOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IParenthesizedOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> OperandAccessor;
        private readonly IOperation operation;
        static IParenthesizedOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IParenthesizedOperationWrapper));
            OperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Operand));
        }

        private IParenthesizedOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Operand => OperandAccessor(this.WrappedOperation);
        public static IParenthesizedOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IParenthesizedOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}