namespace PullRequestQuantifier.GitHub.Client.Telemetry
{
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
    {
        private readonly TelemetryClient telemetry;

        public OperationScope(Activity activity, TelemetryClient telemetryClient = null)
        {
            ArgumentCheck.ParameterIsNotNull(activity, nameof(activity));

            telemetry = telemetryClient;

            if (telemetry == null)
            {
                return;
            }

            OperationHolder = telemetry.StartOperation<T>(activity);
        }

        public IOperationHolder<T> OperationHolder { get; set; }

        /// <summary>
        /// A helper to create <see cref="IDisposable"/> from caller class
        /// Example:
        /// using (_metrics.StartOperation("methodName")) {}
        /// using (_metrics.StartOperation("methodName", parentOperationId)) {}
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
        {
            var activity = new Activity(operationName);

            if (!string.IsNullOrEmpty(parentOperationId))
            {
                activity.SetParentId(parentOperationId);
            }
            else
            {
                activity.SetIdFormat(ActivityIdFormat.W3C);
            }

            return new OperationScope<TTelemetry>(activity, appInsights);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (OperationHolder != null)
                {
                    telemetry?.StopOperation(OperationHolder);

                    OperationHolder.Dispose();
                }
            }

            OperationHolder = null;
        }
    }
}
