namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using TestHelper;

    public abstract class SampleCodeBuilder
    {
        private const string Indent = @"    ";

        private static readonly Regex DiagnosticMarkerPattern = new Regex(@"\~\~([^\~]*?)\~\~", RegexOptions.Compiled);

        private readonly StringBuilder stringBuilder = new StringBuilder();

        private ImmutableList<DiagnosticResultLocation> diagnosticLocations = ImmutableList<DiagnosticResultLocation>.Empty;

        private int indentLevel;
        private int linesWritten;
        private int columnIndex;

        public IImmutableList<DiagnosticResultLocation> DiagnosticLocations
        {
            get { return this.diagnosticLocations; }
        }

        public string GeneratedText
        {
            get { return this.stringBuilder.ToString(); }
        }

        public void Reset()
        {
            this.diagnosticLocations = ImmutableList<DiagnosticResultLocation>.Empty;
            this.stringBuilder.Clear();
            this.indentLevel = 0;
            this.linesWritten = 0;
            this.columnIndex = 0;
        }

        public void PushIndent()
        {
            ++this.indentLevel;
        }

        public void PopIndent()
        {
            --this.indentLevel;
        }

        public IDisposable SuppressIndent()
        {
            return new RestoreIndentDisposable(this);
        }

        public void WriteLine(string line)
        {
            StringReader reader = new StringReader(line);
            string addLine = reader.ReadLine();
            while (addLine != null)
            {
                if (this.columnIndex == 0)
                {
                    this.WriteIndent();
                }

                addLine = this.AddDiagnosticLocationsAndReturnPlainString(addLine);

                this.stringBuilder.AppendLine(addLine);
                this.columnIndex = 0;

                addLine = reader.ReadLine();
                ++this.linesWritten;
            }
        }

        public void Write(string line)
        {
            string[] lines = line.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                if (this.columnIndex == 0)
                {
                    this.WriteIndent();
                }

                string addLine = lines[i];
                addLine = this.AddDiagnosticLocationsAndReturnPlainString(addLine);

                // Write line with markers removed to output.
                this.stringBuilder.Append(addLine);
                this.columnIndex += addLine.Length;

                if (i < (lines.Length - 1))
                {
                    ++this.linesWritten;
                    this.columnIndex = 0;
                    this.stringBuilder.AppendLine();
                }
            }
        }

        private void WriteIndent()
        {
            for (int i = 0; i < this.indentLevel; i++)
            {
                this.stringBuilder.Append(Indent);
                this.columnIndex += Indent.Length;
            }
        }

        private string AddDiagnosticLocationsAndReturnPlainString(string textWithMarkers)
        {
            string addLine = textWithMarkers;

            var markerMatch = DiagnosticMarkerPattern.Match(addLine);
            while (markerMatch.Success)
            {
                // Add diagnostic location for the marker.
                this.diagnosticLocations = this.diagnosticLocations.Add(
                    new DiagnosticResultLocation(null, this.linesWritten + 1, markerMatch.Index + this.columnIndex + 1));

                // Remove the marker from the line.
                addLine = DiagnosticMarkerPattern.Replace(addLine, @"$1", 1);

                markerMatch = DiagnosticMarkerPattern.Match(addLine);
            }

            return addLine;
        }

        private sealed class RestoreIndentDisposable : IDisposable
        {
            private int saved;
            private SampleCodeBuilder restoreTo;

            public RestoreIndentDisposable(SampleCodeBuilder builder)
            {
                this.restoreTo = builder;
                this.saved = builder.indentLevel;
                builder.indentLevel = 0;
            }

            public void Dispose()
            {
                this.restoreTo.indentLevel = this.saved;
            }
        }
    }
}
