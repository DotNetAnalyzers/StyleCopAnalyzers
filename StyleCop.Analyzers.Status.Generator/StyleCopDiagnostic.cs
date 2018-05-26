// Copyright (c) Dennis Fischer. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Status.Generator
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents a diagnostic in the StyleCop.Analyzers project
    /// </summary>
    public class StyleCopDiagnostic
    {
        /// <summary>
        /// The Id of the diagnostic including the prefix 'SA' or 'SX'
        /// </summary>
        /// <value>
        /// The Id of the diagnostic including the prefix 'SA' or 'SX'
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// The short name if the diagnostic that is used in the class name.
        /// </summary>
        /// <value>
        /// The short name if the diagnostic that is used in the class name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Whether or not the diagnostic is implemented.
        /// </summary>
        /// <value>
        /// Whether or not the diagnostic is implemented.
        /// </value>
        public bool HasImplementation { get; set; }

        /// <summary>
        /// Represents if the diagnostic is enabled or not. This can indicate if the
        /// diagnostic is enabled by default or not, or if it is disabled because
        /// there are no tests for the diagnostic.
        /// </summary>
        /// <value>
        /// Represents if the diagnostic is enabled or not. This can indicate if the
        /// diagnostic is enabled by default or not, or if it is disabled because
        /// there are no tests for the diagnostic.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Represents whether or not there is a code fix for the diagnostic.
        /// </summary>
        /// <value>
        /// Represents whether or not there is a code fix for the diagnostic.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public CodeFixStatus CodeFixStatus { get; set; }

        /// <summary>
        /// Returns the reason why a code fix is not implemented, or null if there is
        /// no reason.
        /// </summary>
        /// <value>
        /// Returns the reason why a code fix is not implemented, or null if there is
        /// no reason.
        /// </value>
        public string NoCodeFixReason { get; set; }

        /// <summary>
        /// Returns the title of this diagnostic
        /// no reason.
        /// </summary>
        /// <value>
        /// Returns the title of this diagnostic
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Returns the category of this diagnostic
        /// no reason.
        /// </summary>
        /// <value>
        /// Returns the category of this diagnostic
        /// </value>
        public string Category { get; set; }

        /// <summary>
        /// Returns help link for this diagnostic
        /// </summary>
        /// <value>
        /// Returns help link for this diagnostic
        /// </value>
        public string HelpLink { get; set; }

        /// <summary>
        /// Creates an instance of the <see cref="StyleCopDiagnostic"/> class
        /// that is populated with the data stored in <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A json representing a <see cref="StyleCopDiagnostic"/></param>
        /// <returns>A <see cref="StyleCopDiagnostic"/> that is populated with the data stored in <paramref name="value"/>.</returns>
        public static StyleCopDiagnostic FromJson(string value)
        {
            return JsonConvert.DeserializeObject<StyleCopDiagnostic>(value);
        }

        /// <summary>
        /// Returns a string representing this diagnostic
        /// </summary>
        /// <returns>
        /// The string contains the diagnostic id and the short name.
        /// </returns>
        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }

        /// <summary>
        /// Returns a json representation of this diagnostic
        /// </summary>
        /// <returns>A json string representing this diagnostic</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
