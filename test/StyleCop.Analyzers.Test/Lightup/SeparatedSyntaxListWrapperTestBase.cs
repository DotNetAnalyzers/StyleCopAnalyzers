// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public abstract class SeparatedSyntaxListWrapperTestBase
    {
        [Fact]
        public void TestBasicProperties()
        {
            var list = this.CreateList();
            Assert.Empty(list);
            Assert.Equal(0, list.SeparatorCount);
            Assert.Equal(default(SeparatedSyntaxList<SyntaxNode>).FullSpan, list.FullSpan);
            Assert.Equal(default(SeparatedSyntaxList<SyntaxNode>).Span, list.Span);
            Assert.Equal(default(SeparatedSyntaxList<SyntaxNode>).ToString(), list.ToString());
            Assert.Equal(default(SeparatedSyntaxList<SyntaxNode>).ToFullString(), list.ToFullString());
            Assert.ThrowsAny<ArgumentException>(() => list[0]);

            if (list.UnderlyingList != null)
            {
                Assert.IsAssignableFrom<SeparatedSyntaxList<SyntaxNode>>(list.UnderlyingList);
                var underlyingList = (SeparatedSyntaxList<SyntaxNode>)list.UnderlyingList;
                Assert.Empty(list);
            }
        }

        [Fact]
        public void TestElements()
        {
            var list = this.CreateList();
            Assert.False(list.Any());
            Assert.Null(list.FirstOrDefault());
            Assert.Null(list.LastOrDefault());
            Assert.ThrowsAny<ArgumentOutOfRangeException>(() => list.First());
            Assert.ThrowsAny<ArgumentOutOfRangeException>(() => list.Last());

            if (this.TryCreateNonEmptyList(out list))
            {
                Assert.True(list.Any());
                Assert.NotNull(list.First());
                Assert.NotNull(list.FirstOrDefault());
                Assert.NotNull(list.Last());
                Assert.NotNull(list.LastOrDefault());
            }
        }

        [Fact]
        [WorkItem(2840, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2840")]
        public void TestEquality()
        {
            var list = this.CreateList();
            var other = this.CreateList();
            SeparatedSyntaxListWrapper<SyntaxNode> nullList = null;

            Assert.True(list.Equals(other));
            Assert.True(list.Equals((object)other));
            Assert.True(list == other);
            Assert.False(list != other);
            Assert.Equal(list.GetHashCode(), other.GetHashCode());

            Assert.False(list.Equals(nullList));
            Assert.False(list.Equals((object)null));
            Assert.False(list.Equals(new object()));
            Assert.False(list == nullList);
            Assert.False(nullList == list);
            Assert.True(list != nullList);
            Assert.True(nullList != list);
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.True(nullList == nullList);
#pragma warning restore CS1718 // Comparison made to same variable

            if (this.TryCreateNonEmptyList(out var nonEmptyList))
            {
                var sameNonEmptyList = nonEmptyList;
                Assert.False(list.Equals(nonEmptyList));
                Assert.False(list == nonEmptyList);
                Assert.True(list != nonEmptyList);
                Assert.True(nonEmptyList.Equals(sameNonEmptyList));
                Assert.True(nonEmptyList == sameNonEmptyList);
            }
        }

        [Fact]
        [WorkItem(2840, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2840")]
        public void TestEnumeratorEquality()
        {
            var list = this.CreateList();
            var enumerator1 = list.GetEnumerator();
            var enumerator2 = list.GetEnumerator();

            Assert.True(enumerator1.Equals(enumerator2));
            Assert.Equal(enumerator1.GetHashCode(), enumerator2.GetHashCode());
            Assert.False(enumerator1.Equals(new object()));
            Assert.False(enumerator1.Equals(default(SeparatedSyntaxListWrapper<SyntaxNode>.Enumerator)));
            Assert.Equal(0, default(SeparatedSyntaxListWrapper<SyntaxNode>.Enumerator).GetHashCode());
            Assert.True(default(SeparatedSyntaxListWrapper<SyntaxNode>.Enumerator).Equals(default(SeparatedSyntaxListWrapper<SyntaxNode>.Enumerator)));
        }

        internal abstract SeparatedSyntaxListWrapper<SyntaxNode> CreateList();

        internal abstract bool TryCreateNonEmptyList(out SeparatedSyntaxListWrapper<SyntaxNode> list);
    }
}
