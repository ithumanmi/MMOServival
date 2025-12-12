using Hawky.TSVtoJSON.Editor;

namespace Hawky.AllConfig.Editor
{
    public class AllConfigExportable : IExportable
    {
        public string FileName()
        {
            return "AllConfig";
        }

        public string FolderName()
        {
            return "AllConfig";
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
