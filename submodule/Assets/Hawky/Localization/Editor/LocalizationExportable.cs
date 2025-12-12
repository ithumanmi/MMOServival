using Hawky.TSVtoJSON.Editor;
namespace Hawky.Localization.Editor
{
    public class LocalizationExportable : IExportable
    {
        public string FileName()
        {
            return "LocalizationConfig";
        }

        public string FolderName()
        {
            return "LocalizationConfig";
        }

        public string FromPath()
        {
            return "../DesignConfig";
        }

        public string ToPath()
        {
            return "Assets/Hawky_General/Resources";
        }
    }
}
