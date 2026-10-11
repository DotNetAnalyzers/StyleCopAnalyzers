// Probe harness: runs every StyleCop analyzer (and optionally the first code fix of each diagnostic) on C# snippet
// files and prints diagnostics, compiler errors, analyzer exceptions (AD0001) and code fix results.
//
// Usage: dotnet run --project <this folder> -- [--fix] [--lang 13|preview] [--ignore SA1,SA2] [--keep SA1,SA2] [--sx] file-or-dir...
//   --fix     apply the first code fix for each diagnostic and report "ok" or "BAD-FIX" (new compiler errors), plus the diff
//   --lang    language version (default: preview)
//   --ignore  hide more diagnostic IDs; --keep shows IDs hidden by default
//   --sx      include the alternative SX rules
// Per file: a line "// probe-keep: SA1600" or "// probe-ignore: SA1101" adjusts the hidden IDs; a sibling <name>.json is
// used as stylecop.json.
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

var fix = false; var sx = false; var lang = LanguageVersion.Preview;
var ignore = new HashSet<string> { "SA1633", "SA1600", "SA1601", "SA1602", "SA0001", "SA1652", "SA1200", "SA1516", "SA1649", "SA1402", "SA1403", "SA1101", "SA1413", "SA1309", "SA1311", "SA1404", "SA1412" };
var paths = new List<string>();
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--fix") fix = true;
    else if (args[i] == "--sx") sx = true;
    else if (args[i] == "--ignore") foreach (var s in args[++i].Split(',')) ignore.Add(s);
    else if (args[i] == "--keep") foreach (var s in args[++i].Split(',')) ignore.Remove(s);
    else if (args[i] == "--lang") LanguageVersionFacts.TryParse(args[++i], out lang);
    else paths.Add(args[i]);
}
var files = paths.SelectMany(p => Directory.Exists(p) ? Directory.GetFiles(p, "*.cs").OrderBy(x => x).AsEnumerable() : new[] { p }).ToList();

var asm = Assembly.Load("StyleCop.Analyzers");
var fixAsm = Assembly.Load("StyleCop.Analyzers.CodeFixes");
var analyzers = asm.GetTypes().Concat(fixAsm.GetTypes())
    .Where(t => !t.IsAbstract && typeof(DiagnosticAnalyzer).IsAssignableFrom(t) && t.GetCustomAttributes(typeof(DiagnosticAnalyzerAttribute), false).Any())
    .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t)).ToImmutableArray();
var fixers = fixAsm.GetTypes().Where(t => !t.IsAbstract && typeof(CodeFixProvider).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null)
    .Select(t => (CodeFixProvider)Activator.CreateInstance(t)).ToList();
var allIds = analyzers.SelectMany(a => a.SupportedDiagnostics).Select(d => d.Id).Distinct().ToList();
var refs = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator)
    .Where(p => Path.GetFileName(p).StartsWith("System") || Path.GetFileName(p) == "netstandard.dll" || Path.GetFileName(p) == "mscorlib.dll")
    .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p)).ToList();

foreach (var file in files)
{
    var code = File.ReadAllText(file).Replace("\r\n", "\n");
    var localIgnore = new HashSet<string>(ignore);
    foreach (var line in code.Split('\n').Where(l => l.StartsWith("// probe-keep:"))) foreach (var s in line.Substring(14).Split(',')) localIgnore.Remove(s.Trim());
    foreach (var line in code.Split('\n').Where(l => l.StartsWith("// probe-ignore:"))) foreach (var s in line.Substring(16).Split(',')) localIgnore.Add(s.Trim());
    Console.WriteLine($"===== {Path.GetFileName(file)}");
    var (doc, ws) = CreateDocument(code, file);
    var (diags, errs, exc) = await Analyze(doc);
    foreach (var e in errs) Console.WriteLine($"  !! compiler {e.Id} {Pos(e)} {e.GetMessage()}");
    foreach (var e in exc) Console.WriteLine($"  !!! EXCEPTION {e}");
    var lines = code.Split('\n');
    foreach (var d in diags.Where(d => !localIgnore.Contains(d.Id) && (sx || !d.Id.StartsWith("SX"))).OrderBy(d => d.Location.SourceSpan.Start).ThenBy(d => d.Id))
    {
        var lp = d.Location.GetLineSpan().StartLinePosition;
        Console.WriteLine($"  {d.Id} {lp.Line + 1}:{lp.Character + 1} {d.GetMessage()}  | {lines[lp.Line].Trim()}");
        if (fix) await TryFix(doc, d, errs.Length);
    }
}

