// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System.Diagnostics.CodeAnalysis;

    internal delegate bool TryGetValueAccessor<T, TKey, TValue>(T instance, TKey key, [MaybeNullWhen(false)] out TValue value);
}
