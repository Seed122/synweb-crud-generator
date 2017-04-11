using System.IO;
using SPGenerator.Common;
using SD = SPGenerator.DataModel;

namespace SPGenerator.UI.Models
{
    internal class SettingsModel
    {
        internal void SaveSettings(SD.Settings settings)
        {
            var filePath = Path.GetTempPath() + "\\" + Constants.settingTempFileName;
            Serializer.Serialize(settings, filePath);
        }

        internal SPGenerator.DataModel.Settings GetSettings()
        {
            var filePath = Path.GetTempPath() + "\\" + Constants.settingTempFileName;
            var settings = Serializer.Deserialize(new DataModel.Settings(), filePath);
            if (settings == null)
                settings = new DataModel.Settings();
            return settings;
        }
    }
}
