// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Tools.Fuzz;

using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

internal static class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static async Task<int> Main(string[] args)
    {
        if (args.Length == 1 && args[0] == "worker")
        {
            var engine = new AnalysisEngine();
            while (await Console.In.ReadLineAsync().ConfigureAwait(false) is { } line)
            {
                var input = JsonSerializer.Deserialize<Input>(line) ?? throw new InvalidOperationException("Invalid worker input.");
                Result result;
                try
                {
                    result = await engine.RunAsync(input).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    result = new Result(false, 0, 0, [new Finding("HostException", "CompilerOrHarness", string.Empty, string.Empty, exception.ToString())]);
                }

                Console.WriteLine(JsonSerializer.Serialize(result));
            }

            return 0;
        }

        if (args.Length == 1 && args[0] == "self-test")
        {
            await SelfTests.RunAsync().ConfigureAwait(false);
            return 0;
        }

        if (args.Length == 3 && args[0] == "extract")
        {
            Console.WriteLine($"Extracted {Corpus.Extract(args[1], args[2])} distinct literal seeds.");
            return 0;
        }

        if (args.Length < 3 || args[0] is not ("repro" or "mutate"))
        {
            Console.WriteLine("""
                Experimental deterministic fuzzing (not coverage-guided).
                  extract <test-directory> <corpus-directory>
                  repro <input-file-or-directory> <output-directory>
                  mutate <input-file-or-directory> <output-directory> [iterations=100] [random-seed=4338] [timeout-seconds=30]
                  self-test
                Repro also accepts saved input.json files, preserving filename and stylecop.json.
                A .cs seed may have a sibling .json containing stylecop.json.
                Every seed is replayed before mutations; iterations is the number of additional mutations.
                Findings are candidates for manual triage, not automatically filed issues.
                """);
            return args.Length == 0 ? 0 : 2;
        }

        var mutations = args[0] == "mutate" ? args.Length > 3 ? int.Parse(args[3], System.Globalization.CultureInfo.InvariantCulture) : 100 : 0;
        var seed = args.Length > 4 ? int.Parse(args[4], System.Globalization.CultureInfo.InvariantCulture) : 4338;
        var timeout = TimeSpan.FromSeconds(args.Length > 5 ? int.Parse(args[5], System.Globalization.CultureInfo.InvariantCulture) : 30);
        if (mutations < 0 || timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(args), "Iterations must be nonnegative and timeout must be positive.");
        }

        var inputs = Corpus.Load(args[1]);
        if (inputs.Count == 0)
        {
            throw new InvalidOperationException("No corpus inputs found.");
        }

        Directory.CreateDirectory(args[2]);
        var random = new Random(seed);
        var cases = inputs.Concat(Enumerable.Range(0, mutations).Select(_ => Corpus.Mutate(inputs[random.Next(inputs.Count)], random)));
        using var worker = new Worker();
        var stopwatch = Stopwatch.StartNew();
        var count = 0;
        var valid = 0;
        var actions = 0;
        var findings = new Dictionary<string, Finding>();
        foreach (var input in cases)
        {
            var result = await worker.RunAsync(input, timeout).ConfigureAwait(false);
            count++;
            valid += result.Compiles ? 1 : 0;
            actions += result.Actions;
            foreach (var finding in result.Findings)
            {
                var signature = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(
                    string.Join("\n", finding.Kind, finding.Component, finding.Rule, finding.Action, FirstDetail(finding)))))[..16];
                var directory = Path.Combine(args[2], signature);
                var inputPath = Path.Combine(directory, "input.json");
                if (!File.Exists(inputPath) || input.Source.Length < (JsonSerializer.Deserialize<Input>(await File.ReadAllTextAsync(inputPath).ConfigureAwait(false))?.Source.Length ?? int.MaxValue))
                {
                    Directory.CreateDirectory(directory);
                    await File.WriteAllTextAsync(inputPath, JsonSerializer.Serialize(input, JsonOptions)).ConfigureAwait(false);
                    await File.WriteAllTextAsync(Path.Combine(directory, "input.cs"), input.Source).ConfigureAwait(false);
                    await File.WriteAllTextAsync(Path.Combine(directory, "finding.json"), JsonSerializer.Serialize(finding, JsonOptions)).ConfigureAwait(false);
                    if (finding.FixedSource != null)
                    {
                        await File.WriteAllTextAsync(Path.Combine(directory, "fixed.cs"), finding.FixedSource).ConfigureAwait(false);
                    }
                }

                if (findings.TryAdd(signature, finding))
                {
                    Console.WriteLine($"{signature}: {finding.Kind} {finding.Rule} {finding.Component} {finding.Action}");
                }
            }

            if (count % 25 == 0)
            {
                Console.WriteLine($"Inputs={count}, compiling={valid}, actions={actions}, signatures={findings.Count}, elapsed={stopwatch.Elapsed}");
            }
        }

        var summary = new
        {
            Version = AnalysisEngine.Version,
            Roslyn = typeof(Microsoft.CodeAnalysis.Compilation).Assembly.GetName().Version?.ToString(),
            Runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
            Seed = seed,
            Inputs = count,
            Compiling = valid,
            Actions = actions,
            Signatures = findings.Count,
            Elapsed = stopwatch.Elapsed.ToString(),
            Kinds = findings.Values.GroupBy(finding => finding.Kind).ToDictionary(group => group.Key, group => group.Count()),
        };
        var json = JsonSerializer.Serialize(summary, JsonOptions);
        await File.WriteAllTextAsync(Path.Combine(args[2], "summary.json"), json).ConfigureAwait(false);
        Console.WriteLine(json);
        return findings.Count == 0 ? 0 : 1;
    }

    private static string FirstDetail(Finding finding) => finding.Details.Split('\n').FirstOrDefault() ?? string.Empty;

    private sealed class Worker : IDisposable
    {
        private Process? process;
        private string stage = string.Empty;

        public void Dispose()
        {
            if (this.process != null)
            {
                if (!this.process.HasExited)
                {
                    this.process.Kill(entireProcessTree: true);
                    this.process.WaitForExit();
                }

                this.process.Dispose();
                this.process = null;
            }
        }

        internal async Task<Result> RunAsync(Input input, TimeSpan timeout)
        {
            if (this.process == null)
            {
                this.Start();
            }

            var process = this.process!;
            this.stage = "Worker startup";
            try
            {
                await process.StandardInput.WriteLineAsync(JsonSerializer.Serialize(input)).ConfigureAwait(false);
                var line = await process.StandardOutput.ReadLineAsync().WaitAsync(timeout).ConfigureAwait(false);
                if (line != null)
                {
                    return JsonSerializer.Deserialize<Result>(line) ?? throw new InvalidOperationException("Invalid worker output.");
                }

                await process.WaitForExitAsync().ConfigureAwait(false);
                var result = new Result(false, 0, 0, [new Finding("Crash", "Worker", string.Empty, this.stage, $"Exit code {process.ExitCode}")]);
                this.Dispose();
                return result;
            }
            catch (TimeoutException)
            {
                var result = new Result(false, 0, 0, [new Finding("Timeout", "Worker", string.Empty, this.stage, $"Exceeded {timeout.TotalSeconds} seconds")]);
                this.Dispose();
                return result;
            }
        }

        private void Start()
        {
            var start = new ProcessStartInfo(Environment.ProcessPath ?? throw new InvalidOperationException("Missing process path."))
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            if (Path.GetFileNameWithoutExtension(start.FileName) == "dotnet")
            {
                start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location);
            }

            start.ArgumentList.Add("worker");
            this.process = Process.Start(start) ?? throw new InvalidOperationException("Failed to start worker.");
            this.process.ErrorDataReceived += (_, args) =>
            {
                if (args.Data != null)
                {
                    this.stage = args.Data;
                }
            };
            this.process.BeginErrorReadLine();
        }
    }
}
