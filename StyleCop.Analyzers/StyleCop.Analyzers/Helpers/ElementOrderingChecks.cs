// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis;
    using OrderingRules;

    /// <summary>
    /// Contains information about enabled element order rules.
    /// </summary>
    internal struct ElementOrderingChecks
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementOrderingChecks"/> struct.
        /// </summary>
        /// <param name="checkElementType">Indicates whether element type should be checked.</param>
        /// <param name="checkAccessLevel">Indicates whether access level should be checked.</param>
        /// <param name="checkConst">Indicates whether const modifiers should be checked.</param>
        /// <param name="checkStatic">Indicates whether static modifiers should be checked.</param>
        /// <param name="checkReadonly">Indicates whether readonly modifiers should be checked.</param>
        internal ElementOrderingChecks(bool checkElementType, bool checkAccessLevel, bool checkConst, bool checkStatic, bool checkReadonly)
        {
            this.ElementType = checkElementType;
            this.AccessLevel = checkAccessLevel;
            this.Const = checkConst;
            this.Static = checkStatic;
            this.Readonly = checkReadonly;
        }

        /// <summary>
        /// Gets a value indicating whether the element type should be checked.
        /// </summary>
        /// <value>Indicates whether element type should be checked.</value>
        internal bool ElementType { get; }

        /// <summary>
        /// Gets a value indicating whether the access level should be checked.
        /// </summary>
        /// <value>Indicates whether access level should be checked.</value>
        internal bool AccessLevel { get; }

        /// <summary>
        /// Gets a value indicating whether the const modifier should be checked.
        /// </summary>
        /// <value>Indicates whether const modifier should be checked.</value>
        internal bool Const { get; }

        /// <summary>
        /// Gets a value indicating whether the static modifier should be checked.
        /// </summary>
        /// <value>Indicates whether static modifier should be checked.</value>
        internal bool Static { get; }

        /// <summary>
        /// Gets a value indicating whether the readonly modifier should be checked.
        /// </summary>
        /// <value>Indicates whether readonly modifier should be checked.</value>
        internal bool Readonly { get; }

        /// <summary>
        /// Creates a configured instance of the <see cref="ElementOrderingChecks"/> struct for the given semantic model.
        /// </summary>
        /// <param name="semanticModel">The semantic model.</param>
        /// <returns>The created <see cref="ElementOrderingChecks"/>.</returns>
        internal static ElementOrderingChecks GetElementOrderingChecksForSemanticModel(SemanticModel semanticModel)
        {
            var checkReadonly = !semanticModel.Compilation.IsAnalyzerSuppressed(SA1214StaticReadonlyElementsMustAppearBeforeStaticNonReadonlyElements.DiagnosticId)
                || !semanticModel.Compilation.IsAnalyzerSuppressed(SA1215InstanceReadonlyElementsMustAppearBeforeInstanceNonReadonlyElements.DiagnosticId);
            return new ElementOrderingChecks(
                !semanticModel.Compilation.IsAnalyzerSuppressed(SA1201ElementsMustAppearInTheCorrectOrder.DiagnosticId),
                !semanticModel.Compilation.IsAnalyzerSuppressed(SA1202ElementsMustBeOrderedByAccess.DiagnosticId),
                !semanticModel.Compilation.IsAnalyzerSuppressed(SA1203ConstantsMustAppearBeforeFields.DiagnosticId),
                !semanticModel.Compilation.IsAnalyzerSuppressed(SA1204StaticElementsMustAppearBeforeInstanceElements.DiagnosticId),
                checkReadonly);
        }
    }
}
