namespace Hawky.CoR
{
    public interface ICoRHandler
    {
        string CorType();
        string CorId();
        void Handle(Request request);
    }
}
