using Hawky.Loading;
public class DefaultLoadingControllerCreator : IAutoLoadingControllerCreator
{
    public ILoadingController CreateLoading()
    {
        var loadingController = new LoadingController(LoadingControllerId.DEFAULT);

        loadingController.AddHandler(new DefaultLoading(10, 100));

        return loadingController;
    }

    public string GetId()
    {
        return LoadingControllerCreatorId.DEFAULT;
    }
}

