// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;

    internal readonly struct IOperationWrapper
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.IOperation";
        private static readonly Func<IOperation, IOperation?> ParentAccessor;
        private static readonly Func<IOperation, IEnumerable<IOperation>?> ChildrenAccessor;
        private static readonly Func<IOperation, string?> LanguageAccessor;
        private static readonly Func<IOperation, bool> IsImplicitAccessor;
        private static readonly Func<IOperation, SemanticModel?> SemanticModelAccessor;
        private readonly IOperation? operation;
        static IOperationWrapper()
        {
            ParentAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IOperation?>(typeof(IOperation), nameof(Parent));
            ChildrenAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IEnumerable<IOperation>?>(typeof(IOperation), nameof(Children));
            LanguageAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, string?>(typeof(IOperation), nameof(Language));
            IsImplicitAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, bool>(typeof(IOperation), nameof(IsImplicit));
            SemanticModelAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, SemanticModel?>(typeof(IOperation), nameof(SemanticModel));
        }

        private IOperationWrapper(IOperation? operation)
        {
            this.operation = operation;
        }

        public IOperation? WrappedOperation => this.operation;
        public IOperation? Parent => ParentAccessor(this.operation!);
        public OperationKind Kind => this.operation!.Kind;
        public SyntaxNode Syntax => this.operation!.Syntax;
        public ITypeSymbol? Type => this.operation!.Type;
        public Optional<object?> ConstantValue => this.operation!.ConstantValue;
        public IEnumerable<IOperation>? Children => ChildrenAccessor(this.operation!);
        public string? Language => LanguageAccessor(this.operation!);
        public bool IsImplicit => IsImplicitAccessor(this.operation!);
        public SemanticModel? SemanticModel => SemanticModelAccessor(this.operation!);

        public static IOperationWrapper FromOperation(IOperation? operation)
        {
            if (operation == null)
            {
                return default;
            }

            return new IOperationWrapper(operation);
        }

        public static bool IsInstance([NotNullWhen(true)] IOperation? operation)
        {
            return operation != null;
        }

        internal static IOperationWrapper FromUpcast(IOperation? operation)
        {
            return new IOperationWrapper(operation);
        }
    }
}
