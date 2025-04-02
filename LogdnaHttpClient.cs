using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Http;

namespace Serilog.Sinks.LogDNA
{
    class LogdnaHttpClient : IHttpClient, IDisposable
    {
        private readonly HttpClient client;

        public LogdnaHttpClient(string apiKey)
        {
            client = new HttpClient();

            var authToken = Encoding.ASCII.GetBytes($"{apiKey}:");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream)
        {
            using (var content = new StreamContent(contentStream))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return await client.PostAsync(requestUri, content).ConfigureAwait(false);
            }
        }

        public void Configure(IConfiguration configuration) { }

        public void Dispose() => client?.Dispose();
    }
}