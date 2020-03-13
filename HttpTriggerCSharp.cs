using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CertTest.Function
{
    public static class HttpTriggerCSharp
    {
        private static readonly string ClientCertKeyString = "X-ARR-ClientCert";

        [FunctionName("HttpTriggerCSharp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            log.LogDebug(JsonConvert.SerializeObject(req.Headers));
            if (req.Headers.ContainsKey(ClientCertKeyString))
            {
                byte[] clientCertBytes = Convert.FromBase64String(req.Headers[ClientCertKeyString].FirstOrDefault());
                var clientCert = new X509Certificate2(clientCertBytes);
                var thumbprint = $"Thumbprint: {clientCert.Thumbprint}";
                log.LogDebug(thumbprint);
                return new OkObjectResult(thumbprint);
            }
            else
            {
                log.LogDebug("Client certificate is not found");
                return new ForbidResult();
            }
        }
    }
}
