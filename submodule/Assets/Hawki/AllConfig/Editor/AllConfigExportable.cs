using Hawki.TSVtoJSON.Editor;

namespace Hawki.AllConfig.Editor
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
            return "Assets/Hawki_General/Resources";
        }
    }
}
