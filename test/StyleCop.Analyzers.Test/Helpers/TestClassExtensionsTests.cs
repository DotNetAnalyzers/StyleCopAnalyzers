// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers;

using System;
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// This class verifies <see cref="TestClassExtensions"/>.
/// </summary>
public class TestClassExtensionsTests
{
    [Fact]
    public void TestDeclaredMethodIsNotInherited()
    {
        Assert.False(new BaseClass().Method());
    }

    [Fact]
    public void TestInheritedMethodIsInherited()
    {
        Assert.True(new DerivedClass().Method());
    }

    [Fact]
    public async Task TestDeclaredAsyncMethodIsNotInheritedAsync()
    {
        Assert.False(await new BaseClass().MethodAsync().ConfigureAwait(false));
    }

    [Fact]
    public async Task TestInheritedAsyncMethodIsInheritedAsync()
    {
        Assert.True(await new DerivedClass().MethodAsync().ConfigureAwait(false));
    }

    [Fact]
    public void TestDeclaredVirtualMethodIsNotInherited()
    {
        Assert.False(new BaseClass().VirtualMethod());
    }

    [Fact]
    public void TestOverriddenMethodIsNotInherited()
    {
        Assert.False(new DerivedClass().VirtualMethod());
    }

    [Fact]
    public void TestOverloadedMethodThrows()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => new BaseClass().OverloadedMethod());
        Assert.Contains("found 2", ex.Message);
    }

    [Fact]
    public void TestOverloadedMethodWithParameterThrows()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => new BaseClass().OverloadedMethod(0));
        Assert.Contains("found 2", ex.Message);
    }

    [Fact]
    public void TestNonPublicMethodThrows()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => new BaseClass().CallFromPrivateMethod());
        Assert.Contains("found 0", ex.Message);
    }

    public class BaseClass
    {
        public bool Method() => this.IsInheritedTest();

        public async Task<bool> MethodAsync()
        {
            await Task.Yield();
            return this.IsInheritedTest();
        }

        public virtual bool VirtualMethod() => this.IsInheritedTest();

        public bool OverloadedMethod() => this.IsInheritedTest();

        public bool OverloadedMethod(int value) => this.IsInheritedTest();

        public bool CallFromPrivateMethod() => this.PrivateMethod();

        private bool PrivateMethod() => this.IsInheritedTest();
    }

    public class DerivedClass : BaseClass
    {
        public override bool VirtualMethod() => this.IsInheritedTest();
    }
}
