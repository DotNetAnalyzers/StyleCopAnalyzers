// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using Xunit.Abstractions;

    internal sealed class ExpectedViolation<TSetting> : IXunitSerializable
    {
        public ExpectedViolation()
        {
        }

        internal ExpectedViolation(TSetting setting, int line, int column)
        {
            this.Setting = setting;
            this.Line = line;
            this.Column = column;
        }

        internal TSetting Setting { get; private set; }

        internal int Line { get; private set; }

        internal int Column { get; private set; }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(this.Setting), this.Setting);
            info.AddValue(nameof(this.Line), this.Line);
            info.AddValue(nameof(this.Column), this.Column);
        }

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
        {
            this.Setting = info.GetValue<TSetting>(nameof(this.Setting));
            this.Line = info.GetValue<int>(nameof(this.Line));
            this.Column = info.GetValue<int>(nameof(this.Column));
        }
    }
}
