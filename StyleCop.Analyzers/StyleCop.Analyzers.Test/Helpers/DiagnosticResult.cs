// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace TestHelper
{
    using System;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Structure that stores information about a <see cref="Diagnostic"/> appearing in a source.
    /// </summary>
    public struct DiagnosticResult
    {
        private static readonly object[] EmptyArguments = new object[0];

        private DiagnosticResultLocation[] locations;
        private string message;

        public DiagnosticResult(DiagnosticDescriptor descriptor)
            : this()
        {
            this.Id = descriptor.Id;
            this.Severity = descriptor.DefaultSeverity;
            this.MessageFormat = descriptor.MessageFormat;
        }

        public DiagnosticResultLocation[] Locations
        {
            get
            {
                if (this.locations == null)
                {
                    this.locations = new DiagnosticResultLocation[] { };
                }

                return this.locations;
            }

            set
            {
                this.locations = value;
            }
        }

        public DiagnosticSeverity Severity
        {
            get; set;
        }

        public string Id
        {
            get; set;
        }

        public string Message
        {
            get
            {
                if (this.message != null)
                {
                    return this.message;
                }

                if (this.MessageFormat != null)
                {
                    return string.Format(this.MessageFormat.ToString(), this.MessageArguments ?? EmptyArguments);
                }

                return null;
            }

            set
            {
                this.message = value;
            }
        }

        public LocalizableString MessageFormat
        {
            get;
            set;
        }

        public object[] MessageArguments
        {
            get;
            set;
        }

        public string Path
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Path : string.Empty;
            }
        }

        public int Line
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Line : -1;
            }
        }

        public int Column
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Column : -1;
            }
        }

        public DiagnosticResult WithArguments(params object[] arguments)
        {
            DiagnosticResult result = this;
            result.MessageArguments = arguments;
            return result;
        }

        public DiagnosticResult WithMessageFormat(LocalizableString messageFormat)
        {
            DiagnosticResult result = this;
            result.MessageFormat = messageFormat;
            return result;
        }

        public DiagnosticResult WithLocation(int line, int column)
        {
            return this.WithLocation("Test0.cs", line, column);
        }

        public DiagnosticResult WithLocation(string path, int line, int column)
        {
            DiagnosticResult result = this;
            Array.Resize(ref result.locations, (result.locations?.Length ?? 0) + 1);
            result.locations[result.locations.Length - 1] = new DiagnosticResultLocation(path, line, column);
            return result;
        }

        public DiagnosticResult WithLineOffset(int offset)
        {
            DiagnosticResult result = this;
            Array.Resize(ref result.locations, result.locations?.Length ?? 0);
            for (int i = 0; i < result.locations.Length; i++)
            {
                result.locations[i].Line += offset;
            }

            return result;
        }
    }
}
