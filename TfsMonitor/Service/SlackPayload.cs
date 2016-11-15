using System;
using System.Collections.Generic;
using TfsMonitor.Service.Interfaces;

namespace TfsMonitor.Service
{
    public class SlackPayload : IAlertBody
    {
        public string text { get; set; }
        public List<SlackAttachment> attachments { get; set; }

        public string username
        {
            get { return "Noah"; }
        }

        public string icon_emoji
        {
            get { return ":anchor:"; }
        }

        public static string ToSlackLink(string url, string text)
        {
            return String.Format("<{0}|{1}>", url, text);
        }
    }

    public class SlackAttachment
    {
        public string title { get; set; }
        public string text { get; set; }
        public string color { get; set; }
    }
}