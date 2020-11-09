namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ILockOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ILockOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> LockedValueAccessor;
        private static readonly Func<IOperation, IOperation> BodyAccessor;
        private readonly IOperation operation;
        static ILockOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ILockOperationWrapper));
            LockedValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(LockedValue));
            BodyAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Body));
        }

        private ILockOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation LockedValue => LockedValueAccessor(this.WrappedOperation);
        public IOperation Body => BodyAccessor(this.WrappedOperation);
        public static ILockOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ILockOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}