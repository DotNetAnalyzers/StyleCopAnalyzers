// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Verifiers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.Text;

    public class SourceFileCollection : List<(string filename, SourceText content)>
    {
        public void Add((string filename, string content) file)
        {
            this.Add((file.filename, SourceText.From(file.content)));
        }
    }
}
