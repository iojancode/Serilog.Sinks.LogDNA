using System;
using System.Net;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.LogDNA;

namespace Serilog
{
    public static class LogdnaExtensions
    {
        public static LoggerConfiguration LogDNA(
            this LoggerSinkConfiguration sinkConfiguration,
            string apiKey,
            string appName = null,
            string commaSeparatedTags = null,
            string ingestUrl = "https://logs.mezmo.com/logs/ingest",
            string hostname = null,
            long? queueLimitBytes = null,
            int? logEventsInBatchLimit = null,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            if (string.IsNullOrWhiteSpace(ingestUrl)) throw new ArgumentNullException(nameof(ingestUrl));

            ingestUrl += (ingestUrl.Contains("?") ? "&" : "?") + $"hostname={hostname ?? Dns.GetHostName().ToLower()}";
            if (commaSeparatedTags != null) ingestUrl += $"&tags={WebUtility.UrlEncode(commaSeparatedTags)}";

            var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT");

            return sinkConfiguration.Http(
                requestUri: ingestUrl,
                queueLimitBytes: queueLimitBytes,
                logEventsInBatchLimit: logEventsInBatchLimit ?? 100,
                batchSizeLimitBytes: 5000000, // max 10MB per request, extra characters for json not included 
                period: period ?? TimeSpan.FromSeconds(15),
                textFormatter: new LogdnaTextFormatter(appName ?? "unknown", envName),
                batchFormatter: new LogdnaBatchFormatter(),
                restrictedToMinimumLevel: restrictedToMinimumLevel,
                httpClient: new LogdnaHttpClient(apiKey));
        }

        public static LoggerConfiguration DurableLogDNA(
            this LoggerSinkConfiguration sinkConfiguration,
            string apiKey,
            string appName = null,
            string commaSeparatedTags = null,
            string ingestUrl = "https://logs.mezmo.com/logs/ingest",
            string hostname = null,
            long? bufferFileSizeLimitBytes = null,
            int? logEventsInBatchLimit = null,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            if (string.IsNullOrWhiteSpace(ingestUrl)) throw new ArgumentNullException(nameof(ingestUrl));

            ingestUrl += (ingestUrl.Contains("?") ? "&" : "?") + $"hostname={hostname ?? Dns.GetHostName().ToLower()}";
            if (commaSeparatedTags != null) ingestUrl += $"&tags={WebUtility.UrlEncode(commaSeparatedTags)}";

            var envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT");

            return sinkConfiguration.DurableHttpUsingFileSizeRolledBuffers(
                requestUri: ingestUrl,
                bufferBaseFileName: "logdna-buffer",
                bufferFileSizeLimitBytes: bufferFileSizeLimitBytes,
                logEventsInBatchLimit: logEventsInBatchLimit ?? 100,
                batchSizeLimitBytes: 5000000, // max 10MB per request, extra characters for json not included 
                period: period ?? TimeSpan.FromSeconds(15),
                textFormatter: new LogdnaTextFormatter(appName ?? "unknown", envName),
                batchFormatter: new LogdnaBatchFormatter(),
                restrictedToMinimumLevel: restrictedToMinimumLevel,
                httpClient: new LogdnaHttpClient(apiKey));
        }
    }
}
