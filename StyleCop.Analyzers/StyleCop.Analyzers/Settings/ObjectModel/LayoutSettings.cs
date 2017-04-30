// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;

    internal class LayoutSettings
    {
        /// <summary>
        /// This is the backing field for the <see cref="NewlineAtEndOfFile"/> property.
        /// </summary>
        private EndOfFileHandling newlineAtEndOfFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSettings"/> class.
        /// </summary>
        protected internal LayoutSettings()
        {
            this.newlineAtEndOfFile = EndOfFileHandling.Allow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutSettings"/> class.
        /// </summary>
        /// <param name="layoutSettingsObject">The JSON object containing the settings.</param>
        protected internal LayoutSettings(JsonObject layoutSettingsObject)
            : this()
        {
            foreach (var kvp in layoutSettingsObject)
            {
                switch (kvp.Key)
                {
                case "newlineAtEndOfFile":
                    this.newlineAtEndOfFile = kvp.ToEnumValue<EndOfFileHandling>();
                    break;

                default:
                    throw new InvalidSettingsException($"layoutRules should not contain a child named {kvp.Key}");
                }
            }
        }

        public EndOfFileHandling NewlineAtEndOfFile =>
            this.newlineAtEndOfFile;
    }
}
