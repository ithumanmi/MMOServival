#if ANTADA_FALCON

using Hawki.SaveData;

namespace Hawki_Antada_Falcon.AppsFlyerEvent
{
    public class FalconAppsFlyerEventData : SaveDataBase<FalconAppsFlyerEventData>
    {
        public int currentStageAchieved;
    }
}

#endif