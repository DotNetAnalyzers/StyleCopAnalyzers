// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System.Collections.Immutable;
    using System.Text.RegularExpressions;
    using LightJson;

    internal class NamingSettings
    {
        /// <summary>
        /// This is the backing field for the <see cref="AllowedHungarianPrefixes"/> property.
        /// </summary>
        private readonly ImmutableArray<string>.Builder allowedHungarianPrefixes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamingSettings"/> class.
        /// </summary>
        protected internal NamingSettings()
        {
            this.AllowCommonHungarianPrefixes = true;
            this.allowedHungarianPrefixes = ImmutableArray.CreateBuilder<string>();

            this.IncludeInferredTupleElementNames = false;
            this.TupleElementNameCasing = TupleElementNameCase.PascalCase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamingSettings"/> class.
        /// </summary>
        /// <param name="namingSettingsObject">The JSON object containing the settings.</param>
        protected internal NamingSettings(JsonObject namingSettingsObject)
            : this()
        {
            foreach (var kvp in namingSettingsObject)
            {
                switch (kvp.Key)
                {
                case "allowCommonHungarianPrefixes":
                    this.AllowCommonHungarianPrefixes = kvp.ToBooleanValue();
                    break;

                case "allowedHungarianPrefixes":
                    kvp.AssertIsArray();
                    foreach (var prefixJsonValue in kvp.Value.AsJsonArray)
                    {
                        var prefix = prefixJsonValue.ToStringValue(kvp.Key);

                        if (!Regex.IsMatch(prefix, "^[a-z]{1,2}$"))
                        {
                            continue;
                        }

                        this.allowedHungarianPrefixes.Add(prefix);
                    }

                    break;

                case "includeInferredTupleElementNames":
                    this.IncludeInferredTupleElementNames = kvp.ToBooleanValue();
                    break;

                case "tupleElementNameCasing":
                    this.TupleElementNameCasing = kvp.ToEnumValue<TupleElementNameCase>();
                    break;

                default:
                    break;
                }
            }
        }

        public bool AllowCommonHungarianPrefixes { get; }

        public ImmutableArray<string> AllowedHungarianPrefixes
            => this.allowedHungarianPrefixes.ToImmutable();

        public bool IncludeInferredTupleElementNames { get; }

        public TupleElementNameCase TupleElementNameCasing { get; }
    }
}
