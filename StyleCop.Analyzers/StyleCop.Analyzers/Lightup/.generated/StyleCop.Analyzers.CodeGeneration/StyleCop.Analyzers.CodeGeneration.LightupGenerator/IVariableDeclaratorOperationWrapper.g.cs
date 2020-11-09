namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IVariableDeclaratorOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IVariableDeclaratorOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ILocalSymbol> SymbolAccessor;
        private static readonly Func<IOperation, IOperation> InitializerAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> IgnoredArgumentsAccessor;
        private readonly IOperation operation;
        static IVariableDeclaratorOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IVariableDeclaratorOperationWrapper));
            SymbolAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ILocalSymbol>(WrappedType, nameof(Symbol));
            InitializerAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Initializer));
            IgnoredArgumentsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IOperation>>(WrappedType, nameof(IgnoredArguments));
        }

        private IVariableDeclaratorOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ILocalSymbol Symbol => SymbolAccessor(this.WrappedOperation);
        public IVariableInitializerOperationWrapper Initializer => IVariableInitializerOperationWrapper.FromOperation(InitializerAccessor(this.WrappedOperation));
        public ImmutableArray<IOperation> IgnoredArguments => IgnoredArgumentsAccessor(this.WrappedOperation);
        public static IVariableDeclaratorOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IVariableDeclaratorOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}