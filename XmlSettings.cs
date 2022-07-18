using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TrayToolbar
{
    internal class XmlSettings
    {
        static XmlSettings xmlsettings;
        public static XmlSettings Instance
        {
            get
            {
                if (xmlsettings == null)
                {
                    xmlsettings = new XmlSettings();
                }
                return xmlsettings;
            }
        }

        //public List<XmlSettings> settings;
        public List<Setting> settings = new List<Setting>();

        public string cachedFile;

        //public static void Load(string file)
        public List<Setting> Load(string file)
        {
            settings.Clear();

            cachedFile = file;
            //List<Setting> settings = new();

            XmlDocument xmlDoc1 = new XmlDocument();
            //xmlDoc1.Load("test.xml");
            xmlDoc1.Load(file);
            XmlNodeList? itemNodes1 = xmlDoc1.SelectNodes("//settings/setting");

            foreach (XmlNode itemNode in itemNodes1)
            {
                //XmlNode settingsNode = itemNode.SelectSingleNode("setting");
                //XmlAttributeCollection xmlAttributes = settingsNode.Attributes;
                XmlAttributeCollection xmlAttributes = itemNode.Attributes;

                Setting setting = new Setting(xmlAttributes["name"].Value, xmlAttributes["value"].Value);
                settings.Add(setting);
            }
            //settings = settings;
            //XmlSerializer ser = new XmlSerializer(typeof(Setting));

            return settings;
        }

        internal object Name()
        {
            throw new NotImplementedException();
        }

        public string GetValueOfSetting(string name)
        {
            //if (cashedFile != null)
            //    Load(cashedFile);

            if (settings != null && settings.Find(s => s.Name.Equals(name)) != null)
                return settings.Find(s => s.Name.Equals(name)).Value;

            return null;
        }
    }

    class Setting
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Setting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override bool Equals(object? obj)
        {
            //return base.Equals(obj);
            //return obj is Setting other && other.Name == Name && other.Value == Value;
            return Name == obj;
        }
    }
}
