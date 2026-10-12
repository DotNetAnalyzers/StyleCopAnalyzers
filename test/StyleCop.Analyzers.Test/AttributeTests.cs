// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test
{
    using Xunit;

    public class AttributeTests
    {
        [Fact]
        public void TestNoCodeFixAttributeReason()
        {
            string reason = "Reason";
            var attribute = new NoCodeFixAttribute(reason);
            Assert.Same(reason, attribute.Reason);
        }

        [Fact]
        public void TestNoDiagnosticAttributeReason()
        {
            string reason = "Reason";
            var attribute = new NoDiagnosticAttribute(reason);
            Assert.Same(reason, attribute.Reason);
        }

        [Fact]
        public void TestWorkItemAttribute()
        {
            int id = 1234;
            string issueUri = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2419";
            var attribute = new WorkItemAttribute(id, issueUri);
            Assert.Equal(id, attribute.Id);
            Assert.Same(issueUri, attribute.Location);
        }
    }
}
