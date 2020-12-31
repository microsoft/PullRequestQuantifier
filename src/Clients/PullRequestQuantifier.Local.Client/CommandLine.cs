namespace PrQuantifier.Local.Client
***REMOVED***
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class CommandLine
    ***REMOVED***
        private readonly Stack<string> arguments = new Stack<string>();

        public CommandLine(string[] args)
        ***REMOVED***
            if (args.Length == 1 && (args[0] == "-?" || args[0] == "/?" || args[0] == "-h" || args[0] == "--help"))
            ***REMOVED***
                PrintUsage();
                return;
    ***REMOVED***

            for (var i = args.Length - 1; i >= 0; i--)
            ***REMOVED***
                arguments.Push(args[i]);
    ***REMOVED***

            ParseArgs();
***REMOVED***

        public string GitRepoPath ***REMOVED*** get; private set; ***REMOVED*** = Environment.CurrentDirectory;

        public string ContextPath ***REMOVED*** get; set; ***REMOVED***

        public bool Service ***REMOVED*** get; set; ***REMOVED***

        public bool PrintJson ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// If <see cref="QuantifierInputFile"/> is specified, this is given preference
        /// over <see cref="GitRepoPath"/>.
        /// </summary>
        public string QuantifierInputFile ***REMOVED*** get; set; ***REMOVED***

        private void ParseArgs()
        ***REMOVED***
            while (arguments.Count > 0)
            ***REMOVED***
                var option = PopOption(arguments);
                var optionName = option.Key.ToLowerInvariant();

                if (!optionName.StartsWith("-", StringComparison.Ordinal))
                ***REMOVED***
                    throw new ArgumentException($"unknown command line option: ***REMOVED***option.Key***REMOVED***");
        ***REMOVED***

                optionName = optionName.Substring(1);

                if (optionName == "gitrepopath")
                ***REMOVED***
                    if (option.Value == null)
                    ***REMOVED***
                        throw new ArgumentException("Missing argument for -gitrepopath");
            ***REMOVED***

                    GitRepoPath = option.Value;
        ***REMOVED***
                else if (optionName == "service")
                ***REMOVED***
                    Service = true;
        ***REMOVED***
                else if (optionName == "printjson")
                ***REMOVED***
                    PrintJson = true;
        ***REMOVED***
                else if (optionName == "contextpath")
                ***REMOVED***
                    if (option.Value != null && !File.Exists(option.Value))
                    ***REMOVED***
                        throw new ArgumentException($"Cannot find the specified context file: ***REMOVED***option.Value***REMOVED***");
            ***REMOVED***

                    ContextPath = option.Value;
        ***REMOVED***
                else if (optionName == "quantifierinput")
                ***REMOVED***
                    if (option.Value != null && !File.Exists(option.Value))
                    ***REMOVED***
                        throw new ArgumentException($"Cannot find the specified quantifier input file: ***REMOVED***option.Value***REMOVED***");
            ***REMOVED***

                    QuantifierInputFile = option.Value;
        ***REMOVED***
    ***REMOVED***
***REMOVED***

        private KeyValuePair<string, string> PopOption(Stack<string> arguments)
        ***REMOVED***
            var option = arguments.Pop();
            string value = null;

            if (arguments.Count > 0 && !arguments.Peek().StartsWith("-", StringComparison.Ordinal))
            ***REMOVED***
                value = arguments.Pop();
    ***REMOVED***

            return new KeyValuePair<string, string>(option, value);
***REMOVED***

        private void PrintUsage()
        ***REMOVED***
            var executableName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().GetName().FullName);

            Console.WriteLine("Copyright (C) .NET Foundation.");
            Console.WriteLine();
            Console.WriteLine($"usage: ./***REMOVED***executableName***REMOVED***.exe [options]");
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
***REMOVED***
***REMOVED***
***REMOVED***