// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class TestScenarios<TSetting>
    {
        protected TestScenarios()
        {
        }

        protected abstract IEnumerable<TSetting> SettingsValues { get; }

        protected abstract IEnumerable<TestScenario<TSetting>> Scenarios { get; }

        internal IEnumerable GetTestData()
        {
            return from setting in this.SettingsValues
                   from scenario in this.Scenarios
                   select new object[] { setting, scenario };
        }
    }
}
