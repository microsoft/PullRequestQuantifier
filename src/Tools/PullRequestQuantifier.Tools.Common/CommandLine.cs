namespace PullRequestQuantifier.Tools.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using PullRequestQuantifier.Common;

    [ExcludeFromCodeCoverage]
    public class CommandLine
    {
        private readonly Stack<string> arguments = new Stack<string>();

        public CommandLine(string[] args)
        {
            ArgumentCheck.ParameterIsNotNull(args, nameof(args));

            if (args.Length == 1
                && (args[0] == "-?"
                    || args[0] == "/?"
                    || args[0] == "-h"
                    || args[0] == "--help"))
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

        public string ClonePath { get; private set; } = Environment.CurrentDirectory;

        public string ConfigFile { get; set; }

        public string User { get; set; }

        public string Pat { get; set; }

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

                if (optionName == "clonepath")
                {
                    ClonePath = option.Value ?? throw new ArgumentException("Missing argument for -clonepath");
                }
                else if (optionName == "configfile")
                {
                    if (option.Value != null && !File.Exists(option.Value))
                    {
                        throw new ArgumentException($"Cannot find the specified file: {option.Value}");
                    }

                    ConfigFile = option.Value;
                }
                else if (optionName == "user")
                {
                    User = option.Value ?? throw new ArgumentException("Missing argument for -user");
                }
                else if (optionName == "pat")
                {
                    Pat = option.Value ?? throw new ArgumentException("Missing argument for -pat");
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
            Console.WriteLine("  -clonepath             : path to where to clone the repositories");
            Console.WriteLine("  -configfile            : path to the what to clone input config file");
            Console.WriteLine("  -user                  : user name for ADO connection");
            Console.WriteLine("  -pat            : pat for ADO connection");
            Console.WriteLine();
        }
    }
}