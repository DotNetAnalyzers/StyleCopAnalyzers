namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ISwitchOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ISwitchOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ImmutableArray<ILocalSymbol>> LocalsAccessor;
        private static readonly Func<IOperation, IOperation> ValueAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> CasesAccessor;
        private static readonly Func<IOperation, ILabelSymbol> ExitLabelAccessor;
        private readonly IOperation operation;
        static ISwitchOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ISwitchOperationWrapper));
            LocalsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<ILocalSymbol>>(WrappedType, nameof(Locals));
            ValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Value));
            CasesAccessor = LightupHelpers.CreateOperationListPropertyAccessor<IOperation>(WrappedType, nameof(Cases));
            ExitLabelAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ILabelSymbol>(WrappedType, nameof(ExitLabel));
        }

        private ISwitchOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ImmutableArray<ILocalSymbol> Locals => LocalsAccessor(this.WrappedOperation);
        public IOperation Value => ValueAccessor(this.WrappedOperation);
        public ImmutableArray<IOperation> Cases => CasesAccessor(this.WrappedOperation);
        public ILabelSymbol ExitLabel => ExitLabelAccessor(this.WrappedOperation);
        public static ISwitchOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ISwitchOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}