// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;

    internal class IndentationSettings
    {
        /// <summary>
        /// This is the backing field for the <see cref="IndentationSize"/> property.
        /// </summary>
        private readonly int indentationSize;

        /// <summary>
        /// This is the backing field for the <see cref="TabSize"/> property.
        /// </summary>
        private readonly int tabSize;

        /// <summary>
        /// This is the backing field for the <see cref="UseTabs"/> property.
        /// </summary>
        private readonly bool useTabs;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndentationSettings"/> class.
        /// </summary>
        protected internal IndentationSettings()
        {
            this.indentationSize = 4;
            this.tabSize = 4;
            this.useTabs = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndentationSettings"/> class.
        /// </summary>
        /// <param name="indentationSettingsObject">The JSON object containing the settings.</param>
        protected internal IndentationSettings(JsonObject indentationSettingsObject)
            : this()
        {
            foreach (var kvp in indentationSettingsObject)
            {
                switch (kvp.Key)
                {
                case "indentationSize":
                    this.indentationSize = kvp.ToInt32Value();
                    break;

                case "tabSize":
                    this.tabSize = kvp.ToInt32Value();
                    break;

                case "useTabs":
                    this.useTabs = kvp.ToBooleanValue();
                    break;

                default:
                    break;
                }
            }
        }

        public int IndentationSize =>
            this.indentationSize;

        public int TabSize =>
            this.tabSize;

        public bool UseTabs =>
            this.useTabs;
    }
}
