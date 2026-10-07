// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;

    internal readonly struct IImportScopeWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.IImportScope";
        private static readonly Type WrappedType;

        private static readonly Func<object?, ImmutableArray<IAliasSymbol>> AliasesAccessor;

        private readonly object node;

        static IImportScopeWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(IImportScopeWrapper));

            AliasesAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<object?, ImmutableArray<IAliasSymbol>>(WrappedType, nameof(Aliases));
        }

        private IImportScopeWrapper(object node)
        {
            this.node = node;
        }

        public ImmutableArray<IAliasSymbol> Aliases => AliasesAccessor(this.node);

        // NOTE: Referenced via reflection
        public static IImportScopeWrapper FromObject(object node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new IImportScopeWrapper(node);
        }

        public static bool IsInstance(object obj)
        {
            return obj != null && LightupHelpers.CanWrapObject(obj, WrappedType);
        }
    }
}
