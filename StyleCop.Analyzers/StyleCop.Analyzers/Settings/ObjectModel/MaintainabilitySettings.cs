// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System.Collections.Immutable;
    using LightJson;

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
        private readonly ImmutableArray<TopLevelType>.Builder topLevelTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintainabilitySettings"/> class.
        /// </summary>
        protected internal MaintainabilitySettings()
        {
            this.topLevelTypes = ImmutableArray.CreateBuilder<TopLevelType>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintainabilitySettings"/> class.
        /// </summary>
        /// <param name="maintainabilitySettingsObject">The JSON object containing the settings.</param>
        protected internal MaintainabilitySettings(JsonObject maintainabilitySettingsObject)
            : this()
        {
            foreach (var kvp in maintainabilitySettingsObject)
            {
                switch (kvp.Key)
                {
                case "topLevelTypes":
                    kvp.AssertIsArray();
                    foreach (var value in kvp.Value.AsJsonArray)
                    {
                        var typeKind = value.ToEnumValue<TopLevelType>(kvp.Key);
                        this.topLevelTypes.Add(typeKind);
                    }

                    break;

                default:
                    break;
                }
            }
        }

        public ImmutableArray<TopLevelType> TopLevelTypes
        {
            get
            {
                return this.topLevelTypes.Count > 0 ? this.topLevelTypes.ToImmutable() : DefaultTopLevelTypes;
            }
        }
    }
}
