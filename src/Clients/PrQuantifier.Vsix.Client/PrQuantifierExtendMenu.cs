namespace PrQuantifier.Vsix.Client
***REMOVED***
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.IO;
    using EnvDTE;
    using Microsoft;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Threading;
    using Newtonsoft.Json.Linq;
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
            Projects projects = dte.Solution.Projects;
            if (projects.Count == 0)
            ***REMOVED***
                return;
    ***REMOVED***

            Project project = projects.Item(1);
            var uri = new Uri(typeof(PrQuantifierExtendMenuPackage).Assembly.CodeBase, UriKind.Absolute);

            documentEvents = dte.Events.DocumentEvents;
            documentEvents.DocumentSaved += DocumentEvents_DocumentSaved;

            while (true)
            ***REMOVED***
                if (runPrQuantifierDateTime == changedEventDateTime
                    && runPrQuantifierDateTime != default)
                ***REMOVED***
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                    continue;
        ***REMOVED***

                using var process = new System.Diagnostics.Process()
                ***REMOVED***
                    StartInfo = new ProcessStartInfo
                    ***REMOVED***
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        FileName = Path.Combine(Path.GetDirectoryName(uri.LocalPath), @"PrQuantifier\PrQuantifier.Local.Client.exe"),
                        Arguments = $"GitRepoPath=***REMOVED***project.FullName***REMOVED*** PrintJson=true"
            ***REMOVED***
        ***REMOVED***;

                process.Start();
                process.WaitForExit((int)TimeSpan.FromSeconds(2).TotalMilliseconds);

                string line = await process.StandardOutput.ReadLineAsync();
                Print(statusBar, line);
                runPrQuantifierDateTime = changedEventDateTime;
    ***REMOVED***
***REMOVED***

        private void DocumentEvents_DocumentSaved(Document document)
        ***REMOVED***
           changedEventDateTime = DateTimeOffset.Now;
***REMOVED***

        private void Print(
            IVsStatusbar statusBar,
            string quantifierResultJson)
        ***REMOVED***
            ThreadHelper.ThrowIfNotOnUIThread();

            JObject quantifierResult = JObject.Parse(quantifierResultJson);

            string output = $"PrQuantified = ***REMOVED***quantifierResult["Label"]***REMOVED***\t" +
                    $"Diff +***REMOVED***quantifierResult["QuantifiedLinesAdded"]***REMOVED*** -***REMOVED***quantifierResult["QuantifiedLinesDeleted"]***REMOVED*** (Formula = ***REMOVED***quantifierResult["Formula"]***REMOVED***)" +
                    $"\tTeam percentiles: additions = ***REMOVED***quantifierResult["PercentileAddition"]***REMOVED***%" +
                    $", deletions = ***REMOVED***quantifierResult["PercentileDeletion"]***REMOVED***%.";

            statusBar.SetText(output);
***REMOVED***
***REMOVED***
***REMOVED***
