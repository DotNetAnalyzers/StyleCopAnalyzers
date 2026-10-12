// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IForEachLoopOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IForEachLoopOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> LoopControlVariableAccessor;
        private static readonly Func<IOperation, IOperation> CollectionAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> NextVariablesAccessor;
        private static readonly Func<IOperation, bool> IsAsynchronousAccessor;
        private readonly IOperation operation;
        static IForEachLoopOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IForEachLoopOperationWrapper));
            LoopControlVariableAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(LoopControlVariable));
            CollectionAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Collection));
            NextVariablesAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<IOperation>>(WrappedType, nameof(NextVariables));
            IsAsynchronousAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(IsAsynchronous));
        }

        private IForEachLoopOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public IOperation LoopControlVariable => LoopControlVariableAccessor(this.WrappedOperation);
        public IOperation Collection => CollectionAccessor(this.WrappedOperation);
        public ImmutableArray<IOperation> NextVariables => NextVariablesAccessor(this.WrappedOperation);
        public bool IsAsynchronous => IsAsynchronousAccessor(this.WrappedOperation);

        public static explicit operator IForEachLoopOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IForEachLoopOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public object LoopKind => ((ILoopOperationWrapper)this).LoopKind;
        public IOperation Body => ((ILoopOperationWrapper)this).Body;
        public ImmutableArray<ILocalSymbol> Locals => ((ILoopOperationWrapper)this).Locals;
        public ILabelSymbol ContinueLabel => ((ILoopOperationWrapper)this).ContinueLabel;
        public ILabelSymbol ExitLabel => ((ILoopOperationWrapper)this).ExitLabel;

        public static explicit operator IForEachLoopOperationWrapper(ILoopOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator ILoopOperationWrapper(IForEachLoopOperationWrapper wrapper) => ILoopOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IForEachLoopOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IForEachLoopOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
