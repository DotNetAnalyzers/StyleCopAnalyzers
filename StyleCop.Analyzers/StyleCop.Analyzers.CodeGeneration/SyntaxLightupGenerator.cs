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
            this.GenerateSyntaxWrapperHelper(in context, syntaxData.Nodes);
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

            // public static explicit operator WhenClauseSyntaxWrapper(SyntaxNode node)
            // {
            //     if (node == null)
            //     {
            //         return default;
            //     }
            //
            //     if (!IsInstance(node))
            //     {
            //         throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            //     }
            //
            //     return new WhenClauseSyntaxWrapper((CSharpSyntaxNode)node);
            // }
            members = members.Add(SyntaxFactory.ConversionOperatorDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                implicitOrExplicitKeyword: SyntaxFactory.Token(SyntaxKind.ExplicitKeyword),
                operatorKeyword: SyntaxFactory.Token(SyntaxKind.OperatorKeyword),
                type: SyntaxFactory.IdentifierName(nodeData.WrapperName),
                parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                    attributeLists: default,
                    modifiers: default,
                    type: SyntaxFactory.IdentifierName("SyntaxNode"),
                    identifier: SyntaxFactory.Identifier("node"),
                    @default: null))),
                body: SyntaxFactory.Block(
                    SyntaxFactory.IfStatement(
                        condition: SyntaxFactory.BinaryExpression(
                            SyntaxKind.EqualsExpression,
                            left: SyntaxFactory.IdentifierName("node"),
                            right: SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                        statement: SyntaxFactory.Block(
                            SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression)))),
                    SyntaxFactory.IfStatement(
                        condition: SyntaxFactory.PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            operand: SyntaxFactory.InvocationExpression(
                                expression: SyntaxFactory.IdentifierName("IsInstance"),
                                argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("node")))))),
                        statement: SyntaxFactory.Block(
                            SyntaxFactory.ThrowStatement(SyntaxFactory.ObjectCreationExpression(
                                type: SyntaxFactory.IdentifierName("InvalidCastException"),
                                argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                                    SyntaxFactory.InterpolatedStringExpression(
                                        SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken),
                                        SyntaxFactory.List(new InterpolatedStringContentSyntax[]
                                        {
                                            SyntaxFactory.InterpolatedStringText(SyntaxFactory.Token(
                                                leading: default,
                                                SyntaxKind.InterpolatedStringTextToken,
                                                "Cannot cast '",
                                                "Cannot cast '",
                                                trailing: default)),
                                            SyntaxFactory.Interpolation(SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                expression: SyntaxFactory.InvocationExpression(
                                                    expression: SyntaxFactory.MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        expression: SyntaxFactory.IdentifierName("node"),
                                                        name: SyntaxFactory.IdentifierName("GetType")),
                                                    argumentList: SyntaxFactory.ArgumentList()),
                                                name: SyntaxFactory.IdentifierName("FullName"))),
                                            SyntaxFactory.InterpolatedStringText(SyntaxFactory.Token(
                                                leading: default,
                                                SyntaxKind.InterpolatedStringTextToken,
                                                "' to '",
                                                "' to '",
                                                trailing: default)),
                                            SyntaxFactory.Interpolation(SyntaxFactory.IdentifierName("WrappedTypeName")),
                                            SyntaxFactory.InterpolatedStringText(SyntaxFactory.Token(
                                                leading: default,
                                                SyntaxKind.InterpolatedStringTextToken,
                                                "'",
                                                "'",
                                                trailing: default)),
                                        }))))),
                                initializer: null)))),
                    SyntaxFactory.ReturnStatement(SyntaxFactory.ObjectCreationExpression(
                        type: SyntaxFactory.IdentifierName(nodeData.WrapperName),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                            SyntaxFactory.CastExpression(
                                type: SyntaxFactory.IdentifierName(concreteBase),
                                expression: SyntaxFactory.IdentifierName("node"))))),
                        initializer: null))),
                expressionBody: null,
                semicolonToken: default));

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

            if (nodeData.Kind == NodeKind.Abstract)
            {
                // internal static SyntaxWrapper FromUpcast(CSharpSyntaxNode node)
                // {
                //     return new SyntaxWrapper(node);
                // }
                members = members.Add(SyntaxFactory.MethodDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                    returnType: SyntaxFactory.IdentifierName(nodeData.WrapperName),
                    explicitInterfaceSpecifier: null,
                    identifier: SyntaxFactory.Identifier("FromUpcast"),
                    typeParameterList: null,
                    parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                        attributeLists: default,
                        modifiers: default,
                        type: SyntaxFactory.IdentifierName(concreteBase),
                        identifier: SyntaxFactory.Identifier("node"),
                        @default: null))),
                    constraintClauses: default,
                    body: SyntaxFactory.Block(
                        SyntaxFactory.ReturnStatement(SyntaxFactory.ObjectCreationExpression(
                            type: SyntaxFactory.IdentifierName(nodeData.WrapperName),
                            argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("node")))),
                            initializer: null))),
                    expressionBody: null));
            }

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

        private void GenerateSyntaxWrapperHelper(in GeneratorExecutionContext context, ImmutableArray<NodeData> wrapperTypes)
        {
            // private static readonly ImmutableDictionary<Type, Type> WrappedTypes;
            var wrappedTypes = SyntaxFactory.FieldDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                declaration: SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.GenericName(
                        identifier: SyntaxFactory.Identifier("ImmutableDictionary"),
                        typeArgumentList: SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SeparatedList<TypeSyntax>(
                                new[]
                                {
                                    SyntaxFactory.IdentifierName("Type"),
                                    SyntaxFactory.IdentifierName("Type"),
                                }))),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("WrappedTypes")))));

            // var csharpCodeAnalysisAssembly = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly;
            // var builder = ImmutableDictionary.CreateBuilder<Type, Type>();
            var staticCtorStatements = SyntaxFactory.List<StatementSyntax>()
                .Add(SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.IdentifierName("var"),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(
                        identifier: SyntaxFactory.Identifier("csharpCodeAnalysisAssembly"),
                        argumentList: null,
                        initializer: SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        expression: SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName("CSharpSyntaxNode")),
                                        name: SyntaxFactory.IdentifierName("GetTypeInfo"))),
                                name: SyntaxFactory.IdentifierName("Assembly"))))))))
                .Add(SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.IdentifierName("var"),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(
                        identifier: SyntaxFactory.Identifier("builder"),
                        argumentList: null,
                        initializer: SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    expression: SyntaxFactory.IdentifierName("ImmutableDictionary"),
                                    name: SyntaxFactory.GenericName(
                                        identifier: SyntaxFactory.Identifier("CreateBuilder"),
                                        typeArgumentList: SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SeparatedList<TypeSyntax>(
                                                new[]
                                                {
                                                    SyntaxFactory.IdentifierName("Type"),
                                                    SyntaxFactory.IdentifierName("Type"),
                                                })))))))))));

            foreach (var node in wrapperTypes.OrderBy(node => node.Name, StringComparer.OrdinalIgnoreCase))
            {
                if (node.WrapperName is null)
                {
                    continue;
                }

                if (node.Name == nameof(CommonForEachStatementSyntax))
                {
                    // Prior to C# 7, ForEachStatementSyntax was the base type for all foreach statements. If
                    // the CommonForEachStatementSyntax type isn't found at runtime, we fall back to using this type instead.
                    //
                    // var forEachStatementSyntaxType = csharpCodeAnalysisAssembly.GetType(CommonForEachStatementSyntaxWrapper.WrappedTypeName)
                    //     ?? csharpCodeAnalysisAssembly.GetType(CommonForEachStatementSyntaxWrapper.FallbackWrappedTypeName);
                    staticCtorStatements = staticCtorStatements.Add(
                        SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                            type: SyntaxFactory.IdentifierName("var"),
                            variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(
                                identifier: SyntaxFactory.Identifier("forEachStatementSyntaxType"),
                                argumentList: null,
                                initializer: SyntaxFactory.EqualsValueClause(
                                    SyntaxFactory.BinaryExpression(
                                        SyntaxKind.CoalesceExpression,
                                        left: SyntaxFactory.InvocationExpression(
                                            expression: SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                expression: SyntaxFactory.IdentifierName("csharpCodeAnalysisAssembly"),
                                                name: SyntaxFactory.IdentifierName("GetType")),
                                            argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    expression: SyntaxFactory.IdentifierName(node.WrapperName),
                                                    name: SyntaxFactory.IdentifierName("WrappedTypeName")))))),
                                        right: SyntaxFactory.InvocationExpression(
                                            expression: SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                expression: SyntaxFactory.IdentifierName("csharpCodeAnalysisAssembly"),
                                                name: SyntaxFactory.IdentifierName("GetType")),
                                            argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    expression: SyntaxFactory.IdentifierName(node.WrapperName),
                                                    name: SyntaxFactory.IdentifierName("FallbackWrappedTypeName")))))))))))));

                    // builder.Add(typeof(CommonForEachStatementSyntaxWrapper), forEachStatementSyntaxType);
                    staticCtorStatements = staticCtorStatements.Add(SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.InvocationExpression(
                            expression: SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.IdentifierName("builder"),
                                name: SyntaxFactory.IdentifierName("Add")),
                            argumentList: SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList(
                                    new[]
                                    {
                                        SyntaxFactory.Argument(SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(node.WrapperName))),
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("forEachStatementSyntaxType")),
                                    })))));

                    continue;
                }

                // builder.Add(typeof(ConstantPatternSyntaxWrapper), csharpCodeAnalysisAssembly.GetType(ConstantPatternSyntaxWrapper.WrappedTypeName));
                staticCtorStatements = staticCtorStatements.Add(SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.IdentifierName("builder"),
                            name: SyntaxFactory.IdentifierName("Add")),
                        argumentList: SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList(
                                new[]
                                {
                                    SyntaxFactory.Argument(SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(node.WrapperName))),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.InvocationExpression(
                                            expression: SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                expression: SyntaxFactory.IdentifierName("csharpCodeAnalysisAssembly"),
                                                name: SyntaxFactory.IdentifierName("GetType")),
                                            argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    expression: SyntaxFactory.IdentifierName(node.WrapperName),
                                                    name: SyntaxFactory.IdentifierName("WrappedTypeName"))))))),
                                })))));
            }

            // WrappedTypes = builder.ToImmutable();
            staticCtorStatements = staticCtorStatements.Add(SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    left: SyntaxFactory.IdentifierName("WrappedTypes"),
                    right: SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.IdentifierName("builder"),
                            name: SyntaxFactory.IdentifierName("ToImmutable"))))));

            var staticCtor = SyntaxFactory.ConstructorDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                identifier: SyntaxFactory.Identifier("SyntaxWrapperHelper"),
                parameterList: SyntaxFactory.ParameterList(),
                initializer: null,
                body: SyntaxFactory.Block(staticCtorStatements));

            // internal static Type GetWrappedType(Type wrapperType)
            // {
            //     if (WrappedTypes.TryGetValue(wrapperType, out Type wrappedType))
            //     {
            //         return wrappedType;
            //     }
            //
            //     return null;
            // }
            var getWrappedType = SyntaxFactory.MethodDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                returnType: SyntaxFactory.IdentifierName("Type"),
                explicitInterfaceSpecifier: null,
                identifier: SyntaxFactory.Identifier("GetWrappedType"),
                typeParameterList: null,
                parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                    attributeLists: default,
                    modifiers: default,
                    type: SyntaxFactory.IdentifierName("Type"),
                    identifier: SyntaxFactory.Identifier("wrapperType"),
                    @default: null))),
                constraintClauses: default,
                body: SyntaxFactory.Block(
                    SyntaxFactory.IfStatement(
                        condition: SyntaxFactory.InvocationExpression(
                            expression: SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.IdentifierName("WrappedTypes"),
                                name: SyntaxFactory.IdentifierName("TryGetValue")),
                            argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                                new[]
                                {
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("wrapperType")),
                                    SyntaxFactory.Argument(
                                        nameColon: null,
                                        refKindKeyword: SyntaxFactory.Token(SyntaxKind.OutKeyword),
                                        expression: SyntaxFactory.DeclarationExpression(
                                            type: SyntaxFactory.IdentifierName("Type"),
                                            designation: SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("wrappedType")))),
                                }))),
                        statement: SyntaxFactory.Block(
                            SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("wrappedType")))),
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))),
                expressionBody: null);

            var wrapperHelperClass = SyntaxFactory.ClassDeclaration(
                attributeLists: default,
                modifiers: SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.InternalKeyword)).Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                identifier: SyntaxFactory.Identifier("SyntaxWrapperHelper"),
                typeParameterList: null,
                baseList: null,
                constraintClauses: default,
                members: SyntaxFactory.List<MemberDeclarationSyntax>()
                    .Add(wrappedTypes)
                    .Add(staticCtor)
                    .Add(getWrappedType));
            var wrapperNamespace = SyntaxFactory.NamespaceDeclaration(
                name: SyntaxFactory.ParseName("StyleCop.Analyzers.Lightup"),
                externs: default,
                usings: SyntaxFactory.List<UsingDirectiveSyntax>()
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Immutable")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Reflection")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.CodeAnalysis")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.CodeAnalysis.CSharp"))),
                members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(wrapperHelperClass));

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

            context.AddSource("SyntaxWrapperHelper.g.cs", SourceText.From(wrapperNamespace.ToFullString(), Encoding.UTF8));
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
