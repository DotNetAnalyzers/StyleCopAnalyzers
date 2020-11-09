// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct ILoopOperationWrapper : IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.ILoopOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, IOperation> BodyAccessor;
        private static readonly Func<IOperation, ImmutableArray<ILocalSymbol>> LocalsAccessor;
        private static readonly Func<IOperation, ILabelSymbol> ContinueLabelAccessor;
        private static readonly Func<IOperation, ILabelSymbol> ExitLabelAccessor;
        private readonly IOperation operation;
        static ILoopOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(ILoopOperationWrapper));
            BodyAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation>(WrappedType, nameof(Body));
            LocalsAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ImmutableArray<ILocalSymbol>>(WrappedType, nameof(Locals));
            ContinueLabelAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ILabelSymbol>(WrappedType, nameof(ContinueLabel));
            ExitLabelAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ILabelSymbol>(WrappedType, nameof(ExitLabel));
        }

        private ILoopOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public object LoopKind => throw new NotImplementedException("Property 'ILoopOperation.LoopKind' has unsupported type 'LoopKind'");
        public IOperation Body => BodyAccessor(this.WrappedOperation);
        public ImmutableArray<ILocalSymbol> Locals => LocalsAccessor(this.WrappedOperation);
        public ILabelSymbol ContinueLabel => ContinueLabelAccessor(this.WrappedOperation);
        public ILabelSymbol ExitLabel => ExitLabelAccessor(this.WrappedOperation);
        public static ILoopOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ILoopOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }

        internal static ILoopOperationWrapper FromUpcast(IOperation operation)
        {
            return new ILoopOperationWrapper(operation);
        }
    }
}
