using Hawki.AllConfig;
using Hawki.Config;
using Hawki.SaveData;
using System.Collections.Generic;

namespace Hawki.FakeLeaderBoard
{
    public class FakeLeaderBoardService : FakeLeaderBoardBaseService<FakeLeaderBoardService>
    {
        protected override List<FakeLeaderBoardConfig> Configs()
        {
            var result = new List<FakeLeaderBoardConfig>();
            var config = ConfigManager.Instance.GetData<ConfigAll>();
            if (config.FakeLeaderBoardConfig != null)
            {
                result.AddRange(config.FakeLeaderBoardConfig);
            }
            return result;
        }

        protected override long Point()
        {
            var saveData = SaveDataManager.Instance.GetData<FakeLeaderBoardData>();

            return saveData.point;
        }

        public override void AddPoint(long point)
        {
            var saveData = SaveDataManager.Instance.GetData<FakeLeaderBoardData>();

            saveData.point += point;

            saveData.Save();
        }
    }
}
