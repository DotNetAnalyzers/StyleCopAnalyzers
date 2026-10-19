// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Tools.Fuzz;

internal sealed record Result(bool Compiles, int Diagnostics, int Actions, List<Finding> Findings);
