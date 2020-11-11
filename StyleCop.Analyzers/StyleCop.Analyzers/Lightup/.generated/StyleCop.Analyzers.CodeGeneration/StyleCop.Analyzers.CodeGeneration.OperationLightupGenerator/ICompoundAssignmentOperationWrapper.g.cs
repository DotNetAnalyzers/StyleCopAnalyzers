// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ICompoundAssignmentOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ICompoundAssignmentOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, bool> IsLiftedAccessor;
        private static readonly Func<IOperation, bool> IsCheckedAccessor;
        private static readonly Func<IOperation, IMethodSymbol> OperatorMethodAccessor;
        private readonly IOperation operation;
        static ICompoundAssignmentOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ICompoundAssignmentOperationWrapper));
            IsLiftedAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsLifted));
            IsCheckedAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsChecked));
            OperatorMethodAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(OperatorMethod));
        }

        private ICompoundAssignmentOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public object InConversion => throw new NotImplementedException("Property 'ICompoundAssignmentOperation.InConversion' has unsupported type 'CommonConversion'");
        public object OutConversion => throw new NotImplementedException("Property 'ICompoundAssignmentOperation.OutConversion' has unsupported type 'CommonConversion'");
        public object OperatorKind => throw new NotImplementedException("Property 'ICompoundAssignmentOperation.OperatorKind' has unsupported type 'BinaryOperatorKind'");
        public bool IsLifted => IsLiftedAccessor(this.WrappedOperation);
        public bool IsChecked => IsCheckedAccessor(this.WrappedOperation);
        public IMethodSymbol OperatorMethod => OperatorMethodAccessor(this.WrappedOperation);
        public IOperation Target => ((IAssignmentOperationWrapper)this).Target;
        public IOperation Value => ((IAssignmentOperationWrapper)this).Value;
        public static explicit operator ICompoundAssignmentOperationWrapper(IAssignmentOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IAssignmentOperationWrapper(ICompoundAssignmentOperationWrapper wrapper) => IAssignmentOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static ICompoundAssignmentOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ICompoundAssignmentOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
