// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    internal readonly struct AnalyzerConfigOptionsWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions";
        private static readonly Type? WrappedType;

        private static readonly Func<StringComparer?> KeyComparerAccessor;
        private static readonly TryGetValueAccessor<object, string, string> TryGetValueAccessor;

        private static readonly string[] EmptyKeys = new string[0];
        private static readonly Func<object, IEnumerable<string>?> KeysAccessor;

        private readonly object? node;

        static AnalyzerConfigOptionsWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(AnalyzerConfigOptionsWrapper));

            KeysAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<object, IEnumerable<string>?>(WrappedType, "Keys");
            KeyComparerAccessor = LightupHelpers.CreateStaticPropertyAccessor<StringComparer?>(WrappedType, nameof(KeyComparer));
            TryGetValueAccessor = LightupHelpers.CreateTryGetValueAccessor<object, string, string>(WrappedType, typeof(string), nameof(TryGetValue));
        }

        private AnalyzerConfigOptionsWrapper(object node)
        {
            this.node = node;
        }

        public static StringComparer? KeyComparer
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

        public static AnalyzerConfigOptionsWrapper FromObject(object? node)
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

        public static bool IsInstance([NotNullWhen(true)] object? obj)
        {
            return obj != null && LightupHelpers.CanWrapObject(obj, WrappedType);
        }

        /// <summary>
        /// Gets the keys defined in the options, or an empty collection when the compiler does not support
        /// enumerating them (including hosts that don't implement `Keys`).
        /// </summary>
        /// <returns>The keys defined in the options.</returns>
        public IEnumerable<string> GetKeys()
        {
            if (this.node is null)
            {
                return EmptyKeys;
            }

            try
            {
                return KeysAccessor(this.node) ?? EmptyKeys;
            }
            catch (NotImplementedException)
            {
                // The base AnalyzerConfigOptions.Keys throws when a host doesn't override it.
                return EmptyKeys;
            }
        }

        public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        {
            if (this.node is null && WrappedType is null)
            {
                // Gracefully fall back to a collection with no values
                value = null;
                return false;
            }

            return TryGetValueAccessor(this.node!, key, out value);
        }
    }
}
