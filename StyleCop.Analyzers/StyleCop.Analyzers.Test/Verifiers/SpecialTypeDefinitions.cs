// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    internal static class SpecialTypeDefinitions
    {
        public const string IndexAndRange = @"namespace System
{
    public struct Range
    {
        public Range(Index a, Index b)
        {
        }
        public Index Start { get; }
        public Index End { get; }
    }

    public struct Index
    {
        public static implicit operator Index(int value) => throw null;
        public int GetOffset(int length) => throw null;
    }
}
";
    }
}
