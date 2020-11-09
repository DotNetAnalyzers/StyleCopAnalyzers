namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IInvocationOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IInvocationOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IMethodSymbol> TargetMethodAccessor;
        private static readonly Func<IOperation, IOperation> InstanceAccessor;
        private static readonly Func<IOperation, bool> IsVirtualAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> ArgumentsAccessor;
        private readonly IOperation operation;
        static IInvocationOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IInvocationOperationWrapper));
            TargetMethodAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(TargetMethod));
            InstanceAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Instance));
            IsVirtualAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsVirtual));
            ArgumentsAccessor = LightupHelpers.CreateOperationListPropertyAccessor<IOperation>(WrappedType, nameof(Arguments));
        }

        private IInvocationOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IMethodSymbol TargetMethod => TargetMethodAccessor(this.WrappedOperation);
        public IOperation Instance => InstanceAccessor(this.WrappedOperation);
        public bool IsVirtual => IsVirtualAccessor(this.WrappedOperation);
        public ImmutableArray<IOperation> Arguments => ArgumentsAccessor(this.WrappedOperation);
        public static IInvocationOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IInvocationOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}