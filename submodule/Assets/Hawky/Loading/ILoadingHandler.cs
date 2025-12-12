using System.Collections.Generic;

namespace Hawky.Loading
{
    public interface ILoadingHandler
    {
        string GetId();
        void RequireHandlerIds(List<string> requires);
        void StartLoad();
        void OnRequireLoadDone(Dictionary<string, ILoadingHandler> handlers);
        int TotalPoint();
        int CurrentPoint();
        string CurrentText();
    }
}
