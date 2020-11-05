// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;

    internal readonly struct IMemberReferenceOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IMemberReferenceOperation";
        private static readonly Type WrappedType;

        private static readonly Func<IOperation, IOperation> InstanceAccessor;
        private static readonly Func<IOperation, ISymbol> MemberAccessor;

        private readonly IOperation operation;

        static IMemberReferenceOperationWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(IFieldReferenceOperationWrapper));
            InstanceAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Instance));
            MemberAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ISymbol>(WrappedType, nameof(Member));
        }

        private IMemberReferenceOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;

        public ITypeSymbol Type => this.WrappedOperation.Type;

        public IOperation Instance
        {
            get
            {
                return InstanceAccessor(this.WrappedOperation);
            }
        }

        public ISymbol Member
        {
            get
            {
                return MemberAccessor(this.WrappedOperation);
            }
        }

        public static IMemberReferenceOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IMemberReferenceOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }

        internal static IMemberReferenceOperationWrapper FromUpcast(IOperation operation)
        {
            return new IMemberReferenceOperationWrapper(operation);
        }
    }
}
