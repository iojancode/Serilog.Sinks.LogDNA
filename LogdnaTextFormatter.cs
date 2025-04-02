using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Parsing;

namespace Serilog.Sinks.LogDNA
{
    class LogdnaTextFormatter : ITextFormatter
    {
        private static readonly JsonValueFormatter Instance = new JsonValueFormatter();
        private string _app;
        private string _env;

        public LogdnaTextFormatter(string app, string env)
        {
            _app = app;
            _env = env;
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            try
            {
                var buffer = new StringWriter();
                FormatContent(logEvent, buffer);
                output.WriteLine(buffer.ToString());
            }
            catch (Exception e)
            {
                LogNonFormattableEvent(logEvent, e);
            }
        }

        private void FormatContent(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));

            output.Write("{\"timestamp\":\"");
            output.Write(logEvent.Timestamp.ToString("o"));

            output.Write("\",\"level\":\"");
            output.Write(logEvent.Level);

            output.Write("\",\"app\":\"");
            output.Write(_app);

            if (_env != null) 
            {
                output.Write("\",\"env\":\"");
                output.Write(_env);                
            }

            output.Write("\",\"line\":");
            var message = logEvent.MessageTemplate.Render(logEvent.Properties);
            JsonValueFormatter.WriteQuotedJsonString(message, output);

            WriteMetaObject(logEvent, output);

            output.Write('}');
        }

        private static void WriteMetaObject(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Properties.Count == 0 && logEvent.Exception == null) return;

            output.Write(",\"meta\":{");

            var precedingDelimiter = "";

            if (logEvent.Exception != null)
            {
                output.Write("\"Exception\":");
                JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
                precedingDelimiter = ",";
            }

            foreach (var property in logEvent.Properties)
            {
                output.Write(precedingDelimiter);
                precedingDelimiter = ",";

                JsonValueFormatter.WriteQuotedJsonString(property.Key, output);
                output.Write(':');
                Instance.Format(property.Value, output);
            }

            output.Write('}');
        }

        private static void LogNonFormattableEvent(LogEvent logEvent, Exception e)
        {
            SelfLog.WriteLine(
                "Event at {0} with message template {1} could not be formatted into JSON and will be dropped: {2}",
                logEvent.Timestamp.ToString("o"),
                logEvent.MessageTemplate.Text,
                e);
        }
    }
}