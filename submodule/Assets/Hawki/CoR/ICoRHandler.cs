namespace Hawki.CoR
{
    public interface ICoRHandler
    {
        string CorType();
        string CorId();
        void Handle(Request request);
    }
}
