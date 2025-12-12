namespace Hawky.TSVtoJSON.Editor
{
    public interface IExportable
    {
        string FromPath();
        string ToPath();
        string FolderName();
        string FileName();
    }
}
