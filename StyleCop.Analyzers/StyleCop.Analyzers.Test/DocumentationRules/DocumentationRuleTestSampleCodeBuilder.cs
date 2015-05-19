namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    public sealed class DocumentationRuleTestSampleCodeBuilder : SampleCodeBuilder
    {
        private readonly ImmutableArray<string> diagnosticIds;

        public DocumentationRuleTestSampleCodeBuilder()
            : this(null)
        {
        }

        public DocumentationRuleTestSampleCodeBuilder(IEnumerable<string> diagnosticIds)
        {
            this.diagnosticIds = diagnosticIds == null
                ? ImmutableArray<string>.Empty
                : diagnosticIds.Where(x => !string.IsNullOrEmpty(x)).Distinct(StringComparer.Ordinal).ToImmutableArray();
        }

        public void WriteClassStart(string name, DocumentationOptions documentationOptions, ExpectedResult expectError, params string[] modifiers)
        {
            this.WriteClassStart(name, documentationOptions, expectError, modifiers, null);
        }

        public void WriteClassStart(string name, DocumentationOptions documentationOptions, ExpectedResult expectError, string[] modifiers, string[] inherits)
        {
            switch (documentationOptions)
            {
                case DocumentationOptions.WriteSampleDocumentation:
                    this.Write("/// <summary>\r\n/// Some class documentation.\r\n/// </summary>\r\n");
                    break;
            }

            this.Write(ConcatModifiers(modifiers));
            this.Write("class ");
            this.Write(BuildSymbolWithPositionMarkers(name, expectError));

            if (inherits != null && inherits.Any())
            {
                this.Write(" : ");
                this.Write(string.Join(", ", inherits));
            }

            this.Write("\r\n");
            this.WriteLine("{");

            this.PushIndent();
        }

        public void WriteStructStart(string name, DocumentationOptions documentationOptions, ExpectedResult expectError, params string[] modifiers)
        {
            switch (documentationOptions)
            {
                case DocumentationOptions.WriteSampleDocumentation:
                    this.Write("/// <summary>\r\n/// Some struct documentation.\r\n/// </summary>\r\n");
                    break;
            }

            this.Write(ConcatModifiers(modifiers));
            this.Write("struct ");
            this.Write(BuildSymbolWithPositionMarkers(name, expectError));
            this.Write("\r\n{\r\n");
            this.PushIndent();
        }

        public void WriteEnumStart(string name, DocumentationOptions documentationOptions, ExpectedResult expectError, params string[] modifiers)
        {
            switch (documentationOptions)
            {
                case DocumentationOptions.WriteSampleDocumentation:
                    this.Write("/// <summary>\r\n/// Some enum documentation.\r\n/// </summary>\r\n");
                    break;
            }

            this.Write(ConcatModifiers(modifiers));
            this.Write("enum ");
            this.Write(BuildSymbolWithPositionMarkers(name, expectError));
            this.Write("\r\n{\r\n");
            this.PushIndent();
        }

        public void WriteInterfaceStart(string name, DocumentationOptions documentationOptions, ExpectedResult expectError, params string[] modifiers)
        {
            switch (documentationOptions)
            {
                case DocumentationOptions.WriteSampleDocumentation:
                    this.Write("/// <summary>\r\n/// Some interface documentation.\r\n/// </summary>\r\n");
                    break;
            }

            this.Write(ConcatModifiers(modifiers));
            this.Write("interface ");
            this.Write(BuildSymbolWithPositionMarkers(name, expectError));
            this.Write("\r\n{\r\n");
            this.PushIndent();
        }

        public void WriteConstructor(string name, DocumentationOptions documentationOptions, ExpectedResult expectError, params string[] modifiers)
        {
            switch (documentationOptions)
            {
                case DocumentationOptions.WriteSampleDocumentation:
                    this.Write("/// <summary>\r\n/// Some constructor documentation.\r\n/// </summary>\r\n");
                    break;
            }

            this.Write(ConcatModifiers(modifiers));
            this.Write(BuildSymbolWithPositionMarkers(name, expectError));

            this.Write("()\r\n{\r\n}\r\n");

        }

        public void WriteField(string name, DocumentationOptions documentationOptions, ExpectedResult expectError, params string[] modifiers)
        {
            switch (documentationOptions)
            {
                case DocumentationOptions.WriteSampleDocumentation:
                    this.Write("/// <summary>\r\n/// Some field documentation.\r\n/// </summary>\r\n");
                    break;
            }

            this.Write(ConcatModifiers(modifiers));
            this.Write("object ");
            this.Write(BuildSymbolWithPositionMarkers(name, expectError));
            this.Write(";\r\n");
        }

        public void WriteDestructor(string name, DocumentationOptions documentationOptions, ExpectedResult expectError)
        {
            switch (documentationOptions)
            {
                case DocumentationOptions.WriteSampleDocumentation:
                    this.Write("/// <summary>\r\n/// Some finalizer documentation.\r\n/// </summary>\r\n");
                    break;
            }

            this.Write("~");
            this.Write(BuildSymbolWithPositionMarkers(name, expectError));
            this.Write("()\r\n{\r\n}\r\n");
        }

        public void WriteIndexer(DocumentationOptions documentationOptions, ExpectedResult expectError, params string[] modifiers)
        {
            switch (documentationOptions)
            {
                case DocumentationOptions.WriteSampleDocumentation:
                    this.Write("/// <summary>\r\n/// Some indexer documentation.\r\n/// </summary>\r\n/// <param name=\"" +
                            "ix\">The index.</param>\r\n");
                    break;
            }

            this.Write(ConcatModifiers(modifiers));
            this.Write("object ");
            this.Write(BuildSymbolWithPositionMarkers("this", expectError));
            this.Write("[int ix]\r\n{\r\n\tget { return null; }\r\n}\r\n");
        }

        public void WriteDelegate(string name, DocumentationOptions documentationOptions, ExpectedResult expectError, params string[] modifiers)
        {
            switch (documentationOptions)
            {
                case DocumentationOptions.WriteSampleDocumentation:
                    this.Write("/// <summary>\r\n/// Some delegate documentation.\r\n/// </summary>\r\n");
                    break;
            }

            this.Write(ConcatModifiers(modifiers));
            this.Write("delegate void ");
            this.Write(BuildSymbolWithPositionMarkers(name, expectError));
            this.Write("();\r\n");
        }

        public void WriteEvent(string name, DocumentationOptions documentationOptions, ExpectedResult expectError, params string[] modifiers)
        {
            switch (documentationOptions)
            {
                case DocumentationOptions.WriteSampleDocumentation:
                    this.Write("/// <summary>\r\n/// Some event documentation.\r\n/// </summary>\r\n");
                    break;
            }

            this.Write(ConcatModifiers(modifiers));
            this.Write("event System.EventHandler ");
            this.Write(BuildSymbolWithPositionMarkers(name, expectError));
            this.Write(";\r\n");
        }

        public void WriteMethod(string name, DocumentationOptions documentationOptions, ExpectedResult expectError, params string[] modifiers)
        {
            switch (documentationOptions)
            {
                case DocumentationOptions.WriteSampleDocumentation:
                    this.Write("/// <summary>\r\n/// Some method documentation.\r\n/// </summary>\r\n");
                    break;
            }

            this.Write(ConcatModifiers(modifiers));
            this.Write("void ");
            this.Write(BuildSymbolWithPositionMarkers(name, expectError));
            this.Write("()\r\n{\r\n}\r\n");
        }

        public void WriteProperty(string name, DocumentationOptions documentationOptions, ExpectedResult expectError, params string[] modifiers)
        {
            switch (documentationOptions)
            {
                case DocumentationOptions.WriteSampleDocumentation:
                    this.Write("/// <summary>\r\n/// Some property documentation.\r\n/// </summary>\r\n");
                    break;
            }

            this.Write(ConcatModifiers(modifiers));
            this.Write("object ");
            this.Write(BuildSymbolWithPositionMarkers(name, expectError));
            this.Write("\r\n{\r\n\tget { return null; }\r\n\tset { }\r\n}\r\n");
        }

        public void WriteExplicitInterfaceProperty(string name, string interfaceName, DocumentationOptions documentationOptions, ExpectedResult expectedResult)
        {
            if (documentationOptions == DocumentationOptions.WriteSampleDocumentation)
            {
                this.WriteLine("/// <summary>");
                this.WriteLine("/// Some summary.");
                this.WriteLine("/// </summary>");
            }

            this.WriteLine($"string {interfaceName}.{BuildSymbolWithPositionMarkers(name, expectedResult)}");
            this.WriteLine(string.Empty);
            this.WriteLine("{");
            this.PushIndent();
            this.WriteLine("get { return null; }");
            this.WriteLine("set { }");
            this.PopIndent();
            this.WriteLine("}");

            this.WriteSuppressionStart();
            this.WriteInterfaceStart(interfaceName, DocumentationOptions.OmitSampleDocumentation, ExpectedResult.NoDiagnostic, "public");
            this.WriteLine($"string {name} {{ get; set; }}");
            this.WriteBlockEnd();
            this.WriteSuppressionEnd();
        }

        private void WriteSuppressionStart()
        {
            using (this.SuppressIndent())
            {
                foreach (var diagnosticId in this.diagnosticIds)
                {
                    this.WriteLine($"#pragma warning disable {diagnosticId}");
                }
            }
        }

        private void WriteSuppressionEnd()
        {
            using (this.SuppressIndent())
            {
                foreach (var diagnosticId in this.diagnosticIds)
                {
                    this.WriteLine($"#pragma warning restore {diagnosticId}");
                }
            }
        }

        public void WriteBlockEnd()
        {
            this.PopIndent();
            this.WriteLine("}");
        }

        public void WriteAutoGeneratedHeader()
        {
            this.Write(@"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

");
        }

        private static string ConcatModifiers(IEnumerable<string> modifiers)
        {
            if (modifiers == null || !modifiers.Any())
            {
                return string.Empty;
            }

            return string.Join(" ", modifiers) + " ";
        }

        private static string BuildSymbolWithPositionMarkers(string symbol, ExpectedResult writeMarkers)
        {
            return writeMarkers == ExpectedResult.Diagnostic ? string.Format("~~{0}~~", symbol) : symbol;
        }
    }
}
