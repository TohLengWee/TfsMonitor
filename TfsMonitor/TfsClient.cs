using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using TfsMonitor.Service;
using TfsMonitor.Service.Interfaces;

namespace TfsMonitor
{
    public class TfsClient
    {
        private static TfsClient _tfsClient;
        private readonly VersionControlServer _vcs;
        private DateTime _latestChangesetTime;
        public Dictionary<string, string> MonitorProjects { get; private set; }

        private TfsClient(string url)
        {
            var tfsConnection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(url));
            _vcs = tfsConnection.GetService<VersionControlServer>();
            _latestChangesetTime = DateTime.Now;
            MonitorProjects = new Dictionary<string, string>();
        }

        public static TfsClient GetInstance(string url)
        {
            return _tfsClient ?? (_tfsClient = new TfsClient(url));
        }

        public void LoadMonitorProjects(IEnumerable<XElement> projects)
        {
            foreach (var project in projects)
            {
                MonitorProjects[project.Attribute("key").Value] = project.Attribute("value").Value;
            }
        }

        public void Monitor(INotificationService notificationService)
        {
            var fromVersion = new DateVersionSpec(_latestChangesetTime);
            bool hasChanges = false;

            try
            {
                foreach (var project in MonitorProjects)
                {
                    var changesets = _vcs.QueryHistory(project.Value, VersionSpec.Latest, 0, RecursionType.Full, null,
                        fromVersion, VersionSpec.Latest, int.MaxValue, false, true);
                    foreach (Changeset changeset in changesets)
                    {
                        var message = $"[*{project.Key}*]({changeset.ChangesetId}) - {changeset.CommitterDisplayName}:";
                        var slackMessage = new SlackPayload
                        {
                            text = message,
                            attachments = new List<SlackAttachment>
                            {
                                new SlackAttachment
                                {
                                    text = $"{changeset.Comment}",
                                    color = "good",
                                    mrkdwn_in = "[\"text\"]"
                                }
                            }
                        };
                        notificationService.SendAlert(slackMessage);

                        Console.WriteLine(message);
                        hasChanges = true;
                    }
                }
                if (hasChanges)
                    _latestChangesetTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}{1}", ex.Message, ex.StackTrace);
            }
        }
    }
}
