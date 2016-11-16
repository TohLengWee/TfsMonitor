using System.Collections.Generic;
using TfsMonitor.Service.Interfaces;

namespace TfsMonitor.Service
{
    public class SlackPayload : IAlertBody
    {
        private readonly ConfigHelper _configHelper = ConfigHelper.GetInstance("config.xml");

        public string text { get; set; }
        public List<SlackAttachment> attachments { get; set; }

        public string username => _configHelper.GetSingleConfig("config/master");

        public string icon_emoji => ":anchor:";

        public static string ToSlackLink(string url, string text)
        {
            return $"<{url}|{text}>";
        }
    }

    public class SlackAttachment
    {
        public string text { get; set; }
        public string color { get; set; }
        public string mrkdwn_in { get; set; }
    }
}