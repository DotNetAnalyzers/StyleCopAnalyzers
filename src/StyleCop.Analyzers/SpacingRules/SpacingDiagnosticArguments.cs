// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules;

using Microsoft.CodeAnalysis;

internal static class SpacingDiagnosticArguments
{
    internal static LocalizableString Not { get; } =
        new LocalizableResourceString(nameof(SpacingResources.SpacingNot), SpacingResources.ResourceManager, typeof(SpacingResources));

    internal static LocalizableString Preceded { get; } =
        new LocalizableResourceString(nameof(SpacingResources.SpacingPreceded), SpacingResources.ResourceManager, typeof(SpacingResources));

    internal static LocalizableString Followed { get; } =
        new LocalizableResourceString(nameof(SpacingResources.SpacingFollowed), SpacingResources.ResourceManager, typeof(SpacingResources));

    internal static LocalizableString Increment { get; } =
        new LocalizableResourceString(nameof(SpacingResources.SpacingIncrement), SpacingResources.ResourceManager, typeof(SpacingResources));

    internal static LocalizableString Decrement { get; } =
        new LocalizableResourceString(nameof(SpacingResources.SpacingDecrement), SpacingResources.ResourceManager, typeof(SpacingResources));
}
