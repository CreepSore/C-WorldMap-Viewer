using System;
using System.IO;
using System.Xml.Linq;

namespace MapTime.Handlers
{
    public static class ConfigHandler
    {
        public const string DEFAULT_CFG = "Configuration/Config.xml";

        static string importedConfig = String.Empty;
        static XDocument xmlDoc;


        public static bool InitConfig()
        {
            try
            {
                xmlDoc = XDocument.Load(DEFAULT_CFG);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ReadKey(string key)
        {
            if (xmlDoc == null)
            {
                return null;
            }

            XElement startElem = xmlDoc.Element("configuration");
            foreach(XElement elem in startElem.Elements())
            {
                if(elem.Attribute("key").Value == key)
                {
                    return elem.Attribute("val").Value;
                }
            }

            return String.Empty;
        }
    }
}
