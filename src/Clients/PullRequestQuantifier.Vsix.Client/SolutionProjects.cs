namespace PullRequestQuantifier.Vsix.Client
***REMOVED***
    using System.Collections.Generic;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;

    internal static class SolutionProjects
    ***REMOVED***
        internal static IEnumerable<Project> Projects()
        ***REMOVED***
            ThreadHelper.ThrowIfNotOnUIThread();
            Projects projects = GetActiveIDE().Solution.Projects;
            List<Project> list = new List<Project>();
            var item = projects.GetEnumerator();
            while (item.MoveNext())
            ***REMOVED***
                var project = item.Current as Project;
                if (project == null)
                ***REMOVED***
                    continue;
        ***REMOVED***

                if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                ***REMOVED***
                    list.AddRange(GetSolutionFolderProjects(project));
        ***REMOVED***
                else
                ***REMOVED***
                    list.Add(project);
        ***REMOVED***
    ***REMOVED***

            return list;
***REMOVED***

        private static DTE2 GetActiveIDE()
        ***REMOVED***
            // Get an instance of currently running Visual Studio IDE.
            DTE2 dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            return dte2;
***REMOVED***

        private static IEnumerable<Project> GetSolutionFolderProjects(Project solutionFolder)
        ***REMOVED***
            ThreadHelper.ThrowIfNotOnUIThread();
            List<Project> list = new List<Project>();
            for (var i = 1; i <= solutionFolder.ProjectItems.Count; i++)
            ***REMOVED***
                var subProject = solutionFolder.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                ***REMOVED***
                    continue;
        ***REMOVED***

                // If this is another solution folder, do a recursive call, otherwise add
                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                ***REMOVED***
                    list.AddRange(GetSolutionFolderProjects(subProject));
        ***REMOVED***
                else
                ***REMOVED***
                    list.Add(subProject);
        ***REMOVED***
    ***REMOVED***

            return list;
***REMOVED***
***REMOVED***
***REMOVED***
