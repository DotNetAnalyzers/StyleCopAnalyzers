namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IWithOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IWithOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> OperandAccessor;
        private static readonly Func<IOperation, IMethodSymbol> CloneMethodAccessor;
        private static readonly Func<IOperation, IOperation> InitializerAccessor;
        private readonly IOperation operation;
        static IWithOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IWithOperationWrapper));
            OperandAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Operand));
            CloneMethodAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(CloneMethod));
            InitializerAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Initializer));
        }

        private IWithOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Operand => OperandAccessor(this.WrappedOperation);
        public IMethodSymbol CloneMethod => CloneMethodAccessor(this.WrappedOperation);
        public IObjectOrCollectionInitializerOperationWrapper Initializer => IObjectOrCollectionInitializerOperationWrapper.FromOperation(InitializerAccessor(this.WrappedOperation));
        public static IWithOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IWithOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}