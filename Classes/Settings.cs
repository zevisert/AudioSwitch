using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using AudioSwitch.CoreAudioApi;

namespace AudioSwitch.Classes
{
    [XmlRoot, Serializable]
    public class Settings
    {
        public static Settings NewSettings()
        {
            return new Settings
            {
                DefaultDataFlow = EDataFlow.eRender,
                Device = new List<CDevice>(),
                ShowHardwareName = true
                //QuickSwitchEnabled = false,
            };
        }

        [XmlIgnore]
        private static readonly string settingsxml = Program.AppDataRoot + "Settings.xml";

        [XmlElement]
        public EDataFlow DefaultDataFlow;

        [XmlElement]
        public bool ShowHardwareName;

        [XmlElement]
        public List<CDevice> Device;

        public class CDevice
        {
            [XmlAttribute]
            public string DeviceID;

            [XmlAttribute]
            public bool HideFromList;

            [XmlAttribute]
            public int Hue;

            [XmlAttribute]
            public int Saturation;

            [XmlAttribute]
            public int Brightness;

            [XmlAttribute]
            public bool UseCustomName;

            [XmlAttribute]
            public string CustomName;
        }

        internal void Save()
        {
            var xs = new XmlSerializer(typeof(Settings));
            using (var tw = new StreamWriter(settingsxml))
                xs.Serialize(tw, this);
        }

        internal static Settings Load()
        {
            try
            {
                var xs = new XmlSerializer(typeof(Settings));
                using (var fileStream = new StreamReader(settingsxml))
                    return (Settings)xs.Deserialize(fileStream);
            }
            catch
            {
                var newsettings = NewSettings();
                newsettings.Save();
                return newsettings;
            }
        }
    }
}
