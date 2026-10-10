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

        private static readonly string[] EmptyKeys = new string[0];
        private static readonly Func<object, System.Collections.Generic.IEnumerable<string>> KeysAccessor;

        private readonly object node;

        static AnalyzerConfigOptionsWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(AnalyzerConfigOptionsWrapper));

            KeysAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<object, System.Collections.Generic.IEnumerable<string>>(WrappedType, "Keys");
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

        /// <summary>
        /// Gets the keys defined in the options, or an empty collection when the compiler does not support
        /// enumerating them.
        /// </summary>
        /// <returns>The keys defined in the options.</returns>
        public System.Collections.Generic.IEnumerable<string> GetKeys()
        {
            if (this.node is null)
            {
                return EmptyKeys;
            }

            return KeysAccessor(this.node) ?? EmptyKeys;
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
