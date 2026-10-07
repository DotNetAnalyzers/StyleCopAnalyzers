// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    internal abstract class ImmutableArrayWrapper<T> : IEquatable<ImmutableArrayWrapper<T>>, IReadOnlyList<T>
    {
        public static ImmutableArrayWrapper<T> UnsupportedDefault { get; } =
            new UnsupportedImmutableArray();

        public abstract bool IsEmpty { get; }

        public abstract bool IsDefault { get; }

        public abstract bool IsDefaultOrEmpty { get; }

        public abstract int Length { get; }

        int IReadOnlyCollection<T>.Count => this.Length;

        public abstract T this[int index] { get; }

        public static bool operator ==(ImmutableArrayWrapper<T>? left, ImmutableArrayWrapper<T>? right)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(ImmutableArrayWrapper<T>? left, ImmutableArrayWrapper<T>? right)
        {
            throw new NotImplementedException();
        }

        public bool Equals(ImmutableArrayWrapper<T>? other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        public override abstract int GetHashCode();

        public struct Enumerator : IEnumerator<T>
        {
            private readonly ImmutableArrayWrapper<T> wrapper;
            private int index;
            private T current;

            public Enumerator(ImmutableArrayWrapper<T> wrapper)
            {
                this.wrapper = wrapper;
                this.index = -1;
                this.current = default!;
            }

            public T Current => this.current;

            object IEnumerator.Current => this.Current!;

            public override bool Equals(object obj)
            {
                Enumerator? otherOpt = obj as Enumerator?;
                if (!otherOpt.HasValue)
                {
                    return false;
                }

                Enumerator other = otherOpt.GetValueOrDefault();
                return other.wrapper == this.wrapper
                    && other.index == this.index;
            }

            public override int GetHashCode()
            {
                if (this.wrapper == null)
                {
                    return 0;
                }

                return this.wrapper.GetHashCode() ^ this.index;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (this.index < -1)
                {
                    return false;
                }

                if (this.index == this.wrapper.Length - 1)
                {
                    this.index = int.MinValue;
                    return false;
                }

                this.index++;
                this.current = this.wrapper[this.index];
                return true;
            }

            public void Reset()
            {
                this.index = -1;
                this.current = default!;
            }
        }

        internal sealed class AutoWrapImmutableArray<TElement> : ImmutableArrayWrapper<T>
        {
            private readonly ImmutableArray<TElement> array;

            public AutoWrapImmutableArray()
                : this(default)
            {
            }

            public AutoWrapImmutableArray(ImmutableArray<TElement> array)
            {
                this.array = array;
            }

            public override int Length => this.array.Length;

            public override bool IsEmpty => this.array.IsEmpty;

            public override bool IsDefault => this.array.IsDefault;

            public override bool IsDefaultOrEmpty => this.array.IsDefaultOrEmpty;

            public override T this[int index]
                => (T)WrapperHelper.Wrap(this.array[index]);

            public override int GetHashCode()
                => this.array.GetHashCode();
        }

        private sealed class UnsupportedImmutableArray : ImmutableArrayWrapper<T>
        {
            private static readonly ImmutableArray<T> SyntaxList = ImmutableArray<T>.Empty;

            public UnsupportedImmutableArray()
            {
            }

            public override int Length => SyntaxList.Length;

            public override bool IsEmpty => SyntaxList.IsEmpty;

            public override bool IsDefault => SyntaxList.IsDefault;

            public override bool IsDefaultOrEmpty => SyntaxList.IsDefaultOrEmpty;

            public override T this[int index]
                => SyntaxList[index];

            public override int GetHashCode()
                => SyntaxList.GetHashCode();
        }
    }
}
