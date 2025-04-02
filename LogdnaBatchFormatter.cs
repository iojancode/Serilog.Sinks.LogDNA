using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Sinks.Http;

namespace Serilog.Sinks.LogDNA
{
    class LogdnaBatchFormatter : IBatchFormatter
    {
        public void Format(IEnumerable<string> logEvents, TextWriter output)
        {
            if (logEvents == null) throw new ArgumentNullException(nameof(logEvents));
            if (output == null) throw new ArgumentNullException(nameof(output));

            // Abort if sequence of log events is empty
            if (!logEvents.Any())
            {
                return;
            }

            output.Write("{\"lines\":[");

            var delimStart = string.Empty;

            foreach (var logEvent in logEvents)
            {
                if (string.IsNullOrWhiteSpace(logEvent))
                {
                    continue;
                }

                output.Write(delimStart);
                output.Write(logEvent);
                delimStart = ",";
            }

            output.Write("]}");            
        }
    }
}