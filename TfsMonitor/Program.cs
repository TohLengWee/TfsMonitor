using System.Threading;
using TfsMonitor.Service.Interfaces;

namespace TfsMonitor
{
    class Program
    {
        private static GlobalObject _go;
        public static INotificationService NotificationService = new SlackClient();

        static void Main(string[] args)
        {
            Init();
            while (true)
            {
                _go.TfsClient.Monitor(NotificationService);
                Thread.Sleep(1);
            }
        }

        private static void Init()
        {
            var configHelper = ConfigHelper.GetInstance("config.xml");
            var tfsClient = InitTfsClient(configHelper);

            _go = new GlobalObject()
            {
                ConfigHelper = configHelper,
                TfsClient = tfsClient
            };
        }

        private static TfsClient InitTfsClient(ConfigHelper configHelper)
        {
            var tfsClient = TfsClient.GetInstance(configHelper.GetSingleConfig("config/tfs/url"));
            var projects = configHelper.GetConfig("config/tfs/projects/project");
            tfsClient.LoadMonitorProjects(projects);
            return tfsClient;
        }
    }

    public class GlobalObject
    {
        public ConfigHelper ConfigHelper { get; set; }
        public TfsClient TfsClient { get; set; }
    }
}