// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;

    /// <summary>
    /// An <see cref="IVerifier"/> that reports failures using xUnit assertions.
    /// </summary>
    /// <remarks>
    /// <para>This type is adapted from <c>Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier</c> (MIT license,
    /// .NET Foundation), which shipped in the <c>Microsoft.CodeAnalysis.Testing.Verifiers.XUnit</c> package. That
    /// package was discontinued after version 1.1.2, so it cannot be used with newer versions of
    /// <c>Microsoft.CodeAnalysis.Analyzer.Testing</c>. Keeping a local copy preserves the xUnit failure messages and
    /// exception types that the tests in this repository rely on.</para>
    /// </remarks>
    internal class XUnitVerifier : IVerifier
    {
        public XUnitVerifier()
            : this(ImmutableStack<string>.Empty)
        {
        }

        protected XUnitVerifier(ImmutableStack<string> context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected ImmutableStack<string> Context { get; }

        public virtual void Empty<T>(string collectionName, IEnumerable<T> collection)
        {
            using (var enumerator = collection.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    throw new EmptyWithMessageException(collection, this.CreateMessage($"'{collectionName}' is not empty"));
                }
            }
        }

        public virtual void Equal<T>(T expected, T actual, string message = null)
        {
            if (message is null && this.Context.IsEmpty)
            {
                Assert.Equal(expected, actual);
            }
            else
            {
                if (!EqualityComparer<T>.Default.Equals(expected, actual))
                {
                    throw new EqualWithMessageException(expected, actual, this.CreateMessage(message));
                }
            }
        }

        public virtual void True(bool assert, string message = null)
        {
            if (message is null && this.Context.IsEmpty)
            {
                Assert.True(assert);
            }
            else
            {
                Assert.True(assert, this.CreateMessage(message));
            }
        }

        public virtual void False(bool assert, string message = null)
        {
            if (message is null && this.Context.IsEmpty)
            {
                Assert.False(assert);
            }
            else
            {
                Assert.False(assert, this.CreateMessage(message));
            }
        }

        public virtual void Fail(string message = null)
        {
            if (message is null && this.Context.IsEmpty)
            {
                Assert.True(false);
            }
            else
            {
                Assert.True(false, this.CreateMessage(message));
            }

            throw new InvalidOperationException("This program location is thought to be unreachable.");
        }

        public virtual void LanguageIsSupported(string language)
        {
            Assert.False(language != LanguageNames.CSharp && language != LanguageNames.VisualBasic, this.CreateMessage($"Unsupported Language: '{language}'"));
        }

        public virtual void NotEmpty<T>(string collectionName, IEnumerable<T> collection)
        {
            using (var enumerator = collection.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new NotEmptyWithMessageException(this.CreateMessage($"'{collectionName}' is empty"));
                }
            }
        }

        public virtual void SequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T> equalityComparer = null, string message = null)
        {
            var comparer = new SequenceEqualEnumerableEqualityComparer<T>(equalityComparer);
            var areEqual = comparer.Equals(expected, actual);
            if (!areEqual)
            {
                throw new EqualWithMessageException(expected, actual, this.CreateMessage(message));
            }
        }

        public virtual IVerifier PushContext(string context)
        {
            Assert.IsType<XUnitVerifier>(this);
            return new XUnitVerifier(this.Context.Push(context));
        }

        protected virtual string CreateMessage(string message)
        {
            foreach (var frame in this.Context)
            {
                message = "Context: " + frame + Environment.NewLine + message;
            }

            return message ?? string.Empty;
        }

        private sealed class SequenceEqualEnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
        {
            private readonly IEqualityComparer<T> itemEqualityComparer;

            public SequenceEqualEnumerableEqualityComparer(IEqualityComparer<T> itemEqualityComparer)
            {
                this.itemEqualityComparer = itemEqualityComparer ?? EqualityComparer<T>.Default;
            }

            public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return x.SequenceEqual(y, this.itemEqualityComparer);
            }

            public int GetHashCode(IEnumerable<T> obj)
            {
                if (obj is null)
                {
                    return 0;
                }

                // From System.Tuple
                return obj
                    .Select(item => this.itemEqualityComparer.GetHashCode(item))
                    .Aggregate(
                        0,
                        (aggHash, nextHash) => ((aggHash << 5) + aggHash) ^ nextHash);
            }
        }
    }
}
