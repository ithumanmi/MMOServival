namespace Hawky.Loading
{
    // kế thừa class này sẽ tự động được add vào Loading Controller Factory
    public interface IAutoLoadingControllerCreator : ILoadingControllerCreator
    {

    }

    // kế thừa class này để trở thành creator
    public interface ILoadingControllerCreator
    {
        string GetId();
        ILoadingController CreateLoading();
    }
}
