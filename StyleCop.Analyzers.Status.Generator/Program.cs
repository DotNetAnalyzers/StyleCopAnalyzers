namespace StyleCop.Analyzers.Status.Generator
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// The starting point of this application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The starting point of this application.
        /// </summary>
        /// <param name="args">The command line parameters.</param>
        internal static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Path to sln file required.");
                return;
            }

            SolutionReader reader = SolutionReader.CreateAsync(args[0]).Result;

            Console.WriteLine(JsonConvert.SerializeObject(reader.GetDiagnosticsAsync().Result));
        }
    }
}
