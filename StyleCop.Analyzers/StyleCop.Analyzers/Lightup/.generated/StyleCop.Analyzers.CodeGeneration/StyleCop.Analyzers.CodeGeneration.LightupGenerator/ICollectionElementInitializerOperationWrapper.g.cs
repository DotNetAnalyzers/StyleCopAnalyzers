namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ICollectionElementInitializerOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ICollectionElementInitializerOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IMethodSymbol> AddMethodAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> ArgumentsAccessor;
        private static readonly Func<IOperation, bool> IsDynamicAccessor;
        private readonly IOperation operation;
        static ICollectionElementInitializerOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ICollectionElementInitializerOperationWrapper));
            AddMethodAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(AddMethod));
            ArgumentsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IOperation>>(WrappedType, nameof(Arguments));
            IsDynamicAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsDynamic));
        }

        private ICollectionElementInitializerOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IMethodSymbol AddMethod => AddMethodAccessor(this.WrappedOperation);
        public ImmutableArray<IOperation> Arguments => ArgumentsAccessor(this.WrappedOperation);
        public bool IsDynamic => IsDynamicAccessor(this.WrappedOperation);
        public static ICollectionElementInitializerOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ICollectionElementInitializerOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}