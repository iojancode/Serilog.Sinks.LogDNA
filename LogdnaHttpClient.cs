using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Serilog.Sinks.Http;

namespace Serilog.Sinks.LogDNA
{
    class LogdnaHttpClient : IHttpClient
    {
        private readonly HttpClient client;

        public LogdnaHttpClient(string apiKey)
        {
            client = new HttpClient();

            var authToken = Encoding.ASCII.GetBytes($"{apiKey}:");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content) => client.PostAsync(requestUri, content);

        public void Dispose() => client?.Dispose();
    }
}