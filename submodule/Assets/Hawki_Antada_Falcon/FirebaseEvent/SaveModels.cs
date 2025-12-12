#if ANTADA_FALCON

using Hawki.SaveData;
using System.Collections.Generic;

namespace Hawki_Antada_Falcon.FirebaseEvent
{
    public class FalconFirebaseEventData : SaveDataBase<FalconFirebaseEventData>
    {
        public int currentStageCheckPoint;
        public Dictionary<int, int> stageCountFailed;

        public override void OnLoad()
        {
            base.OnLoad();

            if (stageCountFailed == null)
            {
                stageCountFailed = new Dictionary<int, int>();
            }
        }
    }
}

#endif
