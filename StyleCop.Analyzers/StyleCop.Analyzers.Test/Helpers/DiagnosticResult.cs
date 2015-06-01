namespace TestHelper
{
    using System;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Location where the diagnostic appears, as determined by path, line number, and column number.
    /// </summary>
    public struct DiagnosticResultLocation
    {
        public string Path;
        public int Line;
        public int Column;

        public DiagnosticResultLocation(string path, int line, int column)
        {
            if (line < 0 && column < 0)
            {
                throw new ArgumentOutOfRangeException("At least one of line and column must be > 0");
            }

            if (line < -1 || column < -1)
            {
                throw new ArgumentOutOfRangeException("Both line and column must be >= -1");
            }

            this.Path = path;
            this.Line = line;
            this.Column = column;
        }
    }

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
    }
}