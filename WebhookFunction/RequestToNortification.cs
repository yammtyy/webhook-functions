using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace WebhookFunction
{
    public static class RequestToNortification
    {
        [FunctionName("RequestToNortification")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            WebRequest request = WebRequest.Create("https://notificationdemo-dev.azurewebsites.net");
            request.Method = "POST";
            request.ContentType = "application/json";

            //送信するデータを書き込む
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string data = JsonConvert.SerializeObject(requestBody);
            log.LogInformation($"{data}");
            var streamWriter = new StreamWriter(request.GetRequestStream());
            streamWriter.Write(data);

            //サーバーからの応答を受信するためのWebResponseを取得
            WebResponse response = request.GetResponse();
            //応答データを受信するためのStreamを取得
            Stream resStream = response.GetResponseStream();
            //受信して表示
            StreamReader reader = new StreamReader(resStream, Encoding.GetEncoding("UTF-8"));

            log.LogInformation($"{reader}");
            log.LogInformation($"{((HttpWebResponse)response).StatusDescription}");
            log.LogInformation($"{resStream}");
            log.LogInformation($"{response}");

            string result = ((HttpWebResponse)response).StatusDescription;

            return new OkObjectResult(result);
        }
    }
}
