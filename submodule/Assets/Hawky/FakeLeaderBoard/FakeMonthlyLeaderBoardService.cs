using Hawky.AllConfig;
using Hawky.Config;
using Hawky.SaveData;
using System.Collections.Generic;

namespace Hawky.FakeLeaderBoard
{
    public class FakeMonthlyLeaderBoardService : FakeLeaderBoardBaseService<FakeMonthlyLeaderBoardService>, IUpdateBehaviour
    {
        public override void AddPoint(long point)
        {
            var saveData = SaveDataManager.Ins.GetData<FakeLeaderBoardData>();

            saveData.monthlyPoint += point;

            saveData.Save();
        }

        protected override long Point()
        {
            var saveData = SaveDataManager.Ins.GetData<FakeLeaderBoardData>();

            return saveData.monthlyPoint;
        }

        protected override List<FakeLeaderBoardConfig> Configs()
        {
            var result = new List<FakeLeaderBoardConfig>();
            var config = ConfigManager.Ins.GetData<ConfigAll>();
            if (config.FakeMonthlyLeaderBoardConfig != null)
            {
                result.AddRange(config.FakeMonthlyLeaderBoardConfig);
            }
            return result;
        }

        public void Update()
        {
            var saveData = SaveDataManager.Ins.GetData<FakeLeaderBoardData>();

            if (saveData.nextTimeReseetMonthly == 0)
            {
                saveData.nextTimeReseetMonthly = TimeUtility.GetBeginNextMonth();
                saveData.monthlyPoint = 0;
                saveData.Save();
                return;
            }

            var now = TimeUtility.Now();
            var beginNextMonth = TimeUtility.GetBeginNextMonth();
            if (now > beginNextMonth)
            {
                saveData.nextTimeReseetMonthly = TimeUtility.GetBeginNextMonth();
                saveData.monthlyPoint = 0;
                saveData.Save();
            }
        }
    }
}
