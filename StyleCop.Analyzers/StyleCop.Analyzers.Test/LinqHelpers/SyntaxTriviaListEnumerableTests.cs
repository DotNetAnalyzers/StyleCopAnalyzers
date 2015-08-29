﻿namespace StyleCop.Analyzers.Test.LinqHelpers
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Xunit;

    /// <summary>
    /// This class provides supplementary tests to improve code coverage in <see cref="SyntaxTriviaListEnumerable"/>.
    /// </summary>
    public class SyntaxTriviaListEnumerableTests
    {
        [Fact]
        public void TestAnyWithNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => SyntaxTriviaListEnumerable.Any(SyntaxFactory.TriviaList(), null));
        }

        [Fact]
        public void TestAllWithNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => SyntaxTriviaListEnumerable.All(SyntaxFactory.TriviaList(), null));
        }
    }
}
