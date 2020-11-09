namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IUsingDeclarationOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IUsingDeclarationOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> DeclarationGroupAccessor;
        private static readonly Func<IOperation, bool> IsAsynchronousAccessor;
        private readonly IOperation operation;
        static IUsingDeclarationOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IUsingDeclarationOperationWrapper));
            DeclarationGroupAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(DeclarationGroup));
            IsAsynchronousAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsAsynchronous));
        }

        private IUsingDeclarationOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IVariableDeclarationGroupOperationWrapper DeclarationGroup => IVariableDeclarationGroupOperationWrapper.FromOperation(DeclarationGroupAccessor(this.WrappedOperation));
        public bool IsAsynchronous => IsAsynchronousAccessor(this.WrappedOperation);
        public static IUsingDeclarationOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IUsingDeclarationOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}