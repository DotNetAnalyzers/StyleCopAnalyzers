namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IBranchOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IBranchOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ILabelSymbol> TargetAccessor;
        private readonly IOperation operation;
        static IBranchOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IBranchOperationWrapper));
            TargetAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ILabelSymbol>(WrappedType, nameof(Target));
        }

        private IBranchOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ILabelSymbol Target => TargetAccessor(this.WrappedOperation);
        public object BranchKind => throw new NotImplementedException("Property 'IBranchOperation.BranchKind' has unsupported type 'BranchKind'");
        public static IBranchOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IBranchOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}