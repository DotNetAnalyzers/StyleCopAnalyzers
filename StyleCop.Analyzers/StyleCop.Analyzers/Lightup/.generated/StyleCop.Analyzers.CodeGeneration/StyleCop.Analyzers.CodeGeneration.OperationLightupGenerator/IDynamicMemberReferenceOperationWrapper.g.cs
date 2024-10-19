// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IDynamicMemberReferenceOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IDynamicMemberReferenceOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> InstanceAccessor;
        private static readonly Func<IOperation, string> MemberNameAccessor;
        private static readonly Func<IOperation, ImmutableArray<ITypeSymbol>> TypeArgumentsAccessor;
        private static readonly Func<IOperation, ITypeSymbol> ContainingTypeAccessor;
        private readonly IOperation operation;
        static IDynamicMemberReferenceOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IDynamicMemberReferenceOperationWrapper));
            InstanceAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Instance));
            MemberNameAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, string>(WrappedType, nameof(MemberName));
            TypeArgumentsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<ITypeSymbol>>(WrappedType, nameof(TypeArguments));
            ContainingTypeAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ITypeSymbol>(WrappedType, nameof(ContainingType));
        }

        private IDynamicMemberReferenceOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Instance => InstanceAccessor(this.WrappedOperation);
        public string MemberName => MemberNameAccessor(this.WrappedOperation);
        public ImmutableArray<ITypeSymbol> TypeArguments => TypeArgumentsAccessor(this.WrappedOperation);
        public ITypeSymbol ContainingType => ContainingTypeAccessor(this.WrappedOperation);
        public static explicit operator IDynamicMemberReferenceOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IDynamicMemberReferenceOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IDynamicMemberReferenceOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IDynamicMemberReferenceOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
