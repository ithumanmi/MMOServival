namespace Hawky.Loading
{
    public interface ILoadingController
    {
        string GetId();
        void StartLoad();
        int CurrentPoint();
        int TotalPoint();
    }
}
