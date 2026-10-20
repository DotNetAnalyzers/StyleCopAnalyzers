// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System;
    using System.Collections.Immutable;
    using LightJson;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

    internal class OrderingSettings
    {
        private static readonly ImmutableArray<OrderingTrait> DefaultElementOrder =
            ImmutableArray.Create(
                OrderingTrait.Kind,
                OrderingTrait.Accessibility,
                OrderingTrait.Constant,
                OrderingTrait.Static,
                OrderingTrait.Readonly);

        private static readonly ImmutableArray<AccessLevel> DefaultAccessibilityOrder =
            ImmutableArray.Create(
                AccessLevel.Public,
                AccessLevel.Internal,
                AccessLevel.ProtectedInternal,
                AccessLevel.Protected,
                AccessLevel.PrivateProtected,
                AccessLevel.Private);

        /// <summary>
        /// This is the backing field for the <see cref="ElementOrder"/> property.
        /// </summary>
        private readonly ImmutableArray<OrderingTrait> elementOrder;

        private readonly ImmutableArray<AccessLevel> accessibilityOrder;

        /// <summary>
        /// This is the backing field for the <see cref="SystemUsingDirectivesFirst"/> property.
        /// </summary>
        private readonly bool systemUsingDirectivesFirst;

        /// <summary>
        /// This is the backing field for the <see cref="UsingDirectivesPlacement"/> property.
        /// </summary>
        private readonly UsingDirectivesPlacement usingDirectivesPlacement;

        /// <summary>
        /// This is the backing field for the <see cref="BlankLinesBetweenUsingGroups"/> property.
        /// </summary>
        private readonly OptionSetting blankLinesBetweenUsingGroups;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderingSettings"/> class.
        /// </summary>
        protected internal OrderingSettings()
        {
            this.elementOrder = ImmutableArray<OrderingTrait>.Empty;
            this.accessibilityOrder = DefaultAccessibilityOrder;
            this.systemUsingDirectivesFirst = true;
            this.usingDirectivesPlacement = UsingDirectivesPlacement.InsideNamespace;
            this.blankLinesBetweenUsingGroups = OptionSetting.Allow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderingSettings"/> class.
        /// </summary>
        /// <param name="orderingSettingsObject">The JSON object containing the settings.</param>
        /// <param name="analyzerConfigOptions">The <strong>.editorconfig</strong> options to use if
        /// <strong>stylecop.json</strong> does not provide values.</param>
        protected internal OrderingSettings(JsonObject orderingSettingsObject, AnalyzerConfigOptionsWrapper analyzerConfigOptions)
        {
            var reader = new SettingsReader(orderingSettingsObject, analyzerConfigOptions);

            var usingDirectivesPlacement = reader.GetEnum<UsingDirectivesPlacement>(
                "usingDirectivesPlacement",
                "stylecop.ordering.usingDirectivesPlacement",
                "csharp_using_directive_placement",
                value => value switch
                {
                    "inside_namespace" => UsingDirectivesPlacement.InsideNamespace,
                    "outside_namespace" => UsingDirectivesPlacement.OutsideNamespace,
                    _ => null,
                });
            var blankLinesBetweenUsingGroups = reader.GetEnum<OptionSetting>(
                "blankLinesBetweenUsingGroups",
                "stylecop.ordering.blankLinesBetweenUsingGroups",
                "dotnet_separate_import_directive_groups",
                value => bool.TryParse(value, out var separate) ? (separate ? OptionSetting.Require : OptionSetting.Allow) : null);

            this.elementOrder = reader.GetEnumList<OrderingTrait>("elementOrder", "stylecop.ordering.elementOrder") ?? ImmutableArray<OrderingTrait>.Empty;
            this.accessibilityOrder = reader.GetStringList("accessibilityOrder", "stylecop.ordering.accessibilityOrder") is { } accessibilityNames
                ? ValidateAccessibilityOrder(ParseAccessibilityOrder(accessibilityNames))
                : DefaultAccessibilityOrder;
            if (reader.GetStringList("memberKindOrder", "stylecop.ordering.memberKindOrder", emptyValueIsEmptyList: true) is { } memberKindOrder)
            {
                this.MemberKindOrder = new MemberKindOrder(memberKindOrder, this.ElementOrder, this.AccessibilityOrder);
            }

            this.systemUsingDirectivesFirst = reader.GetBoolean("systemUsingDirectivesFirst", "dotnet_sort_system_directives_first").GetValueOrDefault(true);
            this.usingDirectivesPlacement = usingDirectivesPlacement.GetValueOrDefault(UsingDirectivesPlacement.InsideNamespace);
            this.blankLinesBetweenUsingGroups = blankLinesBetweenUsingGroups.GetValueOrDefault(OptionSetting.Allow);
        }

        public ImmutableArray<OrderingTrait> ElementOrder
        {
            get
            {
                return this.elementOrder.Length > 0 ? this.elementOrder : DefaultElementOrder;
            }
        }

        public ImmutableArray<AccessLevel> AccessibilityOrder =>
            this.accessibilityOrder;

        public bool SystemUsingDirectivesFirst =>
            this.systemUsingDirectivesFirst;

        public UsingDirectivesPlacement UsingDirectivesPlacement =>
            this.usingDirectivesPlacement;

        public OptionSetting BlankLinesBetweenUsingGroups =>
            this.blankLinesBetweenUsingGroups;

        internal MemberKindOrder? MemberKindOrder { get; }

        private static ImmutableArray<AccessLevel> ParseAccessibilityOrder(ImmutableArray<string> accessibilityNames)
        {
            var builder = ImmutableArray.CreateBuilder<AccessLevel>(accessibilityNames.Length);
            foreach (var accessibilityName in accessibilityNames)
            {
                if (accessibilityName.Length == 0
                    || !char.IsLetter(accessibilityName[0])
                    || !Enum.TryParse(accessibilityName, ignoreCase: true, out AccessLevel accessLevel)
                    || !Enum.IsDefined(typeof(AccessLevel), accessLevel))
                {
                    throw new InvalidSettingsException($"accessibilityOrder cannot contain value '{accessibilityName}'.");
                }

                builder.Add(accessLevel);
            }

            return builder.ToImmutable();
        }

        private static ImmutableArray<AccessLevel> ValidateAccessibilityOrder(ImmutableArray<AccessLevel> accessibilityOrder)
        {
            if (accessibilityOrder.Length != DefaultAccessibilityOrder.Length)
            {
                throw new InvalidSettingsException("accessibilityOrder must contain each access level exactly once.");
            }

            foreach (var accessLevel in DefaultAccessibilityOrder)
            {
                if (!accessibilityOrder.Contains(accessLevel))
                {
                    throw new InvalidSettingsException("accessibilityOrder must contain each access level exactly once.");
                }
            }

            return accessibilityOrder;
        }
    }
}
