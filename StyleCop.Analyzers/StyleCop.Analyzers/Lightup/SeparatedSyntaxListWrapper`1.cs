// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Text;

    internal abstract class SeparatedSyntaxListWrapper<TNode> : IEquatable<SeparatedSyntaxListWrapper<TNode>>, IReadOnlyList<TNode>
    {
        private static readonly SyntaxWrapper<TNode> SyntaxWrapper = SyntaxWrapper<TNode>.Default;

        public static SeparatedSyntaxListWrapper<TNode> UnsupportedEmpty { get; } =
            new UnsupportedSyntaxList();

        public abstract int Count
        {
            get;
        }

        public abstract TextSpan FullSpan
        {
            get;
        }

        public abstract int SeparatorCount
        {
            get;
        }

        public abstract TextSpan Span
        {
            get;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract object UnderlyingList
        {
            get;
        }

        public abstract TNode this[int index]
        {
            get;
        }

        public static bool operator ==(SeparatedSyntaxListWrapper<TNode> left, SeparatedSyntaxListWrapper<TNode> right)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(SeparatedSyntaxListWrapper<TNode> left, SeparatedSyntaxListWrapper<TNode> right)
        {
            throw new NotImplementedException();
        }

        // Summary:
        //     Creates a new list with the specified node added to the end.
        //
        // Parameters:
        //   node:
        //     The node to add.
        public SeparatedSyntaxListWrapper<TNode> Add(TNode node)
            => this.Insert(this.Count, node);

        // Summary:
        //     Creates a new list with the specified nodes added to the end.
        //
        // Parameters:
        //   nodes:
        //     The nodes to add.
        public SeparatedSyntaxListWrapper<TNode> AddRange(IEnumerable<TNode> nodes)
            => this.InsertRange(this.Count, nodes);

        public abstract bool Any();

        public abstract bool Contains(TNode node);

        public bool Equals(SeparatedSyntaxListWrapper<TNode> other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public abstract TNode First();

        public abstract TNode FirstOrDefault();

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TNode>)this).GetEnumerator();
        }

        public override abstract int GetHashCode();

        public abstract SyntaxToken GetSeparator(int index);

        public abstract IEnumerable<SyntaxToken> GetSeparators();

        public abstract SyntaxNodeOrTokenList GetWithSeparators();

        public abstract int IndexOf(Func<TNode, bool> predicate);

        public abstract int IndexOf(TNode node);

        public abstract SeparatedSyntaxListWrapper<TNode> Insert(int index, TNode node);

        public abstract SeparatedSyntaxListWrapper<TNode> InsertRange(int index, IEnumerable<TNode> nodes);

        public abstract TNode Last();

        public abstract int LastIndexOf(Func<TNode, bool> predicate);

        public abstract int LastIndexOf(TNode node);

        public abstract TNode LastOrDefault();

        public abstract SeparatedSyntaxListWrapper<TNode> Remove(TNode node);

        public abstract SeparatedSyntaxListWrapper<TNode> RemoveAt(int index);

        public abstract SeparatedSyntaxListWrapper<TNode> Replace(TNode nodeInList, TNode newNode);

        public abstract SeparatedSyntaxListWrapper<TNode> ReplaceRange(TNode nodeInList, IEnumerable<TNode> newNodes);

        public abstract SeparatedSyntaxListWrapper<TNode> ReplaceSeparator(SyntaxToken separatorToken, SyntaxToken newSeparator);

        public abstract string ToFullString();

        public override abstract string ToString();

        public struct Enumerator : IEnumerator<TNode>
        {
            private readonly SeparatedSyntaxListWrapper<TNode> wrapper;
            private int index;
            private TNode current;

            public Enumerator(SeparatedSyntaxListWrapper<TNode> wrapper)
            {
                this.wrapper = wrapper;
                this.index = -1;
                this.current = default(TNode);
            }

            public TNode Current => this.current;

            object IEnumerator.Current => this.Current;

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

                if (this.index == this.wrapper.Count - 1)
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
                this.current = default(TNode);
            }
        }

        internal sealed class AutoWrapSeparatedSyntaxList<TSyntax> : SeparatedSyntaxListWrapper<TNode>
            where TSyntax : SyntaxNode
        {
            private readonly SeparatedSyntaxList<TSyntax> syntaxList;

            public AutoWrapSeparatedSyntaxList(SeparatedSyntaxList<TSyntax> syntaxList)
            {
                this.syntaxList = syntaxList;
            }

            public override int Count
                => this.syntaxList.Count;

            public override TextSpan FullSpan
                => this.syntaxList.FullSpan;

            public override int SeparatorCount
                => this.syntaxList.SeparatorCount;

            public override TextSpan Span
                => this.syntaxList.Span;

            public override object UnderlyingList
                => this.syntaxList;

            public override TNode this[int index]
                => SyntaxWrapper.Wrap(this.syntaxList[index]);

            public override bool Any()
                => this.syntaxList.Any();

            public override bool Contains(TNode node)
                => this.syntaxList.Contains(SyntaxWrapper.Unwrap(node));

            public override TNode First()
                => SyntaxWrapper.Wrap(this.syntaxList.First());

            public override TNode FirstOrDefault()
                => SyntaxWrapper.Wrap(this.syntaxList.FirstOrDefault());

            public override int GetHashCode()
                => this.syntaxList.GetHashCode();

            public override SyntaxToken GetSeparator(int index)
                => this.syntaxList.GetSeparator(index);

            public override IEnumerable<SyntaxToken> GetSeparators()
                => this.syntaxList.GetSeparators();

            public override SyntaxNodeOrTokenList GetWithSeparators()
                => this.syntaxList.GetWithSeparators();

            public override int IndexOf(TNode node)
                => this.syntaxList.IndexOf((TSyntax)SyntaxWrapper.Unwrap(node));

            public override int IndexOf(Func<TNode, bool> predicate)
                => this.syntaxList.IndexOf(node => predicate(SyntaxWrapper.Wrap(node)));

            public override SeparatedSyntaxListWrapper<TNode> Insert(int index, TNode node)
                => new AutoWrapSeparatedSyntaxList<TSyntax>(this.syntaxList.Insert(index, (TSyntax)SyntaxWrapper.Unwrap(node)));

            public override SeparatedSyntaxListWrapper<TNode> InsertRange(int index, IEnumerable<TNode> nodes)
                => new AutoWrapSeparatedSyntaxList<TSyntax>(this.syntaxList.InsertRange(index, nodes.Select(node => (TSyntax)SyntaxWrapper.Unwrap(node))));

            public override TNode Last()
                => SyntaxWrapper.Wrap(this.syntaxList.Last());

            public override int LastIndexOf(TNode node)
                => this.syntaxList.LastIndexOf((TSyntax)SyntaxWrapper.Unwrap(node));

            public override int LastIndexOf(Func<TNode, bool> predicate)
                => this.syntaxList.LastIndexOf(node => predicate(SyntaxWrapper.Wrap(node)));

            public override TNode LastOrDefault()
                => SyntaxWrapper.Wrap(this.syntaxList.LastOrDefault());

            public override SeparatedSyntaxListWrapper<TNode> Remove(TNode node)
                => new AutoWrapSeparatedSyntaxList<TSyntax>(this.syntaxList.Remove((TSyntax)SyntaxWrapper.Unwrap(node)));

            public override SeparatedSyntaxListWrapper<TNode> RemoveAt(int index)
                => new AutoWrapSeparatedSyntaxList<TSyntax>(this.syntaxList.RemoveAt(index));

            public override SeparatedSyntaxListWrapper<TNode> Replace(TNode nodeInList, TNode newNode)
                => new AutoWrapSeparatedSyntaxList<TSyntax>(this.syntaxList.Replace((TSyntax)SyntaxWrapper.Unwrap(nodeInList), (TSyntax)SyntaxWrapper.Unwrap(newNode)));

            public override SeparatedSyntaxListWrapper<TNode> ReplaceRange(TNode nodeInList, IEnumerable<TNode> newNodes)
                => new AutoWrapSeparatedSyntaxList<TSyntax>(this.syntaxList.ReplaceRange((TSyntax)SyntaxWrapper.Unwrap(nodeInList), newNodes.Select(node => (TSyntax)SyntaxWrapper.Unwrap(node))));

            public override SeparatedSyntaxListWrapper<TNode> ReplaceSeparator(SyntaxToken separatorToken, SyntaxToken newSeparator)
                => new AutoWrapSeparatedSyntaxList<TSyntax>(this.syntaxList.ReplaceSeparator(separatorToken, newSeparator));

            public override string ToFullString()
                => this.syntaxList.ToFullString();

            public override string ToString()
                => this.syntaxList.ToString();
        }

        /// <summary>
        /// Wrapper class for elements that have no type definition at compile time.
        /// </summary>
        internal sealed class UnknownElementTypeSyntaxList : SeparatedSyntaxListWrapper<TNode>
        {
            private static readonly Type WrappedType = WrapperHelper.GetWrappedType(typeof(TNode));
            private static readonly MethodInfo EnumerableCastMethodInfo = typeof(Enumerable).GetTypeInfo().GetDeclaredMethod(nameof(Enumerable.Cast)).MakeGenericMethod(WrappedType);

            private readonly object underlyingList;
            private readonly System.Reflection.TypeInfo underlyingListTypeInfo;

            private Lazy<PropertyInfo> countPropertyInfo;
            private Lazy<PropertyInfo> fullSpanPropertyInfo;
            private Lazy<PropertyInfo> separatorCountPropertyInfo;
            private Lazy<PropertyInfo> spanPropertyInfo;
            private Lazy<PropertyInfo> indexerPropertyInfo;
            private Lazy<MethodInfo> anyMethodInfo;
            private Lazy<MethodInfo> containsMethodInfo;
            private Lazy<MethodInfo> firstMethodInfo;
            private Lazy<MethodInfo> firstOrDefaultMethodInfo;
            private Lazy<MethodInfo> getSeparatorMethodInfo;
            private Lazy<MethodInfo> getSeparatorsMethodInfo;
            private Lazy<MethodInfo> getWithSeparatorsMethodInfo;
            private Lazy<MethodInfo> indexOfPredicateMethodInfo;
            private Lazy<MethodInfo> indexOfItemMethodInfo;
            private Lazy<MethodInfo> insertMethodInfo;
            private Lazy<MethodInfo> insertRangeMethodInfo;
            private Lazy<MethodInfo> lastMethodInfo;
            private Lazy<MethodInfo> lastIndexOfPredicateMethodInfo;
            private Lazy<MethodInfo> lastIndexOfItemMethodInfo;
            private Lazy<MethodInfo> lastOrDefaultMethodInfo;
            private Lazy<MethodInfo> removeMethodInfo;
            private Lazy<MethodInfo> removeAtMethodInfo;
            private Lazy<MethodInfo> replaceMethodInfo;
            private Lazy<MethodInfo> replaceRangeMethodInfo;
            private Lazy<MethodInfo> replaceSeparatorMethodInfo;
            private Lazy<MethodInfo> toFullStringMethodInfo;

            public UnknownElementTypeSyntaxList(IEnumerable<TNode> elements)
            {
                this.underlyingList = this.CreateUnderlyingList(elements);
                this.underlyingListTypeInfo = this.underlyingList.GetType().GetTypeInfo();

                this.SetupMethodInfos();
            }

            public UnknownElementTypeSyntaxList(object underlyingList)
            {
                this.underlyingList = underlyingList;
                this.underlyingListTypeInfo = this.underlyingList.GetType().GetTypeInfo();

                this.SetupMethodInfos();
            }

            public override int Count
                => this.GetPropertyValue<int>(this.countPropertyInfo);

            public override TextSpan FullSpan
                => this.GetPropertyValue<TextSpan>(this.fullSpanPropertyInfo);

            public override int SeparatorCount
                => this.GetPropertyValue<int>(this.separatorCountPropertyInfo);

            public override TextSpan Span
                => this.GetPropertyValue<TextSpan>(this.spanPropertyInfo);

            public override object UnderlyingList
                => this.underlyingList;

            public override TNode this[int index]
                => SyntaxWrapper.Wrap((SyntaxNode)this.indexerPropertyInfo.Value.GetValue(this.underlyingList, new object[] { index }));

            public override bool Any()
                => this.InvokeFunction<bool>(this.anyMethodInfo);

            public override bool Contains(TNode node)
                => this.InvokeFunction<bool>(this.containsMethodInfo, SyntaxWrapper.Unwrap(node));

            public override TNode First()
                => SyntaxWrapper.Wrap(this.InvokeFunction<SyntaxNode>(this.firstMethodInfo));

            public override TNode FirstOrDefault()
                => SyntaxWrapper.Wrap(this.InvokeFunction<SyntaxNode>(this.firstOrDefaultMethodInfo));

            public override int GetHashCode()
                => this.underlyingList.GetHashCode();

            public override SyntaxToken GetSeparator(int index)
                => this.InvokeFunction<SyntaxToken>(this.getSeparatorMethodInfo, index);

            public override IEnumerable<SyntaxToken> GetSeparators()
                => this.InvokeFunction<IEnumerable<SyntaxToken>>(this.getSeparatorsMethodInfo);

            public override SyntaxNodeOrTokenList GetWithSeparators()
                => this.InvokeFunction<SyntaxNodeOrTokenList>(this.getWithSeparatorsMethodInfo);

            public override int IndexOf(Func<TNode, bool> predicate)
            {
                //// TODO: create a new Func<TNode, bool> instance based on reflected information.
                //// => this.InvokeFunction<int>(this.indexOfPredicateMethodInfo, node => predicate(SyntaxWrapper.Wrap(node)));
                throw new NotImplementedException();
            }

            public override int IndexOf(TNode node)
                => this.InvokeFunction<int>(this.indexOfItemMethodInfo, SyntaxWrapper.Unwrap(node));

            public override SeparatedSyntaxListWrapper<TNode> Insert(int index, TNode node)
            {
                var newUnderlyingList = this.InvokeFunction<object>(this.insertMethodInfo, index, SyntaxWrapper.Unwrap(node));
                return new UnknownElementTypeSyntaxList(newUnderlyingList);
            }

            public override SeparatedSyntaxListWrapper<TNode> InsertRange(int index, IEnumerable<TNode> nodes)
            {
                var newUnderlyingList = this.InvokeFunction<object>(this.insertMethodInfo, index, CreateUnwrappedEnumerable(nodes));
                return new UnknownElementTypeSyntaxList(newUnderlyingList);
            }

            public override TNode Last()
                => SyntaxWrapper.Wrap(this.InvokeFunction<SyntaxNode>(this.lastMethodInfo));

            public override int LastIndexOf(Func<TNode, bool> predicate)
            {
                //// TODO: See implementation of IndexOf with a predicate
                throw new NotImplementedException();
            }

            public override int LastIndexOf(TNode node)
                => this.InvokeFunction<int>(this.lastIndexOfItemMethodInfo, SyntaxWrapper.Unwrap(node));

            public override TNode LastOrDefault()
                => SyntaxWrapper.Wrap(this.InvokeFunction<SyntaxNode>(this.lastOrDefaultMethodInfo));

            public override SeparatedSyntaxListWrapper<TNode> Remove(TNode node)
            {
                var newUnderlyingList = this.InvokeFunction<object>(this.removeMethodInfo, SyntaxWrapper.Unwrap(node));
                return new UnknownElementTypeSyntaxList(newUnderlyingList);
            }

            public override SeparatedSyntaxListWrapper<TNode> RemoveAt(int index)
            {
                var newUnderlyingList = this.InvokeFunction<object>(this.removeAtMethodInfo, index);
                return new UnknownElementTypeSyntaxList(newUnderlyingList);
            }

            public override SeparatedSyntaxListWrapper<TNode> Replace(TNode nodeInList, TNode newNode)
            {
                var newUnderlyingList = this.InvokeFunction<object>(this.replaceMethodInfo, SyntaxWrapper.Unwrap(nodeInList), SyntaxWrapper.Unwrap(newNode));
                return new UnknownElementTypeSyntaxList(newUnderlyingList);
            }

            public override SeparatedSyntaxListWrapper<TNode> ReplaceRange(TNode nodeInList, IEnumerable<TNode> newNodes)
            {
                var newUnderlyingList = this.InvokeFunction<object>(this.replaceRangeMethodInfo, SyntaxWrapper.Unwrap(nodeInList), CreateUnwrappedEnumerable(newNodes));
                return new UnknownElementTypeSyntaxList(newUnderlyingList);
            }

            public override SeparatedSyntaxListWrapper<TNode> ReplaceSeparator(SyntaxToken separatorToken, SyntaxToken newSeparator)
            {
                var newUnderlyingList = this.InvokeFunction<object>(this.replaceSeparatorMethodInfo, separatorToken, newSeparator);
                return new UnknownElementTypeSyntaxList(newUnderlyingList);
            }

            public override string ToFullString()
                => this.InvokeFunction<string>(this.toFullStringMethodInfo);

            public override string ToString()
                => this.underlyingList.ToString();

            private static object CreateUnwrappedEnumerable(IEnumerable<TNode> elements)
            {
                return EnumerableCastMethodInfo.Invoke(null, new object[] { elements.Select(e => SyntaxWrapper.Unwrap(e)) });
            }

            private object CreateUnderlyingList(IEnumerable<TNode> elements)
            {
                var syntaxFactoryTypeInfo = typeof(SyntaxFactory).GetTypeInfo();
                var method = syntaxFactoryTypeInfo.GetDeclaredMethods(nameof(SyntaxFactory.SeparatedList))
                    .Single(m =>
                    {
                        if (!m.IsStatic)
                        {
                            return false;
                        }

                        var parameters = m.GetParameters();
                        if (parameters.Length != 1)
                        {
                            return false;
                        }

                        var parameterType = parameters[0].ParameterType;
                        if (!parameterType.IsConstructedGenericType || (parameterType.GetGenericTypeDefinition() != typeof(IEnumerable<>)))
                        {
                            return false;
                        }

                        return parameterType.GenericTypeArguments[0].Name == "TNode";
                    })
                    .MakeGenericMethod(WrappedType);

                return method.Invoke(null, new object[] { CreateUnwrappedEnumerable(elements) });
            }

            private void SetupMethodInfos()
            {
                this.countPropertyInfo = new Lazy<PropertyInfo>(() => this.underlyingListTypeInfo.GetDeclaredProperty(nameof(SeparatedSyntaxList<SyntaxNode>.Count)));
                this.fullSpanPropertyInfo = new Lazy<PropertyInfo>(() => this.underlyingListTypeInfo.GetDeclaredProperty(nameof(SeparatedSyntaxList<SyntaxNode>.FullSpan)));
                this.separatorCountPropertyInfo = new Lazy<PropertyInfo>(() => this.underlyingListTypeInfo.GetDeclaredProperty(nameof(SeparatedSyntaxList<SyntaxNode>.SeparatorCount)));
                this.spanPropertyInfo = new Lazy<PropertyInfo>(() => this.underlyingListTypeInfo.GetDeclaredProperty(nameof(SeparatedSyntaxList<SyntaxNode>.Span)));
                this.indexerPropertyInfo = new Lazy<PropertyInfo>(() => this.underlyingListTypeInfo.GetDeclaredProperty("Item"));

                this.anyMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.Any)));
                this.containsMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.Contains)));
                this.firstMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.First)));
                this.firstOrDefaultMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.FirstOrDefault)));
                this.getSeparatorMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.GetSeparator)));
                this.getSeparatorsMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.GetSeparators)));
                this.getWithSeparatorsMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.GetWithSeparators)));

                this.indexOfPredicateMethodInfo = new Lazy<MethodInfo>(() =>
                {
                    return this.underlyingListTypeInfo.GetDeclaredMethods(nameof(SeparatedSyntaxList<SyntaxNode>.IndexOf))
                        .Single(m => m.GetParameters()[0].ParameterType.Name == "Func`2");
                });
                this.indexOfItemMethodInfo = new Lazy<MethodInfo>(() =>
                {
                    return this.underlyingListTypeInfo.GetDeclaredMethods(nameof(SeparatedSyntaxList<SyntaxNode>.IndexOf))
                        .Single(m => m.GetParameters()[0].ParameterType == WrappedType);
                });
                this.insertMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.Insert)));
                this.insertRangeMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.InsertRange)));
                this.lastMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.Last)));
                this.lastIndexOfPredicateMethodInfo = new Lazy<MethodInfo>(() =>
                {
                    return this.underlyingListTypeInfo.GetDeclaredMethods(nameof(SeparatedSyntaxList<SyntaxNode>.LastIndexOf))
                        .Single(m => m.GetParameters()[0].ParameterType.Name == "Func`2");
                });
                this.lastIndexOfItemMethodInfo = new Lazy<MethodInfo>(() =>
                {
                    return this.underlyingListTypeInfo.GetDeclaredMethods(nameof(SeparatedSyntaxList<SyntaxNode>.LastIndexOf))
                        .Single(m => m.GetParameters()[0].ParameterType == WrappedType);
                });
                this.lastOrDefaultMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.LastOrDefault)));
                this.removeMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.Remove)));
                this.removeAtMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.RemoveAt)));
                this.replaceMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.Replace)));
                this.replaceRangeMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.ReplaceRange)));
                this.replaceSeparatorMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.ReplaceSeparator)));
                this.toFullStringMethodInfo = new Lazy<MethodInfo>(() => this.underlyingListTypeInfo.GetDeclaredMethod(nameof(SeparatedSyntaxList<SyntaxNode>.ToFullString)));
            }

            private T GetPropertyValue<T>(Lazy<PropertyInfo> propertyInfo)
            {
                return (T)propertyInfo.Value.GetValue(this.underlyingList);
            }

            private T InvokeFunction<T>(Lazy<MethodInfo> methodInfo, params object[] parameters)
            {
                return (T)methodInfo.Value.Invoke(this.underlyingList, parameters);
            }
        }

        private sealed class UnsupportedSyntaxList : SeparatedSyntaxListWrapper<TNode>
        {
            private static readonly SeparatedSyntaxList<SyntaxNode> SyntaxList = default(SeparatedSyntaxList<SyntaxNode>);

            public UnsupportedSyntaxList()
            {
            }

            public override int Count
                => 0;

            public override TextSpan FullSpan
                => SyntaxList.FullSpan;

            public override int SeparatorCount
                => 0;

            public override TextSpan Span
                => SyntaxList.Span;

            public override object UnderlyingList
                => null;

            public override TNode this[int index]
                => SyntaxWrapper.Wrap(SyntaxList[index]);

            public override bool Any()
                => false;

            public override bool Contains(TNode node)
                => false;

            public override TNode First()
                => SyntaxWrapper.Wrap(SyntaxList.First());

            public override TNode FirstOrDefault()
                => SyntaxWrapper.Wrap(default(SyntaxNode));

            public override int GetHashCode()
                => SyntaxList.GetHashCode();

            public override SyntaxToken GetSeparator(int index)
                => SyntaxList.GetSeparator(index);

            public override IEnumerable<SyntaxToken> GetSeparators()
                => SyntaxList.GetSeparators();

            public override SyntaxNodeOrTokenList GetWithSeparators()
                => SyntaxList.GetWithSeparators();

            public override int IndexOf(TNode node)
                => SyntaxList.IndexOf(SyntaxWrapper.Unwrap(node));

            public override int IndexOf(Func<TNode, bool> predicate)
                => SyntaxList.IndexOf(node => predicate(SyntaxWrapper.Wrap(node)));

            public override SeparatedSyntaxListWrapper<TNode> Insert(int index, TNode node)
            {
                throw new NotSupportedException();
            }

            public override SeparatedSyntaxListWrapper<TNode> InsertRange(int index, IEnumerable<TNode> nodes)
            {
                throw new NotSupportedException();
            }

            public override TNode Last()
                => SyntaxWrapper.Wrap(SyntaxList.Last());

            public override int LastIndexOf(TNode node)
                => SyntaxList.LastIndexOf(SyntaxWrapper.Unwrap(node));

            public override int LastIndexOf(Func<TNode, bool> predicate)
                => SyntaxList.LastIndexOf(node => predicate(SyntaxWrapper.Wrap(node)));

            public override TNode LastOrDefault()
                => SyntaxWrapper.Wrap(default(SyntaxNode));

            public override SeparatedSyntaxListWrapper<TNode> Remove(TNode node)
            {
                throw new NotSupportedException();
            }

            public override SeparatedSyntaxListWrapper<TNode> RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            public override SeparatedSyntaxListWrapper<TNode> Replace(TNode nodeInList, TNode newNode)
            {
                throw new NotSupportedException();
            }

            public override SeparatedSyntaxListWrapper<TNode> ReplaceRange(TNode nodeInList, IEnumerable<TNode> newNodes)
            {
                throw new NotSupportedException();
            }

            public override SeparatedSyntaxListWrapper<TNode> ReplaceSeparator(SyntaxToken separatorToken, SyntaxToken newSeparator)
            {
                throw new NotSupportedException();
            }

            public override string ToFullString()
                => SyntaxList.ToFullString();

            public override string ToString()
                => SyntaxList.ToString();
        }
    }
}
