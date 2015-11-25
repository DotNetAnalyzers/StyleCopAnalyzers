// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class LayoutSettings
    {
        /// <summary>
        /// This is the backing field for the <see cref="NewlineAtEndOfFile"/> property.
        /// </summary>
        [JsonProperty("newlineAtEndOfFile", DefaultValueHandling = DefaultValueHandling.Include)]
        private EndOfFileHandling newlineAtEndOfFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSettings"/> class during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected internal LayoutSettings()
        {
            this.newlineAtEndOfFile = EndOfFileHandling.Allow;
        }

        public EndOfFileHandling NewlineAtEndOfFile =>
            this.newlineAtEndOfFile;
    }
}
