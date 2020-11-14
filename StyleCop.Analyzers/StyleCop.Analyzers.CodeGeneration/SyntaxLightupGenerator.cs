// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    [Generator]
    internal sealed class SyntaxLightupGenerator : ISourceGenerator
    {
        private enum NodeKind
        {
            Predefined,
            Abstract,
            Concrete,
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxFile = context.AdditionalFiles.Single(x => Path.GetFileName(x.Path) == "Syntax.xml");
            var syntaxText = syntaxFile.GetText(context.CancellationToken);
            if (syntaxText is null)
            {
                throw new InvalidOperationException("Failed to read Syntax.xml");
            }

            var syntaxData = new SyntaxData(in context, XDocument.Parse(syntaxText.ToString()));
            this.GenerateSyntaxWrappers(in context, syntaxData);
        }

        private void GenerateSyntaxWrappers(in GeneratorExecutionContext context, SyntaxData syntaxData)
        {
            foreach (var node in syntaxData.Nodes)
            {
                if (node.WrapperName is not null)
                {
                    this.GenerateSyntaxWrapper(in context, syntaxData, node);
                }
            }
        }

        private void GenerateSyntaxWrapper(in GeneratorExecutionContext context, SyntaxData syntaxData, NodeData nodeData)
        {
            var concreteBase = syntaxData.TryGetConcreteBase(nodeData)?.Name ?? nameof(SyntaxNode);

            var members = SyntaxFactory.List<MemberDeclarationSyntax>();

            // internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax";
            members = members.Add(SyntaxFactory.FieldDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.ConstKeyword)),
                declaration: SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(
                        identifier: SyntaxFactory.Identifier("WrappedTypeName"),
                        argumentList: null,
                        initializer: SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("Microsoft.CodeAnalysis.CSharp.Syntax." + nodeData.Name))))))));

            // private static readonly Type WrappedType;
            members = members.Add(SyntaxFactory.FieldDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                declaration: SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.IdentifierName("Type"),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator("WrappedType")))));

            // private readonly SyntaxNode node;
            members = members.Add(SyntaxFactory.FieldDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                declaration: SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.IdentifierName(concreteBase),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator("node")))));

            // private SyntaxNodeWrapper(SyntaxNode node)
            // {
            //     this.node = node;
            // }
            members = members.Add(SyntaxFactory.ConstructorDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)),
                identifier: SyntaxFactory.Identifier(nodeData.WrapperName),
                parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                    attributeLists: default,
                    modifiers: default,
                    type: SyntaxFactory.IdentifierName(concreteBase),
                    identifier: SyntaxFactory.Identifier("node"),
                    @default: null))),
                initializer: null,
                body: SyntaxFactory.Block(
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        left: SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.ThisExpression(),
                            name: SyntaxFactory.IdentifierName("node")),
                        right: SyntaxFactory.IdentifierName("node"))))));

            // public SyntaxNode SyntaxNode => this.node;
            members = members.Add(SyntaxFactory.PropertyDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                type: SyntaxFactory.IdentifierName(concreteBase),
                explicitInterfaceSpecifier: null,
                identifier: SyntaxFactory.Identifier("SyntaxNode"),
                accessorList: null,
                expressionBody: SyntaxFactory.ArrowExpressionClause(SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    expression: SyntaxFactory.ThisExpression(),
                    name: SyntaxFactory.IdentifierName("node"))),
                initializer: null,
                semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            // public static implicit operator CSharpSyntaxNode(SyntaxWrapper wrapper)
            // {
            //     return wrapper.node;
            // }
            members = members.Add(SyntaxFactory.ConversionOperatorDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                implicitOrExplicitKeyword: SyntaxFactory.Token(SyntaxKind.ImplicitKeyword),
                operatorKeyword: SyntaxFactory.Token(SyntaxKind.OperatorKeyword),
                type: SyntaxFactory.IdentifierName(concreteBase),
                parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                    attributeLists: default,
                    modifiers: default,
                    type: SyntaxFactory.IdentifierName(nodeData.WrapperName),
                    identifier: SyntaxFactory.Identifier("wrapper"),
                    @default: null))),
                body: SyntaxFactory.Block(SyntaxFactory.ReturnStatement(SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    expression: SyntaxFactory.IdentifierName("wrapper"),
                    name: SyntaxFactory.IdentifierName("node")))),
                expressionBody: null,
                semicolonToken: default));

            // public static bool IsInstance(SyntaxNode node)
            // {
            //     return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
            // }
            members = members.Add(SyntaxFactory.MethodDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                returnType: SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                explicitInterfaceSpecifier: null,
                identifier: SyntaxFactory.Identifier("IsInstance"),
                typeParameterList: null,
                parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                    attributeLists: default,
                    modifiers: default,
                    type: SyntaxFactory.IdentifierName("SyntaxNode"),
                    identifier: SyntaxFactory.Identifier("node"),
                    @default: null))),
                constraintClauses: default,
                body: SyntaxFactory.Block(
                    SyntaxFactory.ReturnStatement(SyntaxFactory.BinaryExpression(
                        SyntaxKind.LogicalAndExpression,
                        left: SyntaxFactory.BinaryExpression(
                            SyntaxKind.NotEqualsExpression,
                            left: SyntaxFactory.IdentifierName("node"),
                            right: SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                        right: SyntaxFactory.InvocationExpression(
                            expression: SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.IdentifierName("LightupHelpers"),
                                name: SyntaxFactory.IdentifierName("CanWrapNode")),
                            argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                                new[]
                                {
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("node")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("WrappedType")),
                                })))))),
                expressionBody: null));

            var wrapperStruct = SyntaxFactory.StructDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword)),
                identifier: SyntaxFactory.Identifier(nodeData.WrapperName),
                typeParameterList: null,
                baseList: SyntaxFactory.BaseList(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.GenericName(
                        identifier: SyntaxFactory.Identifier("ISyntaxWrapper"),
                        typeArgumentList: SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList<TypeSyntax>(SyntaxFactory.IdentifierName(concreteBase))))))),
                constraintClauses: default,
                members: members);
            var wrapperNamespace = SyntaxFactory.NamespaceDeclaration(
                name: SyntaxFactory.ParseName("StyleCop.Analyzers.Lightup"),
                externs: default,
                usings: SyntaxFactory.List<UsingDirectiveSyntax>()
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Immutable")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.CodeAnalysis")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.CodeAnalysis.CSharp")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.CodeAnalysis.CSharp.Syntax"))),
                members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(wrapperStruct));

            wrapperNamespace = wrapperNamespace
                .NormalizeWhitespace()
                .WithLeadingTrivia(
                    SyntaxFactory.Comment("// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved."),
                    SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment("// Licensed under the MIT License. See LICENSE in the project root for license information."),
                    SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.CarriageReturnLineFeed)
                .WithTrailingTrivia(
                    SyntaxFactory.CarriageReturnLineFeed);

            context.AddSource(nodeData.WrapperName + ".g.cs", SourceText.From(wrapperNamespace.ToFullString(), Encoding.UTF8));
        }

        private sealed class SyntaxData
        {
            private readonly Dictionary<string, NodeData> nameToNode;

            public SyntaxData(in GeneratorExecutionContext context, XDocument document)
            {
                var nodesBuilder = ImmutableArray.CreateBuilder<NodeData>();
                foreach (var element in document.XPathSelectElement("/Tree[@Root='SyntaxNode']").XPathSelectElements("PredefinedNode|AbstractNode|Node"))
                {
                    nodesBuilder.Add(new NodeData(in context, element));
                }

                this.Nodes = nodesBuilder.ToImmutable();
                this.nameToNode = this.Nodes.ToDictionary(node => node.Name);
            }

            public ImmutableArray<NodeData> Nodes { get; }

            public NodeData TryGetConcreteBase(NodeData node)
            {
                for (var current = this.TryGetNode(node.BaseName); current is not null; current = this.TryGetNode(current.BaseName))
                {
                    if (current.WrapperName is null)
                    {
                        // This is not a wrapper
                        return current;
                    }
                }

                return null;
            }

            private NodeData TryGetNode(string name)
            {
                this.nameToNode.TryGetValue(name, out var node);
                return node;
            }
        }

        private sealed class NodeData
        {
            public NodeData(in GeneratorExecutionContext context, XElement element)
            {
                this.Kind = element.Name.LocalName switch
                {
                    "PredefinedNode" => NodeKind.Predefined,
                    "AbstractNode" => NodeKind.Abstract,
                    "Node" => NodeKind.Concrete,
                    _ => throw new NotSupportedException($"Unknown element name '{element.Name}'"),
                };

                this.Name = element.Attribute("Name").Value;

                var existingType = context.Compilation.GetTypeByMetadataName($"Microsoft.CodeAnalysis.CSharp.Syntax.{this.Name}")
                    ?? context.Compilation.GetTypeByMetadataName($"Microsoft.CodeAnalysis.CSharp.{this.Name}")
                    ?? context.Compilation.GetTypeByMetadataName($"Microsoft.CodeAnalysis.{this.Name}");
                if (existingType?.DeclaredAccessibility == Accessibility.Public)
                {
                    this.WrapperName = null;
                }
                else
                {
                    this.WrapperName = this.Name + "Wrapper";
                }

                this.BaseName = element.Attribute("Base").Value;
                this.Fields = element.XPathSelectElements("Field").Select(field => new FieldData(field)).ToImmutableArray();
            }

            public NodeKind Kind { get; }

            public string Name { get; }

            public string WrapperName { get; }

            public string BaseName { get; }

            public ImmutableArray<FieldData> Fields { get; }
        }

        private sealed class FieldData
        {
            public FieldData(XElement element)
            {
                this.Name = element.Attribute("Name").Value;
                this.Type = element.Attribute("Type").Value;
                this.IsOverride = element.Attribute("Override")?.Value == "true";
            }

            public string Name { get; }

            public string Type { get; }

            public bool IsOverride { get; }
        }
    }
}
