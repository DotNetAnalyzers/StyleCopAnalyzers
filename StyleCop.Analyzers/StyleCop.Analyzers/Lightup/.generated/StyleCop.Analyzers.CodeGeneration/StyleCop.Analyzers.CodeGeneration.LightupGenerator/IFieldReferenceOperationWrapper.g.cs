namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IFieldReferenceOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IFieldReferenceOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IFieldSymbol> FieldAccessor;
        private static readonly Func<IOperation, bool> IsDeclarationAccessor;
        private readonly IOperation operation;
        static IFieldReferenceOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IFieldReferenceOperationWrapper));
            FieldAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IFieldSymbol>(WrappedType, nameof(Field));
            IsDeclarationAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsDeclaration));
        }

        private IFieldReferenceOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IFieldSymbol Field => FieldAccessor(this.WrappedOperation);
        public bool IsDeclaration => IsDeclarationAccessor(this.WrappedOperation);
        public IOperation Instance => ((IMemberReferenceOperationWrapper)this).Instance;
        public ISymbol Member => ((IMemberReferenceOperationWrapper)this).Member;
        public static explicit operator IFieldReferenceOperationWrapper(IMemberReferenceOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IMemberReferenceOperationWrapper(IFieldReferenceOperationWrapper wrapper) => IMemberReferenceOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IFieldReferenceOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IFieldReferenceOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}