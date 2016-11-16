using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace TfsMonitor
{
    public class ConfigHelper
    {
        private static ConfigHelper _configHelper;
        private readonly XDocument _xDocument;

        private ConfigHelper(string configPath)
        {
            _xDocument = XDocument.Load(configPath);
        }

        public static ConfigHelper GetInstance(string configPath)
        {
            return _configHelper ?? (_configHelper = new ConfigHelper(configPath));
        }

        public IEnumerable<XElement> GetConfig(string xPath)
        {
            return _xDocument.XPathSelectElements(xPath);
        }

        public string GetSingleConfig(string xPath)
        {
            return _xDocument.XPathSelectElement(xPath).Value;
        }
    }
}
