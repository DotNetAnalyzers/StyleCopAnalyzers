// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using LightJson;
    using StyleCop.Analyzers.Lightup;

    internal class DocumentationSettings
    {
        /// <summary>
        /// The default value for the <see cref="CompanyName"/> property.
        /// </summary>
        internal const string DefaultCompanyName = "PlaceholderCompany";

        /// <summary>
        /// The default value for the <see cref="GetCopyrightText(string)"/> method.
        /// </summary>
        internal const string DefaultCopyrightText = "Copyright (c) {companyName}. All rights reserved.";

        /// <summary>
        /// The default value for the <see cref="DocumentationCulture"/> property.
        /// </summary>
        internal const string DefaultDocumentationCulture = "en-US";

        /// <summary>
        /// The default value for the <see cref="ExcludeFromPunctuationCheck"/> property.
        /// </summary>
        internal static readonly ImmutableArray<string> DefaultExcludeFromPunctuationCheck = ImmutableArray.Create("seealso");

        /// <summary>
        /// This is the backing field for the <see cref="CompanyName"/> property.
        /// </summary>
        private readonly string companyName;

        /// <summary>
        /// This is the backing field for the <see cref="GetCopyrightText(string)"/> method.
        /// </summary>
        private readonly string copyrightText;

        /// <summary>
        /// This is the backing field for the <see cref="HeaderDecoration"/> property.
        /// </summary>
        private readonly string headerDecoration;

        /// <summary>
        /// This is the backing field for the <see cref="Variables"/> property.
        /// </summary>
        private readonly ImmutableDictionary<string, string> variables;

        /// <summary>
        /// This is the backing field for the <see cref="XmlHeader"/> property.
        /// </summary>
        private readonly bool xmlHeader;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentExposedElements"/> property.
        /// </summary>
        private readonly bool documentExposedElements;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentInternalElements"/> property.
        /// </summary>
        private readonly bool documentInternalElements;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentPrivateElements"/> property.
        /// </summary>
        private readonly bool documentPrivateElements;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentInterfaces"/> property.
        /// </summary>
        private readonly InterfaceDocumentationMode documentInterfaces;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentPrivateFields"/> property.
        /// </summary>
        private readonly bool documentPrivateFields;

        /// <summary>
        /// This is the backing field for the <see cref="FileNamingConvention"/> property.
        /// </summary>
        private readonly FileNamingConvention fileNamingConvention;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentationCulture"/> property.
        /// </summary>
        private readonly string documentationCulture;

        /// <summary>
        /// This is backing field for the <see cref="DocumentationCultureInfo"/> property.
        /// </summary>
        private readonly CultureInfo documentationCultureInfo;

        /// <summary>
        /// This is the backing field for the <see cref="ExcludeFromPunctuationCheck"/> property.
        /// </summary>
        private readonly ImmutableArray<string> excludeFromPunctuationCheck;

        /// <summary>
        /// This is the cache for the <see cref="GetCopyrightText(string)"/> method.
        /// </summary>
        private string copyrightTextCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationSettings"/> class during JSON deserialization.
        /// </summary>
        protected internal DocumentationSettings()
        {
            this.companyName = DefaultCompanyName;
            this.copyrightText = DefaultCopyrightText;
            this.headerDecoration = string.Empty;
            this.variables = ImmutableDictionary<string, string>.Empty;
            this.xmlHeader = true;

            this.documentExposedElements = true;
            this.documentInternalElements = true;
            this.documentPrivateElements = false;
            this.documentInterfaces = InterfaceDocumentationMode.All;
            this.documentPrivateFields = false;

            this.fileNamingConvention = FileNamingConvention.StyleCop;

            this.documentationCulture = DefaultDocumentationCulture;
            this.documentationCultureInfo = CultureInfo.InvariantCulture;

            this.excludeFromPunctuationCheck = DefaultExcludeFromPunctuationCheck;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationSettings"/> class.
        /// </summary>
        /// <param name="documentationSettingsObject">The JSON object containing the settings.</param>
        /// <param name="analyzerConfigOptions">The <strong>.editorconfig</strong> options to use if
        /// <strong>stylecop.json</strong> does not provide values.</param>
        protected internal DocumentationSettings(JsonObject documentationSettingsObject, AnalyzerConfigOptionsWrapper analyzerConfigOptions)
        {
            var reader = new SettingsReader(documentationSettingsObject, analyzerConfigOptions);

            this.documentExposedElements = reader.GetBoolean("documentExposedElements", "stylecop.documentation.documentExposedElements").GetValueOrDefault(true);
            this.documentInternalElements = reader.GetBoolean("documentInternalElements", "stylecop.documentation.documentInternalElements").GetValueOrDefault(true);
            this.documentPrivateElements = reader.GetBoolean("documentPrivateElements", "stylecop.documentation.documentPrivateElements").GetValueOrDefault(false);
            this.documentInterfaces = reader.GetMapped<InterfaceDocumentationMode>(
                "documentInterfaces",
                ParseDocumentInterfacesValue,
                "stylecop.documentation.documentInterfaces",
                ParseDocumentInterfacesEditorConfigValue) ?? InterfaceDocumentationMode.All;
            this.documentPrivateFields = reader.GetBoolean("documentPrivateFields", "stylecop.documentation.documentPrivateFields").GetValueOrDefault(false);
            this.companyName = reader.GetString("companyName", "stylecop.documentation.companyName") ?? DefaultCompanyName;
            this.copyrightText = reader.GetMultiLineString("copyrightText", "stylecop.documentation.copyrightText", "file_header_template") ?? DefaultCopyrightText;
            this.headerDecoration = reader.GetString("headerDecoration", "stylecop.documentation.headerDecoration") ?? string.Empty;
            this.variables = reader.GetStringMap("variables", "stylecop.documentation.variables.", IsValidVariableName) ?? ImmutableDictionary<string, string>.Empty;
            this.xmlHeader = reader.GetBoolean("xmlHeader", "stylecop.documentation.xmlHeader").GetValueOrDefault(true);
            this.fileNamingConvention = reader.GetMapped<FileNamingConvention>(
                "fileNamingConvention",
                kvp => kvp.ToEnumValue<FileNamingConvention>(),
                "stylecop.documentation.fileNamingConvention",
                value => value switch
                {
                    "stylecop" => FileNamingConvention.StyleCop,
                    "metadata" => FileNamingConvention.Metadata,
                    _ => null,
                }).GetValueOrDefault(FileNamingConvention.StyleCop);
            this.documentationCulture = reader.GetString("documentationCulture", "stylecop.documentation.documentationCulture") ?? DefaultDocumentationCulture;
            this.documentationCultureInfo = this.documentationCulture == DefaultDocumentationCulture ? CultureInfo.InvariantCulture : new CultureInfo(this.documentationCulture);
            this.excludeFromPunctuationCheck = reader.GetStringList("excludeFromPunctuationCheck", "stylecop.documentation.excludeFromPunctuationCheck") ?? DefaultExcludeFromPunctuationCheck;
        }

        public string CompanyName
        {
            get
            {
                return this.companyName;
            }
        }

        public string HeaderDecoration
        {
            get
            {
                return this.headerDecoration;
            }
        }

        public ImmutableDictionary<string, string> Variables
        {
            get
            {
                return this.variables;
            }
        }

        public bool XmlHeader
        {
            get
            {
                return this.xmlHeader;
            }
        }

        public bool DocumentExposedElements =>
            this.documentExposedElements;

        public bool DocumentInternalElements =>
            this.documentInternalElements;

        public bool DocumentPrivateElements =>
            this.documentPrivateElements;

        public InterfaceDocumentationMode DocumentInterfaces =>
            this.documentInterfaces;

        public bool DocumentPrivateFields =>
            this.documentPrivateFields;

        public FileNamingConvention FileNamingConvention =>
            this.fileNamingConvention;

        public string DocumentationCulture =>
            this.documentationCulture;

        public ImmutableArray<string> ExcludeFromPunctuationCheck
            => this.excludeFromPunctuationCheck;

        public CultureInfo DocumentationCultureInfo
            => this.documentationCultureInfo;

        public string GetCopyrightText(string fileName)
        {
            string copyrightText = this.copyrightTextCache;
            if (copyrightText != null)
            {
                return copyrightText;
            }

            var expandedCopyrightText = this.BuildCopyrightText(fileName);
            if (!expandedCopyrightText.Value)
            {
                // Unable to cache the copyright text due to use of a {fileName} variable.
                return expandedCopyrightText.Key;
            }

            this.copyrightTextCache = expandedCopyrightText.Key;
            return this.copyrightTextCache;
        }

        private static InterfaceDocumentationMode ParseDocumentInterfacesValue(KeyValuePair<string, JsonValue> kvp)
        {
            if (kvp.Value.IsBoolean)
            {
                return kvp.Value.AsBoolean ? InterfaceDocumentationMode.All : InterfaceDocumentationMode.None;
            }

            if (kvp.Value.IsString)
            {
                return kvp.ToEnumValue<InterfaceDocumentationMode>();
            }

            throw new StyleCop.Analyzers.InvalidSettingsException($"{kvp.Key} must contain a boolean or string value");
        }

        private static InterfaceDocumentationMode? ParseDocumentInterfacesEditorConfigValue(string value)
        {
            if (bool.TryParse(value, out var boolValue))
            {
                return boolValue ? InterfaceDocumentationMode.All : InterfaceDocumentationMode.None;
            }

            switch (value.ToLowerInvariant())
            {
            case "all":
                return InterfaceDocumentationMode.All;
            case "exposed":
                return InterfaceDocumentationMode.Exposed;
            case "none":
                return InterfaceDocumentationMode.None;
            default:
                return null;
            }
        }

        private static bool IsValidVariableName(string name)
        {
            // Equivalent to Regex.IsMatch(prefix, "^[a-zA-Z0-9]+$")
            for (var i = 0; i < name.Length; i++)
            {
                if (name[i] is not ((>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9')))
                {
                    return false;
                }
            }

            return name.Length > 0;
        }

        private KeyValuePair<string, bool> BuildCopyrightText(string fileName)
        {
            bool canCache = true;

            string Evaluator(Match match)
            {
                string key = match.Groups["Property"].Value;
                switch (key)
                {
                case "companyName":
                    return this.CompanyName;

                case "copyrightText":
                    return "[CircularReference]";

                default:
                    string value;
                    if (this.Variables.TryGetValue(key, out value)
                        || this.Variables.TryGetValue(key.ToLowerInvariant(), out value))
                    {
                        return value;
                    }

                    if (key == "fileName")
                    {
                        // The 'fileName' built-in variable is only applied when the user did not include an
                        // explicit value for a custom 'fileName' variable.
                        canCache = false;
                        return fileName;
                    }

                    break;
                }

                return "[InvalidReference]";
            }

            string pattern = Regex.Escape("{") + "(?<Property>[a-zA-Z0-9]+)" + Regex.Escape("}");
            string expanded = Regex.Replace(this.copyrightText, pattern, Evaluator);
            return new KeyValuePair<string, bool>(expanded, canCache);
        }
    }
}
