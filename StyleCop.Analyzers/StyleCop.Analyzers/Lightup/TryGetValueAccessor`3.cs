// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    internal delegate bool TryGetValueAccessor<T, TKey, TValue>(T instance, TKey key, out TValue value);
}
