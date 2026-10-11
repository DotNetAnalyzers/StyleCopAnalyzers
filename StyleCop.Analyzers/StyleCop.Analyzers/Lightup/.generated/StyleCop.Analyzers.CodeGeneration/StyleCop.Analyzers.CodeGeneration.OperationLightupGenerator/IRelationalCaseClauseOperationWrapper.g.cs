// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IRelationalCaseClauseOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IRelationalCaseClauseOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> ValueAccessor;
        private readonly IOperation operation;
        static IRelationalCaseClauseOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IRelationalCaseClauseOperationWrapper));
            ValueAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Value));
        }

        private IRelationalCaseClauseOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation Value => ValueAccessor(this.WrappedOperation);
        public object Relation => throw new NotImplementedException("Property 'IRelationalCaseClauseOperation.Relation' has unsupported type 'BinaryOperatorKind'");

        public static explicit operator IRelationalCaseClauseOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IRelationalCaseClauseOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public object CaseKind => ((ICaseClauseOperationWrapper)this).CaseKind;
        public ILabelSymbol Label => ((ICaseClauseOperationWrapper)this).Label;

        public static explicit operator IRelationalCaseClauseOperationWrapper(ICaseClauseOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator ICaseClauseOperationWrapper(IRelationalCaseClauseOperationWrapper wrapper) => ICaseClauseOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IRelationalCaseClauseOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IRelationalCaseClauseOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
