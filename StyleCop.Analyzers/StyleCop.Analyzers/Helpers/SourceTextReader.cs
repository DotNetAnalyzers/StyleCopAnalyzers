// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.IO;
    using Microsoft.CodeAnalysis.Text;

    // NOTE: Depending on use, Read(char[] buffer, int index, int count) might also need to be
    // implemented, but it is currently not needed.
    internal sealed class SourceTextReader : TextReader
    {
        private readonly SourceText text;
        private int position;

        public SourceTextReader(SourceText text)
        {
            this.text = text;
        }

        public override int Peek()
        {
            if (this.position < this.text.Length)
            {
                return this.text[this.position];
            }
            else
            {
                return -1;
            }
        }

        public override int Read()
        {
            if (this.position < this.text.Length)
            {
                return this.text[this.position++];
            }
            else
            {
                return -1;
            }
        }
    }
}
