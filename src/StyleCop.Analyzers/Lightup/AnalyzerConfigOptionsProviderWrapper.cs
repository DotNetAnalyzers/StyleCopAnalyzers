// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal readonly struct AnalyzerConfigOptionsProviderWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider";
        private static readonly Type WrappedType;

        private static readonly Func<object, object> GlobalOptionsAccessor;
        private static readonly Func<object, SyntaxTree, object> GetOptionsSyntaxTreeAccessor;
        private static readonly Func<object, AdditionalText, object> GetOptionsAdditionalTextAccessor;

        private readonly object node;

        static AnalyzerConfigOptionsProviderWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(AnalyzerConfigOptionsProviderWrapper));

            GlobalOptionsAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<object, object>(WrappedType, nameof(GlobalOptions));
            GetOptionsSyntaxTreeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<object, SyntaxTree, object>(WrappedType, typeof(SyntaxTree), nameof(GetOptions));
            GetOptionsAdditionalTextAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<object, AdditionalText, object>(WrappedType, typeof(AdditionalText), nameof(GetOptions));
        }

        private AnalyzerConfigOptionsProviderWrapper(object node)
        {
            this.node = node;
        }

        public AnalyzerConfigOptionsWrapper GlobalOptions
        {
            get
            {
                if (this.node == null && WrappedType == null)
                {
                    // Gracefully fall back to a collection with no values
                    return AnalyzerConfigOptionsWrapper.FromObject(null);
                }

                return AnalyzerConfigOptionsWrapper.FromObject(GlobalOptionsAccessor(this.node));
            }
        }

        public static AnalyzerConfigOptionsProviderWrapper FromObject(object node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new AnalyzerConfigOptionsProviderWrapper(node);
        }

        public static bool IsInstance(object obj)
        {
            return obj != null && LightupHelpers.CanWrapObject(obj, WrappedType);
        }

        public AnalyzerConfigOptionsWrapper GetOptions(SyntaxTree tree)
        {
            if (this.node == null && WrappedType == null)
            {
                // Gracefully fall back to a collection with no values
                if (tree == null)
                {
                    throw new ArgumentNullException(nameof(tree));
                }

                return AnalyzerConfigOptionsWrapper.FromObject(null);
            }

            return AnalyzerConfigOptionsWrapper.FromObject(GetOptionsSyntaxTreeAccessor(this.node, tree));
        }

        public AnalyzerConfigOptionsWrapper GetOptions(AdditionalText textFile)
        {
            if (this.node == null && WrappedType == null)
            {
                // Gracefully fall back to a collection with no values
                if (textFile == null)
                {
                    throw new ArgumentNullException(nameof(textFile));
                }

                return AnalyzerConfigOptionsWrapper.FromObject(null);
            }

            return AnalyzerConfigOptionsWrapper.FromObject(GetOptionsAdditionalTextAccessor(this.node, textFile));
        }
    }
}
