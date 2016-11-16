using System;
using System.Net;
using System.Text;
using System.Web.Helpers;
using TfsMonitor.Service;
using TfsMonitor.Service.Interfaces;

namespace TfsMonitor
{
    class SlackClient : INotificationService
    {
        public SlackClient()
        {
            var configHelper = ConfigHelper.GetInstance("config.xml");
            SendAlert(new SlackPayload
            {
                text = "Hello "+configHelper.GetSingleConfig("config/master")+", Good day, I am awake."
            });
        }

        public bool SendAlert(IAlertBody alertBody)
        {
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Encoding = Encoding.UTF8;

            var jsonPayload = Json.Encode(alertBody);
            var dataBytes = Encoding.UTF8.GetBytes(jsonPayload);
            byte[] responseBytes;

            try
            {
                var configHelper = ConfigHelper.GetInstance("config.xml");
                var webhookUrl = configHelper.GetSingleConfig("config/slack/webhookurl");
                responseBytes = client.UploadData(webhookUrl, dataBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}{1}", ex.Message, ex.StackTrace);
                throw;
            }

            var response = Encoding.UTF8.GetString(responseBytes);
            return response.ToLower() == "ok";
        }
    }
}