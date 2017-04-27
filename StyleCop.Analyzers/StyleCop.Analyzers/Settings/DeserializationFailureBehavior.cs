﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers
{
    using Newtonsoft.Json;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Defines the behavior of various <see cref="SettingsHelper"/> methods in the event of a deserialization error.
    /// </summary>
    internal enum DeserializationFailureBehavior
    {
        /// <summary>
        /// When deserialization fails, return a default <see cref="StyleCopSettings"/> instance.
        /// </summary>
        ReturnDefaultSettings,

        /// <summary>
        /// When deserialization fails, throw a <see cref="JsonException"/> containing details about the error.
        /// </summary>
        ThrowException
    }
}
