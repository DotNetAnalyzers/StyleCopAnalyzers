// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;

    internal readonly struct AnalyzerConfigOptionsWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions";
        private static readonly Type WrappedType;

        private static readonly Func<StringComparer> KeyComparerAccessor;
        private static readonly TryGetValueAccessor<object, string, string> TryGetValueAccessor;

        private readonly object node;

        static AnalyzerConfigOptionsWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(AnalyzerConfigOptionsWrapper));

            KeyComparerAccessor = LightupHelpers.CreateStaticPropertyAccessor<StringComparer>(WrappedType, nameof(KeyComparer));
            TryGetValueAccessor = LightupHelpers.CreateTryGetValueAccessor<object, string, string>(WrappedType, typeof(string), nameof(TryGetValue));
        }

        private AnalyzerConfigOptionsWrapper(object node)
        {
            this.node = node;
        }

        public static StringComparer KeyComparer
        {
            get
            {
                if (WrappedType is null)
                {
                    // Gracefully fall back to a collection with no values
                    return StringComparer.Ordinal;
                }

                return KeyComparerAccessor();
            }
        }

        public static AnalyzerConfigOptionsWrapper FromObject(object node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new AnalyzerConfigOptionsWrapper(node);
        }

        public static bool IsInstance(object obj)
        {
            return obj != null && LightupHelpers.CanWrapObject(obj, WrappedType);
        }

        public bool TryGetValue(string key, out string value)
        {
            if (this.node is null && WrappedType is null)
            {
                // Gracefully fall back to a collection with no values
                value = null;
                return false;
            }

            return TryGetValueAccessor(this.node, key, out value);
        }
    }
}
