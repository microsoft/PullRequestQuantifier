namespace PrQuantifier.Vsix.Client
***REMOVED***
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using EnvDTE;
    using Microsoft;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using PullRequestQuantifier.Vsix.Client;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// Command handler.
    /// </summary>
    internal sealed class PrQuantifierExtendMenu
    ***REMOVED***
        private static DateTimeOffset changedEventDateTime = default;
        private static DateTimeOffset runPrQuantifierDateTime = default;
        private static DocumentEvents documentEvents;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrQuantifierExtendMenu"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file).
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private PrQuantifierExtendMenu(AsyncPackage package, OleMenuCommandService commandService)
        ***REMOVED***
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            ***REMOVED***
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                await ExecuteAsync();
    ***REMOVED***);
***REMOVED***

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static PrQuantifierExtendMenu Instance
        ***REMOVED***
            get;
            private set;
***REMOVED***

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        ***REMOVED***
            get
            ***REMOVED***
                return this.package;
    ***REMOVED***
***REMOVED***

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task InitializeAsync(AsyncPackage package)
        ***REMOVED***
            // Switch to the main thread - the call to AddCommand in PrQuantifierExtendMenu's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new PrQuantifierExtendMenu(package, commandService);
***REMOVED***

        private async Task ExecuteAsync()
        ***REMOVED***
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IVsStatusbar statusBar = (IVsStatusbar)await ServiceProvider.GetServiceAsync(typeof(SVsStatusbar));
            Assumes.Present(statusBar);

            DTE dte = (DTE)await ServiceProvider.GetServiceAsync(typeof(DTE));
            Assumes.Present(dte);
            var uri = new Uri(typeof(PrQuantifierExtendMenuPackage).Assembly.CodeBase, UriKind.Absolute);

            documentEvents = dte.Events.DocumentEvents;
            documentEvents.DocumentSaved += DocumentEvents_DocumentSaved;

            var projects = SolutionProjects.Projects();

            while (true)
            ***REMOVED***
                if ((runPrQuantifierDateTime == changedEventDateTime
                    && runPrQuantifierDateTime != default)
                    || projects.Count() == 0)
                ***REMOVED***
                    await Task.Delay(TimeSpan.FromMilliseconds(500));

                    // refresh projects list in case is still empty
                    if (projects.Count() == 0)
                    ***REMOVED***
                        projects = SolutionProjects.Projects();
            ***REMOVED***

                    continue;
        ***REMOVED***

                try
                ***REMOVED***
                    using var process = new System.Diagnostics.Process()
                    ***REMOVED***
                        StartInfo = new ProcessStartInfo
                        ***REMOVED***
                            RedirectStandardOutput = true,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            FileName = Path.Combine(Path.GetDirectoryName(uri.LocalPath), @"PrQuantifier\PullRequestQuantifier.Local.Client.exe"),
                            Arguments = $"-GitRepoPath \"***REMOVED***projects.ElementAt(0).FullName***REMOVED***\" -PrintJson true"
                ***REMOVED***
            ***REMOVED***;

                    process.Start();
                    Print(statusBar, await ReadOutputAsync(process));

                    if (changedEventDateTime == default)
                    ***REMOVED***
                        changedEventDateTime = DateTimeOffset.Now;
            ***REMOVED***

                    runPrQuantifierDateTime = changedEventDateTime;
        ***REMOVED***
                catch (Exception ex)
                ***REMOVED***
                     statusBar.SetText($"PrQuantifier extension has encountered an error, please report it to ... Exception ***REMOVED***ex.Message***REMOVED***");
                     return;
        ***REMOVED***
    ***REMOVED***
***REMOVED***

        private void DocumentEvents_DocumentSaved(Document document)
        ***REMOVED***
           changedEventDateTime = DateTimeOffset.Now;
***REMOVED***

        private async Task<JObject> ReadOutputAsync(System.Diagnostics.Process process)
        ***REMOVED***
            var result = string.Empty;
            while (string.IsNullOrEmpty(result))
            ***REMOVED***
                result = await process.StandardOutput.ReadToEndAsync();

                if (result.IndexOf(
                    "GitRepoPath couldn't be found",
                    StringComparison.InvariantCultureIgnoreCase) > -1)
                ***REMOVED***
                    throw new ArgumentException("GitRepoPath couldn't be found");
        ***REMOVED***

                try
                ***REMOVED***
                     var jObject = JObject.Parse(result);
                     return jObject;
        ***REMOVED***
                catch
                ***REMOVED***
                    result = string.Empty;
        ***REMOVED***
    ***REMOVED***

            throw new JsonReaderException();
***REMOVED***

        private void Print(
            IVsStatusbar statusBar,
            JObject quantifierResult)
        ***REMOVED***
            ThreadHelper.ThrowIfNotOnUIThread();

            string output = $"PrQuantified = ***REMOVED***quantifierResult["Label"]***REMOVED***,\t" +
                    $"Diff +***REMOVED***quantifierResult["QuantifiedLinesAdded"]***REMOVED*** -***REMOVED***quantifierResult["QuantifiedLinesDeleted"]***REMOVED*** (Formula = ***REMOVED***quantifierResult["Formula"]***REMOVED***)," +
                    $"\tTeam percentiles: additions = ***REMOVED***quantifierResult["PercentileAddition"]***REMOVED***%" +
                    $", deletions = ***REMOVED***quantifierResult["PercentileDeletion"]***REMOVED***%.";

            statusBar.SetText(output);
***REMOVED***
***REMOVED***
***REMOVED***
