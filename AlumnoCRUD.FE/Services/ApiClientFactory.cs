using System;
using System.Net.Http;

namespace AlumnoCRUD.FE.Services
{
    public static class ApiClientFactory
    {
        private static readonly string BaseUrl = "http://localhost:5261/";

        public static HttpClient CreateClient()
        {
            var handler = new HttpClientHandler();
            // Ignorar errores de certificado SSL en desarrollo
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };

            return client;
        }
    }
}
