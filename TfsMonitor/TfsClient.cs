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
        private VersionControlServer _Vcs;
        private DateTime _LatestChangesetTime;
        public Dictionary<string, string> MonitorProjects { get; private set; }

        private TfsClient(string url)
        {
            var tfsConnection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(url));
            _Vcs = tfsConnection.GetService<VersionControlServer>();
            _LatestChangesetTime = DateTime.Now;
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
            var fromVersion = new DateVersionSpec(_LatestChangesetTime);
            bool hasChanges = false;

            try
            {
                foreach (var project in MonitorProjects)
                {
                    var changesets = _Vcs.QueryHistory(project.Value, VersionSpec.Latest, 0, RecursionType.Full, null,
                        fromVersion, VersionSpec.Latest, int.MaxValue, false, true);
                    foreach (Changeset changeset in changesets)
                    {
                        var message = string.Format("{0}: [{1}] {2}", changeset.CommitterDisplayName, project.Key,
                            changeset.Comment);
                        var slackMessage = new SlackPayload
                        {
                            text = message
                        };
                        notificationService.SendAlert(slackMessage);

                        Console.WriteLine(message);
                        hasChanges = true;
                    }
                }
                if (hasChanges)
                    _LatestChangesetTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}{1}", ex.Message, ex.StackTrace);
            }
        }
    }
}
