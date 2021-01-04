namespace PullRequestQuantifier.Local.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class CommandLine
    {
        private readonly Stack<string> arguments = new Stack<string>();

        public CommandLine(string[] args)
        {
            if (args.Length == 1 && (args[0] == "-?" || args[0] == "/?" || args[0] == "-h" || args[0] == "--help"))
            {
                PrintUsage();
                return;
            }

            for (var i = args.Length - 1; i >= 0; i--)
            {
                arguments.Push(args[i]);
            }

            ParseArgs();
        }

        public string GitRepoPath { get; private set; } = Environment.CurrentDirectory;

        public string ContextPath { get; set; }

        public bool Service { get; set; }

        public bool PrintJson { get; set; }

        /// <summary>
        /// Gets or sets if <see cref="QuantifierInputFile"/> is specified, this is given preference
        /// over <see cref="GitRepoPath"/>.
        /// </summary>
        public string QuantifierInputFile { get; set; }

        private void ParseArgs()
        {
            while (arguments.Count > 0)
            {
                var option = PopOption(arguments);
                var optionName = option.Key.ToLowerInvariant();

                if (!optionName.StartsWith("-", StringComparison.Ordinal))
                {
                    throw new ArgumentException($"unknown command line option: {option.Key}");
                }

                optionName = optionName.Substring(1);

                if (optionName == "gitrepopath")
                {
                    GitRepoPath = option.Value ?? throw new ArgumentException("Missing argument for -gitrepopath");
                }
                else if (optionName == "service")
                {
                    Service = true;
                }
                else if (optionName == "printjson")
                {
                    PrintJson = true;
                }
                else if (optionName == "contextpath")
                {
                    if (option.Value != null && !File.Exists(option.Value))
                    {
                        throw new ArgumentException($"Cannot find the specified context file: {option.Value}");
                    }

                    ContextPath = option.Value;
                }
                else if (optionName == "quantifierinput")
                {
                    if (option.Value != null && !File.Exists(option.Value))
                    {
                        throw new ArgumentException($"Cannot find the specified quantifier input file: {option.Value}");
                    }

                    QuantifierInputFile = option.Value;
                }
            }
        }

        private KeyValuePair<string, string> PopOption(Stack<string> arguments)
        {
            var option = arguments.Pop();
            string value = null;

            if (arguments.Count > 0 && !arguments.Peek().StartsWith("-", StringComparison.Ordinal))
            {
                value = arguments.Pop();
            }

            return new KeyValuePair<string, string>(option, value);
        }

        private void PrintUsage()
        {
            var executableName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().GetName().FullName);

            Console.WriteLine("Copyright (C) .NET Foundation.");
            Console.WriteLine();
            Console.WriteLine($"usage: ./{executableName}.exe [options]");
            Console.WriteLine();
            Console.WriteLine("Options");
            Console.WriteLine();
            Console.WriteLine("  -gitrepopath           : path to the git repo on local file system");
            Console.WriteLine("  -quantifierinput       : path to the quantifier input file");
            Console.WriteLine("                          : useful if the git repo is not present on local file system");
            Console.WriteLine("                          : mutually exclusive with the \"-gitrepopath\" option");
            Console.WriteLine("                          : \"-gitrepopath\" is ignored if this is specified");
            Console.WriteLine("  -service               : run as a file watcher service");
            Console.WriteLine("                          : only valid when quantifying a local git repo");
            Console.WriteLine("  -printjson             : print detailed result as a JSON");
            Console.WriteLine("  -contextpath           : pull request quantifier context file");
            Console.WriteLine("                          : if not specified");
            Console.WriteLine("                          :   when \"-gitrepopath\" is specified)");
            Console.WriteLine("                          :     \".prquantifier\" is searched for in the repo root");
            Console.WriteLine("                          :   else uses a default context");
            Console.WriteLine();
        }
    }
}