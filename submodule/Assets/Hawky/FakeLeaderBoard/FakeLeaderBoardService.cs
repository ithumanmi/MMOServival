using Hawky.AllConfig;
using Hawky.Config;
using Hawky.SaveData;
using System.Collections.Generic;

namespace Hawky.FakeLeaderBoard
{
    public class FakeLeaderBoardService : FakeLeaderBoardBaseService<FakeLeaderBoardService>
    {
        protected override List<FakeLeaderBoardConfig> Configs()
        {
            var result = new List<FakeLeaderBoardConfig>();
            var config = ConfigManager.Ins.GetData<ConfigAll>();
            if (config.FakeLeaderBoardConfig != null)
            {
                result.AddRange(config.FakeLeaderBoardConfig);
            }
            return result;
        }

        protected override long Point()
        {
            var saveData = SaveDataManager.Ins.GetData<FakeLeaderBoardData>();

            return saveData.point;
        }

        public override void AddPoint(long point)
        {
            var saveData = SaveDataManager.Ins.GetData<FakeLeaderBoardData>();

            saveData.point += point;

            saveData.Save();
        }
    }
}
