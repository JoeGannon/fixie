﻿namespace Fixie.Execution
{
    using System.IO;
    using Cli;

    public class Options
    {
        public Options(string assemblyPath)
        {
            AssemblyPath = assemblyPath;
        }

        public string AssemblyPath { get; }
        public string Report { get; set; }
        public bool? TeamCity { get; set; }

        public void Validate()
        {
            if (AssemblyPath == null)
                throw new CommandLineException("Missing required test assembly path.");

            if (!File.Exists(AssemblyPath))
                throw new CommandLineException("Specified test assembly does not exist: " + AssemblyPath);

            if (!AssemblyDirectoryContainsFixie(AssemblyPath))
                throw new CommandLineException($"Specified assembly {AssemblyPath} does not appear to be a test assembly. Ensure that it references Fixie.dll and try again.");

            if (Report != null && Report.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                throw new CommandLineException("Specified report name is invalid: " + Report);
        }

        static bool AssemblyDirectoryContainsFixie(string assemblyPath)
            => File.Exists(Path.Combine(Path.GetDirectoryName(assemblyPath), "Fixie.dll"));
    }
}