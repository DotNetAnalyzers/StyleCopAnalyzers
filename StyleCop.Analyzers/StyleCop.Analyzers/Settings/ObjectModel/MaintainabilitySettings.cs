// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System.Collections.Immutable;
    using LightJson;
    using StyleCop.Analyzers.Lightup;

    internal class MaintainabilitySettings
    {
        /// <summary>
        /// The default value for the <see cref="TopLevelTypes"/> property.
        /// </summary>
        private static readonly ImmutableArray<TopLevelType> DefaultTopLevelTypes =
            ImmutableArray.Create(TopLevelType.Class);

        /// <summary>
        /// This is the backing field for the <see cref="TopLevelTypes"/> property.
        /// </summary>
        private readonly ImmutableArray<TopLevelType> topLevelTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintainabilitySettings"/> class.
        /// </summary>
        protected internal MaintainabilitySettings()
        {
            this.topLevelTypes = ImmutableArray<TopLevelType>.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintainabilitySettings"/> class.
        /// </summary>
        /// <param name="maintainabilitySettingsObject">The JSON object containing the settings.</param>
        /// <param name="analyzerConfigOptions">The <strong>.editorconfig</strong> options to use if
        /// <strong>stylecop.json</strong> does not provide values.</param>
        protected internal MaintainabilitySettings(JsonObject maintainabilitySettingsObject, AnalyzerConfigOptionsWrapper analyzerConfigOptions)
        {
            ImmutableArray<TopLevelType>.Builder topLevelTypes = null;

            foreach (var kvp in maintainabilitySettingsObject)
            {
                switch (kvp.Key)
                {
                case "topLevelTypes":
                    kvp.AssertIsArray();
                    topLevelTypes = ImmutableArray.CreateBuilder<TopLevelType>();
                    foreach (var value in kvp.Value.AsJsonArray)
                    {
                        var typeKind = value.ToEnumValue<TopLevelType>(kvp.Key);
                        topLevelTypes.Add(typeKind);
                    }

                    break;

                default:
                    break;
                }
            }

            this.topLevelTypes = topLevelTypes?.ToImmutable() ?? ImmutableArray<TopLevelType>.Empty;
        }

        public ImmutableArray<TopLevelType> TopLevelTypes
        {
            get
            {
                return this.topLevelTypes.Length > 0 ? this.topLevelTypes : DefaultTopLevelTypes;
            }
        }
    }
}
