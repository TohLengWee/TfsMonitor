using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace TfsMonitor
{
    public class ConfigHelper
    {
        private static ConfigHelper _configHelper;
        private XDocument _XDocument;

        private ConfigHelper(string configPath)
        {
            _XDocument = XDocument.Load(configPath);
        }

        public static ConfigHelper GetInstance(string configPath)
        {
            return _configHelper ?? (_configHelper = new ConfigHelper(configPath));
        }

        public IEnumerable<XElement> GetConfig(string xPath)
        {
            return _XDocument.XPathSelectElements(xPath);
        }

        public string GetSingleConfig(string xPath)
        {
            return _XDocument.XPathSelectElement(xPath).Value;
        }
    }
}
