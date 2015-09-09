// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.MaintainabilityRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public class SA1400UnitTests : CodeFixVerifier
    {
        private const string Tab = "\t";

        [Fact]
        public async Task TestClassDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("class").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("class").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("class").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedClassDeclarationAsync()
        {
            await this.TestNestedTypeDeclarationAsync("class").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedClassDeclarationWithAttributesAsync()
        {
            await this.TestNestedTypeDeclarationWithAttributesAsync("class").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedClassDeclarationWithDirectivesAsync()
        {
            await this.TestNestedTypeDeclarationWithDirectivesAsync("class").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialClassDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("partial class", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialClassDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("partial class", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialClassDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("partial class", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("interface").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("interface").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("interface").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedInterfaceDeclarationAsync()
        {
            await this.TestNestedTypeDeclarationAsync("interface").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedInterfaceDeclarationWithAttributesAsync()
        {
            await this.TestNestedTypeDeclarationWithAttributesAsync("interface").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedInterfaceDeclarationWithDirectivesAsync()
        {
            await this.TestNestedTypeDeclarationWithDirectivesAsync("interface").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialInterfaceDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("partial interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialInterfaceDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("partial interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialInterfaceDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("partial interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("struct").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("struct").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("struct").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedStructDeclarationAsync()
        {
            await this.TestNestedTypeDeclarationAsync("struct").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedStructDeclarationWithAttributesAsync()
        {
            await this.TestNestedTypeDeclarationWithAttributesAsync("struct").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedStructDeclarationWithDirectivesAsync()
        {
            await this.TestNestedTypeDeclarationWithDirectivesAsync("struct").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialStructDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("partial struct", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialStructDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("partial struct", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialStructDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("partial struct", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumDeclarationAsync()
        {
            await this.TestTypeDeclarationAsync("enum").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumDeclarationWithAttributesAsync()
        {
            await this.TestTypeDeclarationWithAttributesAsync("enum").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumDeclarationWithDirectivesAsync()
        {
            await this.TestTypeDeclarationWithDirectivesAsync("enum").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedEnumDeclarationAsync()
        {
            await this.TestNestedTypeDeclarationAsync("enum").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedEnumDeclarationWithAttributesAsync()
        {
            await this.TestNestedTypeDeclarationWithAttributesAsync("enum").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedEnumDeclarationWithDirectivesAsync()
        {
            await this.TestNestedTypeDeclarationWithDirectivesAsync("enum").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationAsync()
        {
            await this.TestDeclarationAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationWithAttributesAsync()
        {
            await this.TestDeclarationWithAttributesAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationWithDirectivesAsync()
        {
            await this.TestDeclarationWithDirectivesAsync("internal", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedDelegateDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedDelegateDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedDelegateDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "TypeName", "delegate int TypeName", "  ( int\n parameter\n );").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", "{ add { } remove { } }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticEventDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticEventDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticEventDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static event EventHandler MemberName", "{ add { } remove { } }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceEventDeclarationAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { event EventHandler MemberName; }";
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceEventDeclarationWithAttributesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { event EventHandler MemberName; }";
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceEventDeclarationWithDirectivesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { event EventHandler MemberName; }";
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler IInterface.MemberName", "{ add { } remove { } }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n ) { }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static void MemberName", "  ( int\n parameter\n ) { }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceMethodDeclarationAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { void MemberName(int parameter); }";
            await this.TestNestedDeclarationAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceMethodDeclarationWithAttributesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { void MemberName(int parameter); }";
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceMethodDeclarationWithDirectivesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { void MemberName(int parameter); }";
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void IInterface.MemberName", "  ( int\n parameter\n ) { }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n );", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n );", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "void MemberName", "  ( int\n parameter\n );", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "partial void MemberName", "  ( int\n parameter\n );", containingType: "partial class", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "partial void MemberName", "  ( int\n parameter\n );", containingType: "partial class", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "partial void MemberName", "  ( int\n parameter\n );", containingType: "partial class", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticPropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticPropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticPropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static EventHandler MemberName", "{ get; set; }").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfacePropertyDeclarationAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { EventHandler MemberName { get; set; } }";
            await this.TestNestedDeclarationAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfacePropertyDeclarationWithAttributesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { EventHandler MemberName { get; set; } }";
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfacePropertyDeclarationWithDirectivesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { EventHandler MemberName { get; set; } }";
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler IInterface.MemberName", "{ get; set; }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfacePropertyDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfacePropertyDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfacePropertyDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "EventHandler MemberName", "{ get; set; }", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticEventFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticEventFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticEventFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static event EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceEventFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceEventFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInterfaceEventFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "event EventHandler MemberName", ", AnotherMemberName;", containingType: "interface", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "System.EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticFieldDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticFieldDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticFieldDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "MemberName", "static System.EventHandler MemberName", ", AnotherMemberName;").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "this", "EventHandler this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", elementName: "this[]").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceIndexerDeclarationAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { EventHandler this[int index] { get; set; } }";
            await this.TestNestedDeclarationAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceIndexerDeclarationWithAttributesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { EventHandler this[int index] { get; set; } }";
            await this.TestNestedDeclarationWithAttributesAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExplicitInterfaceIndexerDeclarationWithDirectivesAsync()
        {
            string baseTypeList = ": IInterface";
            string baseTypeDeclaration = "public interface IInterface { EventHandler this[int index] { get; set; } }";
            await this.TestNestedDeclarationWithDirectivesAsync("private", "this", "EventHandler IInterface.this[int", " index ] { get { throw new System.Exception(); } set { throw new System.Exception(); } }", baseTypeList: baseTypeList, baseTypeDeclarations: baseTypeDeclaration, warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "OuterTypeName", "OuterTypeName(", " ) { }", elementName: ".ctor").ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticConstructorDeclarationAsync()
        {
            await this.TestNestedDeclarationAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticConstructorDeclarationWithAttributesAsync()
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticConstructorDeclarationWithDirectivesAsync()
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "OuterTypeName", "static OuterTypeName(", " ) { }", warning: false).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1400AccessModifierMustBeDeclared();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1400CodeFixProvider();
        }

        private async Task TestTypeDeclarationAsync(string keyword, bool warning = true)
        {
            await this.TestDeclarationAsync("internal", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning).ConfigureAwait(false);
        }

        private async Task TestTypeDeclarationWithAttributesAsync(string keyword, bool warning = true)
        {
            await this.TestDeclarationWithAttributesAsync("internal", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning).ConfigureAwait(false);
        }

        private async Task TestTypeDeclarationWithDirectivesAsync(string keyword, bool warning = true)
        {
            await this.TestDeclarationWithDirectivesAsync("internal", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning).ConfigureAwait(false);
        }

        private async Task TestNestedTypeDeclarationAsync(string keyword, bool warning = true)
        {
            await this.TestNestedDeclarationAsync("private", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning).ConfigureAwait(false);
        }

        private async Task TestNestedTypeDeclarationWithAttributesAsync(string keyword, bool warning = true)
        {
            await this.TestNestedDeclarationWithAttributesAsync("private", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning).ConfigureAwait(false);
        }

        private async Task TestNestedTypeDeclarationWithDirectivesAsync(string keyword, bool warning = true)
        {
            await this.TestNestedDeclarationWithDirectivesAsync("private", "TypeName", $"{keyword} TypeName", "{\n}", warning: warning).ConfigureAwait(false);
        }

        private async Task TestDeclarationAsync(string modifier, string identifier, string keywordLine, string linesAfter, string elementName = null, bool warning = true)
        {
            var testCode = $@"
 {Tab} {keywordLine}
{linesAfter}";

            if (!warning)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(elementName ?? identifier).WithLocation(2, 4 + keywordLine.IndexOf(identifier));

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTestCode = $@"
 {Tab} {modifier} {keywordLine}
{linesAfter}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        private async Task TestDeclarationWithAttributesAsync(string modifier, string identifier, string keywordLine, string linesAfter, string elementName = null, bool warning = true)
        {
            var testCode = $@"using System;
  [Obsolete]
 {Tab} {keywordLine}
{linesAfter}";

            if (!warning)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(elementName ?? identifier).WithLocation(3, 4 + keywordLine.IndexOf(identifier));

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTestCode = $@"using System;
  [Obsolete]
 {Tab} {modifier} {keywordLine}
{linesAfter}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        private async Task TestDeclarationWithDirectivesAsync(string modifier, string identifier, string keywordLine, string linesAfter, string elementName = null, bool warning = true)
        {
            var testCode = $@"
 #  if true
 {Tab} {keywordLine}
# endif
{linesAfter}";

            if (!warning)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(elementName ?? identifier).WithLocation(3, 4 + keywordLine.IndexOf(identifier));

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTestCode = $@"
 #  if true
 {Tab} {modifier} {keywordLine}
# endif
{linesAfter}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        private async Task TestNestedDeclarationAsync(string modifier, string identifier, string keywordLine, string linesAfter, string containingType = "class", string baseTypeList = "", string baseTypeDeclarations = "", string elementName = null, bool warning = true)
        {
            var testCode = $@"using System;
public {containingType} OuterTypeName {baseTypeList} {{
 {Tab} {keywordLine}
{linesAfter} }}
{baseTypeDeclarations}";

            if (!warning)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(elementName ?? identifier).WithLocation(3, 4 + keywordLine.IndexOf(identifier));

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTestCode = $@"using System;
public {containingType} OuterTypeName {baseTypeList} {{
 {Tab} {modifier} {keywordLine}
{linesAfter} }}
{baseTypeDeclarations}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        private async Task TestNestedDeclarationWithAttributesAsync(string modifier, string identifier, string keywordLine, string linesAfter, string containingType = "class", string baseTypeList = "", string baseTypeDeclarations = "", string elementName = null, bool warning = true)
        {
            var testCode = $@"using System;
public {containingType} OuterTypeName {baseTypeList} {{
  [Obsolete]
 {Tab} {keywordLine}
{linesAfter} }}
{baseTypeDeclarations}";

            if (!warning)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(elementName ?? identifier).WithLocation(4, 4 + keywordLine.IndexOf(identifier));

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTestCode = $@"using System;
public {containingType} OuterTypeName {baseTypeList} {{
  [Obsolete]
 {Tab} {modifier} {keywordLine}
{linesAfter} }}
{baseTypeDeclarations}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        private async Task TestNestedDeclarationWithDirectivesAsync(string modifier, string identifier, string keywordLine, string linesAfter, string containingType = "class", string baseTypeList = "", string baseTypeDeclarations = "", string elementName = null, bool warning = true)
        {
            var testCode = $@"using System;
public {containingType} OuterTypeName {baseTypeList} {{
 #  if true
 {Tab} {keywordLine}
# endif
{linesAfter} }}
{baseTypeDeclarations}";

            if (!warning)
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(elementName ?? identifier).WithLocation(4, 4 + keywordLine.IndexOf(identifier));

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTestCode = $@"using System;
public {containingType} OuterTypeName {baseTypeList} {{
 #  if true
 {Tab} {modifier} {keywordLine}
# endif
{linesAfter} }}
{baseTypeDeclarations}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
