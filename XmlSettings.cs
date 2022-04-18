using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TrayToolbar
{
    internal class XmlSettings
    {
        static void Load(string file)
        {
            XmlDocument xmlDoc1 = new XmlDocument();
            //xmlDoc1.Load("test.xml");
            xmlDoc1.Load(file);
            XmlNodeList? itemNodes1 = xmlDoc1.SelectNodes("//buttons/ignore");

            foreach (XmlNode itemNode in itemNodes1)
            {
                XmlNode textNode = itemNode.SelectSingleNode("text");
            }
        }
    }
}
