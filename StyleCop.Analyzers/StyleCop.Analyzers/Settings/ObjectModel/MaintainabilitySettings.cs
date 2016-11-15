// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
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
        [JsonProperty("topLevelTypes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<TopLevelType>.Builder topLevelTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaintainabilitySettings"/> class during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected internal MaintainabilitySettings()
        {
            this.topLevelTypes = ImmutableArray.CreateBuilder<TopLevelType>();
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
