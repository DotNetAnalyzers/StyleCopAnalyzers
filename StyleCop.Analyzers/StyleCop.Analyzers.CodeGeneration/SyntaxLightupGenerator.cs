// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
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
    internal sealed class SyntaxLightupGenerator : IIncrementalGenerator
    {
        private enum NodeKind
        {
            Predefined,
            Abstract,
            Concrete,
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var compilation = context.CompilationProvider;
            var syntaxFiles = context.AdditionalTextsProvider.Where(static x => Path.GetFileName(x.Path) == "Syntax.xml");
            context.RegisterSourceOutput(
                syntaxFiles.Combine(compilation),
                (context, value) => this.Execute(in context, value.Right, value.Left));
        }

        private void Execute(in SourceProductionContext context, Compilation compilation, AdditionalText syntaxFile)
        {
            var syntaxText = syntaxFile.GetText(context.CancellationToken);
            if (syntaxText is null)
            {
                throw new InvalidOperationException("Failed to read Syntax.xml");
            }

            var syntaxData = new SyntaxData(compilation, XDocument.Parse(syntaxText.ToString()));
            this.GenerateSyntaxWrappers(in context, syntaxData);
            this.GenerateSyntaxWrapperHelper(in context, syntaxData.Nodes);
        }

        private void GenerateSyntaxWrappers(in SourceProductionContext context, SyntaxData syntaxData)
        {
            foreach (var node in syntaxData.Nodes)
            {
                this.GenerateSyntaxWrapper(in context, syntaxData, node);
            }
        }

        private void GenerateSyntaxWrapper(in SourceProductionContext context, SyntaxData syntaxData, NodeData nodeData)
        {
            if (nodeData.WrapperName is null)
            {
                // No need to generate a wrapper for this type
                return;
            }

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

            bool first = true;
            foreach (var field in nodeData.Fields)
            {
                if (field.IsSkipped)
                {
                    continue;
                }

                if (field.IsOverride)
                {
                    // The 'get' accessor is skipped for override fields
                    continue;
                }

                // private static readonly Func<CSharpSyntaxNode, T> FieldAccessor;
                FieldDeclarationSyntax fieldAccessor = SyntaxFactory.FieldDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                    declaration: SyntaxFactory.VariableDeclaration(
                        type: SyntaxFactory.GenericName(
                            identifier: SyntaxFactory.Identifier("Func"),
                            typeArgumentList: SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(
                                new[]
                                {
                                    SyntaxFactory.IdentifierName(concreteBase),
                                    SyntaxFactory.ParseTypeName(field.GetAccessorResultType(syntaxData)),
                                }))),
                        variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(field.AccessorName))));

                if (first)
                {
                    fieldAccessor = fieldAccessor.WithLeadingBlankLine();
                    first = false;
                }

                members = members.Add(fieldAccessor);
            }

            foreach (var field in nodeData.Fields)
            {
                if (field.IsSkipped)
                {
                    continue;
                }

                // private static readonly Func<CSharpSyntaxNode, T, CSharpSyntaxNode> WithFieldAccessor;
                members = members.Add(SyntaxFactory.FieldDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                    declaration: SyntaxFactory.VariableDeclaration(
                        type: SyntaxFactory.GenericName(
                            identifier: SyntaxFactory.Identifier("Func"),
                            typeArgumentList: SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(
                                new[]
                                {
                                    SyntaxFactory.IdentifierName(concreteBase),
                                    SyntaxFactory.ParseTypeName(field.GetAccessorResultType(syntaxData)),
                                    SyntaxFactory.IdentifierName(concreteBase),
                                }))),
                        variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(field.WithAccessorName)))));
            }

            // private readonly SyntaxNode node;
            members = members.Add(SyntaxFactory.FieldDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                declaration: SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.IdentifierName(concreteBase),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator("node")))).WithLeadingBlankLine());

            // WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(SyntaxWrapper));
            var staticCtorStatements = SyntaxFactory.SingletonList<StatementSyntax>(
                SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    left: SyntaxFactory.IdentifierName("WrappedType"),
                    right: SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.IdentifierName("SyntaxWrapperHelper"),
                            name: SyntaxFactory.IdentifierName("GetWrappedType")),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                            SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(nodeData.WrapperName)))))))));

            foreach (var field in nodeData.Fields)
            {
                if (field.IsSkipped)
                {
                    continue;
                }

                if (field.IsOverride)
                {
                    // The 'get' accessor is skipped for override fields
                    continue;
                }

                SimpleNameSyntax helperName;
                if (field.IsWrappedSeparatedSyntaxList(syntaxData, out var elementNode))
                {
                    Debug.Assert(elementNode.WrapperName is not null, $"Assertion failed: {nameof(elementNode)}.{nameof(elementNode.WrapperName)} is not null");

                    // CreateSeparatedSyntaxListPropertyAccessor<SyntaxNode, T>
                    helperName = SyntaxFactory.GenericName(
                        identifier: SyntaxFactory.Identifier("CreateSeparatedSyntaxListPropertyAccessor"),
                        typeArgumentList: SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(
                            new[]
                            {
                                SyntaxFactory.IdentifierName(concreteBase),
                                SyntaxFactory.IdentifierName(elementNode.WrapperName),
                            })));
                }
                else
                {
                    // CreateSyntaxPropertyAccessor<SyntaxNode, T>
                    helperName = SyntaxFactory.GenericName(
                        identifier: SyntaxFactory.Identifier("CreateSyntaxPropertyAccessor"),
                        typeArgumentList: SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(
                            new[]
                            {
                                SyntaxFactory.IdentifierName(concreteBase),
                                SyntaxFactory.ParseTypeName(field.GetAccessorResultType(syntaxData)),
                            })));
                }

                // ReturnTypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, TypeSyntax>(WrappedType, nameof(ReturnType));
                staticCtorStatements = staticCtorStatements.Add(SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    left: SyntaxFactory.IdentifierName(field.AccessorName),
                    right: SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.IdentifierName("LightupHelpers"),
                            name: helperName),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                            new[]
                            {
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("WrappedType")),
                                SyntaxFactory.Argument(SyntaxFactory.InvocationExpression(
                                    expression: SyntaxFactory.IdentifierName("nameof"),
                                    argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(field.Name)))))),
                            }))))));
            }

            foreach (var field in nodeData.Fields)
            {
                if (field.IsSkipped)
                {
                    continue;
                }

                SimpleNameSyntax helperName;
                if (field.IsWrappedSeparatedSyntaxList(syntaxData, out var elementNode))
                {
                    Debug.Assert(elementNode.WrapperName is not null, $"Assertion failed: {nameof(elementNode)}.{nameof(elementNode.WrapperName)} is not null");

                    // CreateSeparatedSyntaxListWithPropertyAccessor<SyntaxNode, T>
                    helperName = SyntaxFactory.GenericName(
                        identifier: SyntaxFactory.Identifier("CreateSeparatedSyntaxListWithPropertyAccessor"),
                        typeArgumentList: SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(
                            new[]
                            {
                                SyntaxFactory.IdentifierName(concreteBase),
                                SyntaxFactory.IdentifierName(elementNode.WrapperName),
                            })));
                }
                else
                {
                    // CreateSyntaxWithPropertyAccessor<SyntaxNode, T>
                    helperName = SyntaxFactory.GenericName(
                        identifier: SyntaxFactory.Identifier("CreateSyntaxWithPropertyAccessor"),
                        typeArgumentList: SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(
                            new[]
                            {
                                SyntaxFactory.IdentifierName(concreteBase),
                                SyntaxFactory.ParseTypeName(field.GetAccessorResultType(syntaxData)),
                            })));
                }

                // WithReturnTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, TypeSyntax>(WrappedType, nameof(ReturnType));
                staticCtorStatements = staticCtorStatements.Add(SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    left: SyntaxFactory.IdentifierName(field.WithAccessorName),
                    right: SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.IdentifierName("LightupHelpers"),
                            name: helperName),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                            new[]
                            {
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("WrappedType")),
                                SyntaxFactory.Argument(SyntaxFactory.InvocationExpression(
                                    expression: SyntaxFactory.IdentifierName("nameof"),
                                    argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(field.Name)))))),
                            }))))));
            }

            // static SyntaxWrapper()
            // {
            //     WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(SyntaxWrapper));
            // }
            members = members.Add(SyntaxFactory.ConstructorDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                identifier: SyntaxFactory.Identifier(nodeData.WrapperName),
                parameterList: SyntaxFactory.ParameterList(),
                initializer: null,
                body: SyntaxFactory.Block(staticCtorStatements),
                expressionBody: null).WithLeadingBlankLine());

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
                        right: SyntaxFactory.IdentifierName("node")))),
                expressionBody: null));

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

            // public T Field
            // {
            //     get
            //     {
            //         return ...;
            //     }
            // }
            first = true;
            foreach (var field in nodeData.Fields)
            {
                if (field.IsSkipped)
                {
                    continue;
                }

                TypeSyntax propertyType = SyntaxFactory.ParseTypeName(field.GetAccessorResultType(syntaxData));
                ExpressionSyntax returnExpression;
                if (field.IsOverride)
                {
                    var declaringNode = field.GetDeclaringNode(syntaxData);
                    if (declaringNode.WrapperName is not null)
                    {
                        // ((CommonForEachStatementSyntaxWrapper)this).OpenParenToken
                        returnExpression = SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.ParenthesizedExpression(
                                SyntaxFactory.CastExpression(
                                    type: SyntaxFactory.IdentifierName(declaringNode.WrapperName ?? declaringNode.Name),
                                    expression: SyntaxFactory.ThisExpression())),
                            name: SyntaxFactory.IdentifierName(field.Name));
                    }
                    else
                    {
                        // this.SyntaxNode.OpenParenToken
                        returnExpression = SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.ThisExpression(),
                                name: SyntaxFactory.IdentifierName("SyntaxNode")),
                            name: SyntaxFactory.IdentifierName(field.Name));

                        if (declaringNode.TryGetField(field.Name) is { IsExtensionField: true })
                        {
                            // this.SyntaxNode.OpenParenToken()
                            returnExpression = SyntaxFactory.InvocationExpression(
                                expression: returnExpression,
                                argumentList: SyntaxFactory.ArgumentList());
                        }
                    }
                }
                else if (field.IsWrappedSeparatedSyntaxList(syntaxData, out var elementNode))
                {
                    // PatternAccessor(this.SyntaxNode)
                    returnExpression = SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.IdentifierName(field.AccessorName),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.ThisExpression(),
                                name: SyntaxFactory.IdentifierName("SyntaxNode"))))));
                }
                else if (syntaxData.TryGetNode(field.Type) is { } fieldNodeType)
                {
                    // PatternAccessor(this.SyntaxNode)
                    returnExpression = SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.IdentifierName(field.AccessorName),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.ThisExpression(),
                                name: SyntaxFactory.IdentifierName("SyntaxNode"))))));

                    if (fieldNodeType.WrapperName is not null)
                    {
                        // (PatternSyntaxWrapper)...
                        propertyType = SyntaxFactory.IdentifierName(fieldNodeType.WrapperName);
                        returnExpression = SyntaxFactory.CastExpression(
                            type: SyntaxFactory.IdentifierName(fieldNodeType.WrapperName),
                            expression: returnExpression);
                    }
                }
                else
                {
                    // PatternAccessor(this.SyntaxNode)
                    returnExpression = SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.IdentifierName(field.AccessorName),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.ThisExpression(),
                                name: SyntaxFactory.IdentifierName("SyntaxNode"))))));
                }

                // public T Field
                // {
                //     get
                //     {
                //         return ...;
                //     }
                // }
                PropertyDeclarationSyntax property = SyntaxFactory.PropertyDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                    type: propertyType,
                    explicitInterfaceSpecifier: null,
                    identifier: SyntaxFactory.Identifier(field.Name),
                    accessorList: SyntaxFactory.AccessorList(SyntaxFactory.SingletonList(SyntaxFactory.AccessorDeclaration(
                        SyntaxKind.GetAccessorDeclaration,
                        SyntaxFactory.Block(
                            SyntaxFactory.ReturnStatement(returnExpression))))),
                    expressionBody: null,
                    initializer: null,
                    semicolonToken: default);

                if (first)
                {
                    property = property.WithLeadingBlankLine();
                    first = false;
                }

                members = members.Add(property);
            }

            for (var baseNode = syntaxData.TryGetNode(nodeData.BaseName); baseNode?.WrapperName is not null; baseNode = syntaxData.TryGetNode(baseNode.BaseName))
            {
                // public static explicit operator SyntaxWrapper(BaseSyntaxWrapper node)
                // {
                //     return (SyntaxWrapper)node.SyntaxNode;
                // }
                ConversionOperatorDeclarationSyntax wrapperConversion = SyntaxFactory.ConversionOperatorDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                    implicitOrExplicitKeyword: SyntaxFactory.Token(SyntaxKind.ExplicitKeyword),
                    operatorKeyword: SyntaxFactory.Token(SyntaxKind.OperatorKeyword),
                    type: SyntaxFactory.IdentifierName(nodeData.WrapperName),
                    parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                        attributeLists: default,
                        modifiers: default,
                        type: SyntaxFactory.IdentifierName(baseNode.WrapperName),
                        identifier: SyntaxFactory.Identifier("node"),
                        @default: null))),
                    body: SyntaxFactory.Block(SyntaxFactory.ReturnStatement(
                        SyntaxFactory.CastExpression(
                            type: SyntaxFactory.IdentifierName(nodeData.WrapperName),
                            expression: SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.IdentifierName("node"),
                                name: SyntaxFactory.IdentifierName("SyntaxNode"))))),
                    expressionBody: null,
                    semicolonToken: default);

                if (first)
                {
                    wrapperConversion = wrapperConversion.WithLeadingBlankLine();
                    first = false;
                }

                members = members.Add(wrapperConversion);
            }

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
            var nodeConversion = SyntaxFactory.ConversionOperatorDeclaration(
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
                semicolonToken: default);

            if (first)
            {
                nodeConversion = nodeConversion.WithLeadingBlankLine();
                first = false;
            }

            members = members.Add(nodeConversion);

            for (var baseNode = syntaxData.TryGetNode(nodeData.BaseName); baseNode?.WrapperName is not null; baseNode = syntaxData.TryGetNode(baseNode.BaseName))
            {
                // public static implicit operator BaseSyntaxWrapper(SyntaxWrapper wrapper)
                // {
                //     return BaseSyntaxWrapper.FromUpcast(wrapper.node);
                // }
                members = members.Add(SyntaxFactory.ConversionOperatorDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                    implicitOrExplicitKeyword: SyntaxFactory.Token(SyntaxKind.ImplicitKeyword),
                    operatorKeyword: SyntaxFactory.Token(SyntaxKind.OperatorKeyword),
                    type: SyntaxFactory.IdentifierName(baseNode.WrapperName),
                    parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                        attributeLists: default,
                        modifiers: default,
                        type: SyntaxFactory.IdentifierName(nodeData.WrapperName),
                        identifier: SyntaxFactory.Identifier("wrapper"),
                        @default: null))),
                    body: SyntaxFactory.Block(SyntaxFactory.ReturnStatement(
                        SyntaxFactory.InvocationExpression(
                            expression: SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.IdentifierName(baseNode.WrapperName),
                                name: SyntaxFactory.IdentifierName("FromUpcast")),
                            argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    expression: SyntaxFactory.IdentifierName("wrapper"),
                                    name: SyntaxFactory.IdentifierName("node")))))))),
                    expressionBody: null,
                    semicolonToken: default));
            }

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

            foreach (var field in nodeData.Fields)
            {
                if (field.IsSkipped)
                {
                    continue;
                }

                string valueName = char.ToLowerInvariant(field.Name[0]) + field.Name.Substring(1);

                ExpressionSyntax convertedValue = SyntaxFactory.IdentifierName(valueName);

                TypeSyntax propertyType = SyntaxFactory.ParseTypeName(field.GetAccessorResultType(syntaxData));
                if (syntaxData.TryGetNode(field.Type) is { WrapperName: { } wrapperName })
                {
                    propertyType = SyntaxFactory.IdentifierName(wrapperName);
                }

                ExpressionSyntax returnExpression = SyntaxFactory.InvocationExpression(
                    expression: SyntaxFactory.IdentifierName(field.WithAccessorName),
                    argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                        new[]
                        {
                            SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.ThisExpression(),
                                name: SyntaxFactory.IdentifierName("SyntaxNode"))),
                            SyntaxFactory.Argument(convertedValue),
                        })));

                // public SyntaxWrapper WithField(T value)
                // {
                //     return new SyntaxWrapper(...);
                // }
                members = members.Add(SyntaxFactory.MethodDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                    returnType: SyntaxFactory.IdentifierName(nodeData.WrapperName),
                    explicitInterfaceSpecifier: null,
                    identifier: SyntaxFactory.Identifier("With" + field.Name),
                    typeParameterList: null,
                    parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                        attributeLists: default,
                        modifiers: default,
                        type: propertyType,
                        identifier: SyntaxFactory.Identifier(valueName),
                        @default: null))),
                    constraintClauses: default,
                    body: SyntaxFactory.Block(SyntaxFactory.ReturnStatement(
                        SyntaxFactory.ObjectCreationExpression(
                            type: SyntaxFactory.IdentifierName(nodeData.WrapperName),
                            argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(returnExpression))),
                            initializer: null))),
                    expressionBody: null,
                    semicolonToken: default));
            }

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

        private void GenerateSyntaxWrapperHelper(in SourceProductionContext context, ImmutableArray<NodeData> wrapperTypes)
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

                if (node.Name == nameof(BaseObjectCreationExpressionSyntax))
                {
                    // Prior to C# 9, ObjectCreationExpressionSyntax was the base type for all object creation
                    // statements. If the BaseObjectCreationExpressionSyntax type isn't found at runtime, we fall back
                    // to using this type instead.
                    //
                    // var objectCreationExpressionSyntaxType = csharpCodeAnalysisAssembly.GetType(BaseObjectCreationExpressionSyntaxWrapper.WrappedTypeName)
                    //     ?? csharpCodeAnalysisAssembly.GetType(BaseObjectCreationExpressionSyntaxWrapper.FallbackWrappedTypeName);
                    LocalDeclarationStatementSyntax localStatement =
                        SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                            type: SyntaxFactory.IdentifierName("var"),
                            variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(
                                identifier: SyntaxFactory.Identifier("objectCreationExpressionSyntaxType"),
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
                                                    name: SyntaxFactory.IdentifierName("FallbackWrappedTypeName"))))))))))));

                    // This is the first line of the statements that initialize 'builder', so start it with a blank line
                    staticCtorStatements = staticCtorStatements.Add(localStatement.WithLeadingBlankLine());

                    // builder.Add(typeof(BaseObjectCreationExpressionSyntaxWrapper), objectCreationExpressionSyntaxType);
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
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName("objectCreationExpressionSyntaxType")),
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
                            name: SyntaxFactory.IdentifierName("ToImmutable"))))).WithLeadingBlankLine());

            var staticCtor = SyntaxFactory.ConstructorDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                identifier: SyntaxFactory.Identifier("SyntaxWrapperHelper"),
                parameterList: SyntaxFactory.ParameterList(),
                initializer: null,
                body: SyntaxFactory.Block(staticCtorStatements),
                expressionBody: null).WithLeadingBlankLine();

            // /// <summary>
            // /// Gets the type that is wrapped by the given wrapper.
            // /// </summary>
            // /// <param name="wrapperType">Type of the wrapper for which the wrapped type should be retrieved.</param>
            // /// <returns>The wrapped type, or null if there is no info.</returns>
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

            getWrappedType = getWrappedType.WithLeadingTrivia(SyntaxFactory.TriviaList(
                SyntaxFactory.Trivia(SyntaxFactory.DocumentationComment(
                    SyntaxFactory.XmlText(" "),
                    SyntaxFactory.XmlSummaryElement(
                        SyntaxFactory.XmlNewLine(Environment.NewLine),
                        SyntaxFactory.XmlText(" Gets the type that is wrapped by the given wrapper."),
                        SyntaxFactory.XmlNewLine(Environment.NewLine),
                        SyntaxFactory.XmlText(" ")),
                    SyntaxFactory.XmlNewLine(Environment.NewLine),
                    SyntaxFactory.XmlText(" "),
                    SyntaxFactory.XmlParamElement(
                        "wrapperType",
                        SyntaxFactory.XmlText("Type of the wrapper for which the wrapped type should be retrieved.")),
                    SyntaxFactory.XmlNewLine(Environment.NewLine),
                    SyntaxFactory.XmlText(" "),
                    SyntaxFactory.XmlReturnsElement(
                        SyntaxFactory.XmlText("The wrapped type, or null if there is no info.")),
                    SyntaxFactory.XmlNewLine(Environment.NewLine).WithoutTrailingTrivia()))));

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

            public SyntaxData(Compilation compilation, XDocument document)
            {
                var nodesBuilder = ImmutableArray.CreateBuilder<NodeData>();
                foreach (var element in document.XPathSelectElement("/Tree[@Root='SyntaxNode']").XPathSelectElements("PredefinedNode|AbstractNode|Node"))
                {
                    nodesBuilder.Add(new NodeData(compilation, element));
                }

                this.Nodes = nodesBuilder.ToImmutable();
                this.nameToNode = this.Nodes.ToDictionary(node => node.Name);
            }

            public ImmutableArray<NodeData> Nodes { get; }

            public NodeData? TryGetConcreteType(NodeData? node)
            {
                for (var current = node; current is not null; current = this.TryGetNode(current.BaseName))
                {
                    if (current.WrapperName is null)
                    {
                        // This is not a wrapper
                        return current;
                    }
                }

                return null;
            }

            public NodeData? TryGetConcreteBase(NodeData node)
            {
                return this.TryGetConcreteType(this.TryGetNode(node.BaseName));
            }

            public NodeData? TryGetNode(string name)
            {
                this.nameToNode.TryGetValue(name, out var node);
                return node;
            }
        }

        private sealed class NodeData
        {
            public NodeData(Compilation compilation, XElement element)
            {
                this.Kind = element.Name.LocalName switch
                {
                    "PredefinedNode" => NodeKind.Predefined,
                    "AbstractNode" => NodeKind.Abstract,
                    "Node" => NodeKind.Concrete,
                    _ => throw new NotSupportedException($"Unknown element name '{element.Name}'"),
                };

                this.Name = element.Attribute("Name").Value;

                this.ExistingType = compilation.GetTypeByMetadataName($"Microsoft.CodeAnalysis.CSharp.Syntax.{this.Name}")
                    ?? compilation.GetTypeByMetadataName($"Microsoft.CodeAnalysis.CSharp.{this.Name}")
                    ?? compilation.GetTypeByMetadataName($"Microsoft.CodeAnalysis.{this.Name}");
                if (this.ExistingType?.DeclaredAccessibility == Accessibility.Public)
                {
                    this.WrapperName = null;
                }
                else
                {
                    this.WrapperName = this.Name + "Wrapper";
                }

                this.BaseName = element.Attribute("Base").Value;
                this.Fields = element.XPathSelectElements("descendant::Field").Select(field => new FieldData(this, field)).ToImmutableArray();
            }

            public NodeKind Kind { get; }

            public string Name { get; }

            public INamedTypeSymbol? ExistingType { get; }

            public string? WrapperName { get; }

            public string BaseName { get; }

            public ImmutableArray<FieldData> Fields { get; }

            internal FieldData? TryGetField(string name)
            {
                return this.Fields.SingleOrDefault(field => field.Name == name);
            }
        }

        private sealed class FieldData
        {
            private readonly NodeData nodeData;

            public FieldData(NodeData nodeData, XElement element)
            {
                this.nodeData = nodeData;

                this.Name = element.Attribute("Name").Value;

                var type = element.Attribute("Type").Value;
                this.Type = type switch
                {
                    "SyntaxList<SyntaxToken>" => nameof(SyntaxTokenList),
                    _ => type,
                };

                this.IsOverride = element.Attribute("Override")?.Value == "true";

                this.AccessorName = this.Name + "Accessor";
                this.WithAccessorName = "With" + this.Name + "Accessor";
            }

            public bool IsSkipped => false;

            public string Name { get; }

            public string AccessorName { get; }

            public string WithAccessorName { get; }

            public string Type { get; }

            public bool IsOverride { get; }

            /// <summary>
            /// Gets a value indicating whether this field is implemented as an extension method in the lightup layer.
            /// </summary>
            public bool IsExtensionField
            {
                get
                {
                    return this.nodeData.ExistingType is not null
                        && this.nodeData.ExistingType.GetMembers(this.Name).IsEmpty;
                }
            }

            public NodeData GetDeclaringNode(SyntaxData syntaxData)
            {
                for (var current = this.nodeData; current is not null; current = syntaxData.TryGetNode(current.BaseName))
                {
                    var currentField = current.TryGetField(this.Name);
                    if (currentField is { IsOverride: false })
                    {
                        return currentField.nodeData;
                    }
                }

                throw new NotSupportedException("Unable to find declaring node.");
            }

            public bool IsWrappedSeparatedSyntaxList(SyntaxData syntaxData, [NotNullWhen(true)] out NodeData? element)
            {
                if (this.Type.StartsWith("SeparatedSyntaxList<") && this.Type.EndsWith(">"))
                {
                    var elementTypeName = this.Type.Substring("SeparatedSyntaxList<".Length, this.Type.Length - "SeparatedSyntaxList<".Length - ">".Length);
                    var elementTypeNode = syntaxData.TryGetNode(elementTypeName);
                    if (elementTypeNode is { WrapperName: not null })
                    {
                        element = elementTypeNode;
                        return true;
                    }
                }

                element = null;
                return false;
            }

            public string GetAccessorResultType(SyntaxData syntaxData)
            {
                var typeNode = syntaxData.TryGetNode(this.Type);
                if (typeNode is not null)
                {
                    return syntaxData.TryGetConcreteType(typeNode)?.Name ?? nameof(SyntaxNode);
                }

                if (this.IsWrappedSeparatedSyntaxList(syntaxData, out var elementTypeNode))
                {
                    return $"SeparatedSyntaxListWrapper<{elementTypeNode.WrapperName}>";
                }

                return this.Type;
            }

            public string? GetAccessorResultElementType(SyntaxData syntaxData)
            {
                if (this.IsWrappedSeparatedSyntaxList(syntaxData, out var elementTypeNode))
                {
                    return elementTypeNode.WrapperName;
                }

                return null;
            }
        }
    }
}
