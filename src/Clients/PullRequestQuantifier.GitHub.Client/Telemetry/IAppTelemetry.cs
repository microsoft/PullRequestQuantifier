namespace PullRequestQuantifier.GitHub.Client.Telemetry
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    public interface IAppTelemetry
    {
        string OperationId { get; }

        void RecordMetric((string name, long value) metric, params (string key, string value)[] dimensions);

        void RecordMetric(
            string name,
            long value,
            params (string key, string value)[] dimensions);

        OperationScope<T> StartOperation<T>(
            object caller,
            string parentOperationId = null,
            [CallerMemberName] string operationName = "")
            where T : OperationTelemetry, new();

        void TrackEvent(string eventName, IDictionary<string, string> properties);

        void TrackDependency(
            string dependencyTypeName,
            string targetName,
            string dependencyName,
            string data,
            DateTimeOffset startTime,
            TimeSpan duration,
            string resultCode,
            bool success);
    }
}
