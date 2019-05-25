// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System;
    using System.Collections.Immutable;
    using LightJson;
    using Microsoft.CodeAnalysis.CSharp;

    internal class KindOrderingSettings
    {
        // extern alias and using statements are missing here because the compiler itself is enforcing the right order.
        private static readonly ImmutableArray<SyntaxKind> DefaultOuterKindOrder = ImmutableArray.Create(
            SyntaxKind.NamespaceDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.ClassDeclaration);

        private static readonly ImmutableArray<SyntaxKind> DefaultTypeDeclarationKindOrder = ImmutableArray.Create(
            SyntaxKind.FieldDeclaration,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.DestructorDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.EventDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.IndexerDeclaration,
            SyntaxKind.ConversionOperatorDeclaration,
            SyntaxKind.OperatorDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.ClassDeclaration);

        /// <summary>
        /// This is the backing field for the <see cref="CompilationUnit"/> property.
        /// </summary>
        private readonly ImmutableArray<SyntaxKind>.Builder compilationUnit;

        /// <summary>
        /// This is the backing field for the <see cref="NamespaceDeclaration"/> property.
        /// </summary>
        private readonly ImmutableArray<SyntaxKind>.Builder namespaceDeclaration;

        /// <summary>
        /// This is the backing field for the <see cref="TypeDeclaration"/> property.
        /// </summary>
        private readonly ImmutableArray<SyntaxKind>.Builder typeDeclaration;

        /// <summary>
        /// Initializes a new instance of the <see cref="KindOrderingSettings"/> class.
        /// </summary>
        protected internal KindOrderingSettings()
        {
            this.compilationUnit = ImmutableArray.CreateBuilder<SyntaxKind>();
            this.namespaceDeclaration = ImmutableArray.CreateBuilder<SyntaxKind>();
            this.typeDeclaration = ImmutableArray.CreateBuilder<SyntaxKind>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KindOrderingSettings"/> class.
        /// </summary>
        /// <param name="orderingSettingsObject">The JSON object containing the settings.</param>
        protected internal KindOrderingSettings(JsonObject orderingSettingsObject)
            : this()
        {
            foreach (var kvp in orderingSettingsObject)
            {
                switch (kvp.Key)
                {
                case "compilationUnit":
                    kvp.AssertIsArray();
                    foreach (var value in kvp.Value.AsJsonArray)
                    {
                        this.compilationUnit.Add(GetSyntaxKind(value, kvp.Key, DefaultOuterKindOrder));
                    }

                    break;

                case "namespaceDeclaration":
                    kvp.AssertIsArray();
                    foreach (var value in kvp.Value.AsJsonArray)
                    {
                        this.namespaceDeclaration.Add(GetSyntaxKind(value, kvp.Key, DefaultOuterKindOrder));
                    }

                    break;

                case "typeDeclaration":
                    kvp.AssertIsArray();
                    foreach (var value in kvp.Value.AsJsonArray)
                    {
                        this.typeDeclaration.Add(GetSyntaxKind(value, kvp.Key, DefaultTypeDeclarationKindOrder));
                    }

                    break;

                default:
                    break;
                }
            }
        }

        public ImmutableArray<SyntaxKind> CompilationUnit
        {
            get
            {
                return this.compilationUnit.Count > 0 ? this.compilationUnit.ToImmutable() : DefaultOuterKindOrder;
            }
        }

        public ImmutableArray<SyntaxKind> NamespaceDeclaration
        {
            get
            {
                return this.namespaceDeclaration.Count > 0 ? this.namespaceDeclaration.ToImmutable() : DefaultOuterKindOrder;
            }
        }

        public ImmutableArray<SyntaxKind> TypeDeclaration
        {
            get
            {
                return this.typeDeclaration.Count > 0 ? this.typeDeclaration.ToImmutable() : DefaultTypeDeclarationKindOrder;
            }
        }

        private static SyntaxKind GetSyntaxKind(JsonValue jsonValue, string elementName, ImmutableArray<SyntaxKind> validKinds)
        {
            if (!jsonValue.IsString)
            {
                throw new InvalidSettingsException($"{elementName} must contain an enum (string) value");
            }

            if (!Enum.TryParse<SyntaxKind>(jsonValue.AsString + "Declaration", true, out var kind))
            {
                throw new InvalidSettingsException($"{elementName} cannot contain enum value '{jsonValue.AsString}'");
            }

            if (!validKinds.Contains(kind))
            {
                throw new InvalidSettingsException($"{elementName} can only contain syntax kinds {string.Join(", ", validKinds)}, but {kind} was found.");
            }

            return kind;
        }
    }
}
