// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;
    using StyleCop.Analyzers.Lightup;

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
        /// <param name="analyzerConfigOptions">The <strong>.editorconfig</strong> options to use if
        /// <strong>stylecop.json</strong> does not provide values.</param>
        protected internal IndentationSettings(JsonObject indentationSettingsObject, AnalyzerConfigOptionsWrapper analyzerConfigOptions)
        {
            var reader = new SettingsReader(indentationSettingsObject, analyzerConfigOptions);

            this.indentationSize = reader.GetInt32("indentationSize", "indent_size").GetValueOrDefault(4);
            this.tabSize = reader.GetInt32("tabSize", "tab_width").GetValueOrDefault(4);
            this.useTabs = reader.GetMapped<bool>(
                "useTabs",
                kvp => kvp.ToBooleanValue(),
                "indent_style",
                value => value switch
                {
                    "tab" => true,
                    "space" => false,
                    _ => null,
                }).GetValueOrDefault(false);
        }

        public int IndentationSize =>
            this.indentationSize;

        public int TabSize =>
            this.tabSize;

        public bool UseTabs =>
            this.useTabs;
    }
}
