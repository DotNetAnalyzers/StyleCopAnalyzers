namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IMethodReferenceOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IMethodReferenceOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IMethodSymbol> MethodAccessor;
        private static readonly Func<IOperation, bool> IsVirtualAccessor;
        private readonly IOperation operation;
        static IMethodReferenceOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IMethodReferenceOperationWrapper));
            MethodAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(Method));
            IsVirtualAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsVirtual));
        }

        private IMethodReferenceOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IMethodSymbol Method => MethodAccessor(this.WrappedOperation);
        public bool IsVirtual => IsVirtualAccessor(this.WrappedOperation);
        public IOperation Instance => ((IMemberReferenceOperationWrapper)this).Instance;
        public ISymbol Member => ((IMemberReferenceOperationWrapper)this).Member;
        public static explicit operator IMethodReferenceOperationWrapper(IMemberReferenceOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IMemberReferenceOperationWrapper(IMethodReferenceOperationWrapper wrapper) => IMemberReferenceOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IMethodReferenceOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IMethodReferenceOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}