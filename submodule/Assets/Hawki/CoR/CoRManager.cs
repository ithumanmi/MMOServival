using System.Collections.Generic;

namespace Hawki.CoR
{
    public class CoRManager : RuntimeSingleton<CoRManager>
    {
        public Dictionary<string, List<ICoRHandler>> _dicHandlerList = new Dictionary<string, List<ICoRHandler>>();
    }
}
