// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Status.Generator
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents a diagnostic in the StyleCop.Analyzers project.
    /// </summary>
    public class StyleCopDiagnostic
    {
        /// <summary>
        /// Gets or sets the ID of the diagnostic, including the prefix 'SA' or 'SX'.
        /// </summary>
        /// <value>
        /// The ID of the diagnostic including, the prefix 'SA' or 'SX'.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the short name if the diagnostic that is used in the class name.
        /// </summary>
        /// <value>
        /// The short name if the diagnostic that is used in the class name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the diagnostic is implemented.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the diagnostic is implemented; otherwise <see langword="false"/>.
        /// </value>
        public bool HasImplementation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the diagnostic is enabled. This can indicate if the
        /// diagnostic is enabled by default, and if not, whether it is disabled because
        /// there are no tests for the diagnostic.
        /// </summary>
        /// <value>
        /// <list type="bullet">
        /// <item>DisabledNoTests</item>
        /// <item>DisabledAlternative</item>
        /// <item>EnabledByDefault</item>
        /// <item>DisabledByDefault</item>
        /// </list>
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the code fix status for the diagnostic.
        /// </summary>
        /// <value>
        /// A value indicating the code fix status for the diagnostic.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public CodeFixStatus CodeFixStatus { get; set; }

        /// <summary>
        /// Gets or sets the reason why a code fix is not implemented, or <see langword="null"/> if there is
        /// no reason.
        /// </summary>
        /// <value>
        /// The reason why a code fix is not implemented, or <see langword="null"/> if there is
        /// no reason.
        /// </value>
        public string NoCodeFixReason { get; set; }

        /// <summary>
        /// Gets or sets the title of this diagnostic.
        /// </summary>
        /// <value>
        /// The title of this diagnostic.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the category of this diagnostic.
        /// </summary>
        /// <value>
        /// The category of this diagnostic.
        /// </value>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the help link for this diagnostic.
        /// </summary>
        /// <value>
        /// The help link for this diagnostic.
        /// </value>
        public string HelpLink { get; set; }

        /// <summary>
        /// Creates an instance of the <see cref="StyleCopDiagnostic"/> class
        /// that is populated with the data stored in <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A JSON string representing a <see cref="StyleCopDiagnostic"/>.</param>
        /// <returns>A <see cref="StyleCopDiagnostic"/> that is populated with the data stored in <paramref name="value"/>.</returns>
        public static StyleCopDiagnostic FromJson(string value)
        {
            return JsonConvert.DeserializeObject<StyleCopDiagnostic>(value);
        }

        /// <summary>
        /// Returns a string representing this diagnostic.
        /// </summary>
        /// <returns>
        /// The string contains the diagnostic id and the short name.
        /// </returns>
        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }

        /// <summary>
        /// Returns a JSON representation of this diagnostic.
        /// </summary>
        /// <returns>A JSON string representing this diagnostic.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
