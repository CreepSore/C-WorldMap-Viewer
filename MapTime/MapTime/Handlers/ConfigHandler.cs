using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MapTime.Handlers
{
    public static class ConfigHandler
    {
        public const string DEFAULT_CFG = "Configuration/Config.xml";

        static string latestHash = String.Empty;
        static XDocument xmlDoc;

        static MD5 md5Hasher = MD5.Create();
        static long latestReload = 0;

        public static bool InitConfig()
        {
            if(Environment.TickCount - latestReload < 1000)
            {
                return false;
            }
            latestReload = Environment.TickCount;

            try
            {
                XDocument newdoc = XDocument.Load(DEFAULT_CFG);
                string currentHash = System.BitConverter.ToString(
                        md5Hasher.ComputeHash(
                            System.Text.Encoding.Default.GetBytes(
                                newdoc.ToString()
                            )
                        )
                    );

                if(currentHash != latestHash)
                {
                    xmlDoc = newdoc;
                    latestHash = currentHash;
                }

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
