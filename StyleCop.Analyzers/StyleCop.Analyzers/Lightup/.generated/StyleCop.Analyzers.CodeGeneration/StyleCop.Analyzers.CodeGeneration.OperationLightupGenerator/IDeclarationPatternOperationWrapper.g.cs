// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IDeclarationPatternOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IDeclarationPatternOperation";
        private static readonly Type WrappedType;
        private static readonly Func<IOperation, ITypeSymbol> MatchedTypeAccessor;
        private static readonly Func<IOperation, bool> MatchesNullAccessor;
        private static readonly Func<IOperation, ISymbol> DeclaredSymbolAccessor;
        private readonly IOperation operation;
        static IDeclarationPatternOperationWrapper()
        {
            WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IDeclarationPatternOperationWrapper));
            MatchedTypeAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ITypeSymbol>(WrappedType, nameof(MatchedType));
            MatchesNullAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(WrappedType, nameof(MatchesNull));
            DeclaredSymbolAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, ISymbol>(WrappedType, nameof(DeclaredSymbol));
        }

        private IDeclarationPatternOperationWrapper(IOperation operation)
        {
            this.operation = operation;
        }

        public IOperation WrappedOperation => this.operation;
        public ITypeSymbol Type => this.WrappedOperation.Type;
        public ITypeSymbol MatchedType => MatchedTypeAccessor(this.WrappedOperation);
        public bool MatchesNull => MatchesNullAccessor(this.WrappedOperation);
        public ISymbol DeclaredSymbol => DeclaredSymbolAccessor(this.WrappedOperation);
        public static explicit operator IDeclarationPatternOperationWrapper(IOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IOperationWrapper(IDeclarationPatternOperationWrapper wrapper) => IOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public ITypeSymbol InputType => ((IPatternOperationWrapper)this).InputType;
        public ITypeSymbol NarrowedType => ((IPatternOperationWrapper)this).NarrowedType;
        public static explicit operator IDeclarationPatternOperationWrapper(IPatternOperationWrapper wrapper) => FromOperation(wrapper.WrappedOperation);
        public static implicit operator IPatternOperationWrapper(IDeclarationPatternOperationWrapper wrapper) => IPatternOperationWrapper.FromUpcast(wrapper.WrappedOperation);
        public static IDeclarationPatternOperationWrapper FromOperation(IOperation operation)
        {
            if (operation == null)
            {
                return default;
            }

            if (!IsInstance(operation))
            {
                throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IDeclarationPatternOperationWrapper(operation);
        }

        public static bool IsInstance(IOperation operation)
        {
            return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
        }
    }
}
