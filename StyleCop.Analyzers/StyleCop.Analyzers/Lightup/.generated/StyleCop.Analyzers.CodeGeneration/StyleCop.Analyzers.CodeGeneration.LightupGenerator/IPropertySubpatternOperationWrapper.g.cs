namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IPropertySubpatternOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IPropertySubpatternOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> MemberAccessor;
        private static readonly Func<IOperation, IOperation> PatternAccessor;
        private readonly IOperation operation;
        static IPropertySubpatternOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IPropertySubpatternOperationWrapper));
            MemberAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Member));
            PatternAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Pattern));
        }

        private IPropertySubpatternOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Member => MemberAccessor(this.WrappedOperation);
        public IPatternOperationWrapper Pattern => IPatternOperationWrapper.FromOperation(PatternAccessor(this.WrappedOperation));
        public static IPropertySubpatternOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IPropertySubpatternOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}