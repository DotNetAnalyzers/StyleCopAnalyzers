// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.Text;

    public class SourceFileList : List<(string filename, SourceText content)>
    {
        private readonly string defaultPrefix;
        private readonly string defaultExtension;

        public SourceFileList(string defaultPrefix, string defaultExtension)
        {
            this.defaultPrefix = defaultPrefix;
            this.defaultExtension = defaultExtension;
        }

        public void Add(string content)
        {
            this.Add(($"{this.defaultPrefix}{this.Count}.{this.defaultExtension}", content));
        }

        public void Add(SourceText content)
        {
            this.Add(($"{this.defaultPrefix}{this.Count}.{this.defaultExtension}", content));
        }

        public void Add((string filename, string content) file)
        {
            this.Add((file.filename, SourceText.From(file.content)));
        }
    }
}
