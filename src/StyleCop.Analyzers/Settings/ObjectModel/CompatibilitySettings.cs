// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel;

using LightJson;
using StyleCop.Analyzers.Lightup;

internal class CompatibilitySettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompatibilitySettings"/> class.
    /// </summary>
    protected internal CompatibilitySettings()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompatibilitySettings"/> class.
    /// </summary>
    /// <param name="compatibilitySettingsObject">The JSON object containing the settings.</param>
    /// <param name="analyzerConfigOptions">The <strong>.editorconfig</strong> options to use if
    /// <strong>stylecop.json</strong> does not provide values.</param>
    protected internal CompatibilitySettings(JsonObject compatibilitySettingsObject, AnalyzerConfigOptionsWrapper analyzerConfigOptions)
    {
        var reader = new SettingsReader(compatibilitySettingsObject, analyzerConfigOptions);

        this.MultiTargetsLegacyFrameworks = reader.GetBoolean("multiTargetsLegacyFrameworks", "stylecop.compatibility.multiTargetsLegacyFrameworks").GetValueOrDefault(false);
    }

    /// <summary>
    /// Gets a value indicating whether the project also targets frameworks that lack newer APIs, so rules must not
    /// require constructs that older targets cannot compile.
    /// </summary>
    /// <value><see langword="true"/> if the project multi-targets legacy frameworks; otherwise,
    /// <see langword="false"/>.</value>
    public bool MultiTargetsLegacyFrameworks { get; }
}
