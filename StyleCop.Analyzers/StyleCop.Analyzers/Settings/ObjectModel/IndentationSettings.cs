// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
            int? indentationSize = null;
            int? tabSize = null;
            bool? useTabs = null;

            foreach (var kvp in indentationSettingsObject)
            {
                switch (kvp.Key)
                {
                case "indentationSize":
                    indentationSize = kvp.ToInt32Value();
                    break;

                case "tabSize":
                    tabSize = kvp.ToInt32Value();
                    break;

                case "useTabs":
                    useTabs = kvp.ToBooleanValue();
                    break;

                default:
                    break;
                }
            }

            indentationSize ??= AnalyzerConfigHelper.TryGetInt32Value(analyzerConfigOptions, "indent_size");
            tabSize ??= AnalyzerConfigHelper.TryGetInt32Value(analyzerConfigOptions, "tab_width");
            useTabs ??= AnalyzerConfigHelper.TryGetStringValue(analyzerConfigOptions, "indent_style") switch
            {
                "tab" => true,
                "space" => false,
                _ => null,
            };

            this.indentationSize = indentationSize.GetValueOrDefault(4);
            this.tabSize = tabSize.GetValueOrDefault(4);
            this.useTabs = useTabs.GetValueOrDefault(false);
        }

        public int IndentationSize =>
            this.indentationSize;

        public int TabSize =>
            this.tabSize;

        public bool UseTabs =>
            this.useTabs;
    }
}