async Task TryFix(Document doc, Diagnostic d, int baseErrors)
{
    foreach (var f in fixers.Where(f => f.FixableDiagnosticIds.Contains(d.Id)))
    {
        var actions = new List<CodeAction>();
        var ctx = new CodeFixContext(doc, d, (a, _) => actions.Add(a), CancellationToken.None);
        try { await f.RegisterCodeFixesAsync(ctx); } catch (Exception ex) { Console.WriteLine($"      !!! FIX-REGISTER EXCEPTION {f.GetType().Name}: {ex.GetType().Name} {ex.Message}"); continue; }
        foreach (var a in actions.Take(1))
        {
            try
            {
                var ops = await a.GetOperationsAsync(CancellationToken.None);
                var apply = ops.OfType<ApplyChangesOperation>().FirstOrDefault();
                if (apply == null) continue;
                var newDoc = apply.ChangedSolution.GetDocument(doc.Id);
                if (newDoc == null) { Console.WriteLine("      fix: document removed/renamed"); continue; }
                var oldText = (await doc.GetTextAsync()).ToString(); var newText = (await newDoc.GetTextAsync()).ToString();
                var (nd, ne, nx) = await Analyze(newDoc);
                var status = ne.Length > baseErrors ? "BAD-FIX(new compiler errors: " + string.Join("; ", ne.Select(e => e.Id + " " + Pos(e) + " " + e.GetMessage())) + ")" : "ok";
                var still = nd.Count(x => x.Id == d.Id);
                Console.WriteLine($"      fix[{f.GetType().Name}] {status}; {d.Id} count after={still}");
                foreach (var l in Diff(oldText, newText)) Console.WriteLine("        " + l);
            }
            catch (Exception ex) { Console.WriteLine($"      !!! FIX EXCEPTION {f.GetType().Name}: {ex.GetType().Name} {ex.Message}"); }
        }
    }
}

IEnumerable<string> Diff(string a, string b)
{
    var x = a.Split('\n'); var y = b.Split('\n');
    int s = 0; while (s < x.Length && s < y.Length && x[s] == y[s]) s++;
    int ex = x.Length - 1, ey = y.Length - 1; while (ex >= s && ey >= s && x[ex] == y[ey]) { ex--; ey--; }
    for (int i = s; i <= ex; i++) yield return "- " + x[i].Replace("\r", "\\r");
    for (int i = s; i <= ey; i++) yield return "+ " + y[i].Replace("\r", "\\r");
}

string Pos(Diagnostic d) { var p = d.Location.GetLineSpan().StartLinePosition; return $"{p.Line + 1}:{p.Character + 1}"; }

(Document, AdhocWorkspace) CreateDocument(string code, string file)
{
    var ws = new AdhocWorkspace();
    var pid = ProjectId.CreateNewId();
    var specific = allIds.ToImmutableDictionary(id => id, _ => ReportDiagnostic.Warn);
    var sol = ws.CurrentSolution.AddProject(ProjectInfo.Create(pid, VersionStamp.Default, "TestProject", "TestProject", LanguageNames.CSharp,
        compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, specificDiagnosticOptions: specific, nullableContextOptions: NullableContextOptions.Enable),
        parseOptions: new CSharpParseOptions(lang, DocumentationMode.Diagnose), metadataReferences: refs));
    var did = DocumentId.CreateNewId(pid);
    var name = Path.GetFileName(file);
    sol = sol.AddDocument(did, name, SourceText.From(code, System.Text.Encoding.UTF8), filePath: "/src/" + name);
    var jsonPath = Path.ChangeExtension(file, ".json");
    if (File.Exists(jsonPath)) sol = sol.AddAdditionalDocument(DocumentId.CreateNewId(pid), "stylecop.json", SourceText.From(File.ReadAllText(jsonPath)), filePath: "/src/stylecop.json");
    ws.TryApplyChanges(sol);
    return (ws.CurrentSolution.GetDocument(did), ws);
}

async Task<(ImmutableArray<Diagnostic>, Diagnostic[], List<string>)> Analyze(Document doc)
{
    var comp = await doc.Project.GetCompilationAsync();
    var exc = new List<string>();
    var opts = new CompilationWithAnalyzersOptions(doc.Project.AnalyzerOptions, (ex, an, d) => exc.Add(an.GetType().Name + ": " + ex.GetType().Name + " " + ex.Message + " @ " + ex.StackTrace?.Split('\n').FirstOrDefault()?.Trim()), concurrentAnalysis: false, logAnalyzerExecutionTime: false);
    var cwa = comp.WithAnalyzers(analyzers, opts);
    var diags = await cwa.GetAnalyzerDiagnosticsAsync();
    var errs = comp.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
    foreach (var d in diags.Where(d => d.Id == "AD0001")) exc.Add(d.GetMessage());
    return (diags.Where(d => d.Id != "AD0001").ToImmutableArray(), errs, exc);
}
