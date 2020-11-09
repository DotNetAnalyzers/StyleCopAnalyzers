namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IInterpolatedStringContentOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IInterpolatedStringContentOperation";
        private static readonly Type WrappedType;
        private readonly IOperation operation;
        static IInterpolatedStringContentOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IInterpolatedStringContentOperationWrapper));
        }

        private IInterpolatedStringContentOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public static IInterpolatedStringContentOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IInterpolatedStringContentOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }

        internal static IInterpolatedStringContentOperationWrapper FromUpcast(IOperation operation)
        {
            return new IInterpolatedStringContentOperationWrapper(operation);
        }
    }
}