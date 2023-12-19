// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IRecursivePatternOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IRecursivePatternOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ITypeSymbol> MatchedTypeAccessor;
        private static readonly Func<IOperation, ISymbol> DeconstructSymbolAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> DeconstructionSubpatternsAccessor;
        private static readonly Func<IOperation, ImmutableArray<IOperation>> PropertySubpatternsAccessor;
        private static readonly Func<IOperation, ISymbol> DeclaredSymbolAccessor;
        private readonly IOperation operation;
        static IRecursivePatternOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IRecursivePatternOperationWrapper));
            MatchedTypeAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ITypeSymbol>(WrappedType, nameof(MatchedType));
            DeconstructSymbolAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ISymbol>(WrappedType, nameof(DeconstructSymbol));
            DeconstructionSubpatternsAccessor = LightupHelpers.CreateOperationListPropertyAccessor<IOperation>(WrappedType, nameof(DeconstructionSubpatterns));
            PropertySubpatternsAccessor = LightupHelpers.CreateOperationListPropertyAccessor<IOperation>(WrappedType, nameof(PropertySubpatterns));
            DeclaredSymbolAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ISymbol>(WrappedType, nameof(DeclaredSymbol));
        }

        private IRecursivePatternOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ITypeSymbol MatchedType => MatchedTypeAccessor(this.WrappedOperation);
        public ISymbol DeconstructSymbol => DeconstructSymbolAccessor(this.WrappedOperation);
        public ImmutableArray<IOperation> DeconstructionSubpatterns => DeconstructionSubpatternsAccessor(this.WrappedOperation);
        public ImmutableArray<IOperation> PropertySubpatterns => PropertySubpatternsAccessor(this.WrappedOperation);
        public ISymbol DeclaredSymbol => DeclaredSymbolAccessor(this.WrappedOperation);
        public static explicit operator IRecursivePatternOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IRecursivePatternOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public ITypeSymbol InputType => ((IPatternOperationWrapper)this).InputType;
        public ITypeSymbol NarrowedType => ((IPatternOperationWrapper)this).NarrowedType;
        public static explicit operator IRecursivePatternOperationWrapper(IPatternOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IPatternOperationWrapper(IRecursivePatternOperationWrapper wrapper) => IPatternOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IRecursivePatternOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IRecursivePatternOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
