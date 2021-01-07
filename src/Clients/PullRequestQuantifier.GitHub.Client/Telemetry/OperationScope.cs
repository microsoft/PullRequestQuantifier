namespace PullRequestQuantifier.GitHub.Client.Telemetry
***REMOVED***
    using System;
    using System.Diagnostics;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    /// <summary>
    /// A wrapper to app insights method TelemetryClient.StartOperation that will create
    /// either RequestTelemetry or DependencyTelemetry based on if current call is root
    /// This class is returned in using statement in order to emit metric when it's being disposed
    /// </summary>
    public sealed class OperationScope<T> : IDisposable
        where T : OperationTelemetry, new()
    ***REMOVED***
        private readonly TelemetryClient telemetry;

        public OperationScope(Activity activity, TelemetryClient telemetryClient = null)
        ***REMOVED***
            ArgumentCheck.ParameterIsNotNull(activity, nameof(activity));

            telemetry = telemetryClient;

            if (telemetry == null)
            ***REMOVED***
                return;
    ***REMOVED***

            OperationHolder = telemetry.StartOperation<T>(activity);
***REMOVED***

        public IOperationHolder<T> OperationHolder ***REMOVED*** get; set; ***REMOVED***

        /// <summary>
        /// A helper to create <see cref="IDisposable"/> from caller class
        /// Example:
        /// using (_metrics.StartOperation("methodName")) ***REMOVED******REMOVED***
        /// using (_metrics.StartOperation("methodName", parentOperationId)) ***REMOVED******REMOVED***
        ///
        /// if parentId is not passed in, it checks static instance AsyncLocal to find current activity,
        /// which is stored as local copy for each call (ExecutionContext) within a process.
        /// When parentId is not found, it assumes current call is the root
        /// </summary>
        public static OperationScope<TTelemetry> StartOperation<TTelemetry>(
            string parentOperationId,
            string operationName,
            TelemetryClient appInsights)
            where TTelemetry : OperationTelemetry, new()
        ***REMOVED***
            var activity = new Activity(operationName);

            if (!string.IsNullOrEmpty(parentOperationId))
            ***REMOVED***
                activity.SetParentId(parentOperationId);
    ***REMOVED***
            else
            ***REMOVED***
                activity.SetIdFormat(ActivityIdFormat.W3C);
    ***REMOVED***

            return new OperationScope<TTelemetry>(activity, appInsights);
***REMOVED***

        public void Dispose()
        ***REMOVED***
            Dispose(true);
            GC.SuppressFinalize(this);
***REMOVED***

        private void Dispose(bool disposing)
        ***REMOVED***
            if (disposing)
            ***REMOVED***
                if (OperationHolder != null)
                ***REMOVED***
                    telemetry?.StopOperation(OperationHolder);

                    OperationHolder.Dispose();
        ***REMOVED***
    ***REMOVED***

            OperationHolder = null;
***REMOVED***
***REMOVED***
***REMOVED***
