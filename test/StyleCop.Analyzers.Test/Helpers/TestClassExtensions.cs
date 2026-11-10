// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers;

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

internal static class TestClassExtensions
{
    /// <summary>
    /// Determines whether the calling test method was inherited from a test class in an earlier test project, rather
    /// than declared in the class of <paramref name="testClass"/>.
    /// </summary>
    /// <remarks>
    /// <para>Tests with invalid code use this to verify the compiler diagnostics only in the test project that
    /// declares the test, because the compiler diagnostics often differ between compiler versions.</para>
    /// </remarks>
    /// <param name="testClass">The test class instance, normally <see langword="this"/>.</param>
    /// <param name="testMethodName">The name of the calling test method, supplied by the compiler.</param>
    /// <returns><see langword="true"/> if the test is inherited; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">If the class does not have exactly one public instance method
    /// with the name of the caller.</exception>
    internal static bool IsInheritedTest(this object testClass, [CallerMemberName] string testMethodName = "")
    {
        var type = testClass.GetType();
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => method.Name == testMethodName)
            .ToArray();
        if (methods.Length != 1)
        {
            throw new InvalidOperationException($"Expected exactly one public test method '{testMethodName}' in {type}, found {methods.Length}.");
        }

        return methods[0].DeclaringType != type;
    }
}
