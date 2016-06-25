// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    internal sealed class ExpectedViolation<TSetting>
    {
        internal ExpectedViolation(TSetting setting, int line, int column)
        {
            this.Setting = setting;
            this.Line = line;
            this.Column = column;
        }

        internal TSetting Setting { get; }

        internal int Line { get; }

        internal int Column { get; }
    }
}
