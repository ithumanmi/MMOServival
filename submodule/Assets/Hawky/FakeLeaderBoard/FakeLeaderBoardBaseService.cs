using Hawky.SaveData;
using Hawky.Setting;
using System.Collections.Generic;
using System.Linq;

namespace Hawky.FakeLeaderBoard
{
    public class LeaderBoardUnitData
    {
        public int id;
        public string displayName;
        public long point;
        public bool isSelf;
    }

    public abstract class FakeLeaderBoardBaseService<T> : RuntimeSingleton<T>, IStartBehaviour where T : RuntimeSingleton<T>
    {
        private List<FakeLeaderBoardConfig> _config;
        public abstract void AddPoint(long point);

        protected abstract List<FakeLeaderBoardConfig> Configs();
        protected abstract long Point();

        public List<LeaderBoardUnitData> CalculateLeaderBoard()
        {
            var point = Point();

            return CalculateLeaderBoard(point);
        }

        public List<LeaderBoardUnitData> CalculateLeaderBoard(long point)
        {
            var leaderBoardData = new List<LeaderBoardUnitData>();
            var settingData = SaveDataManager.Ins.GetData<SettingData>();

            var newData = new LeaderBoardUnitData()
            {
                displayName = settingData.name,
                point = point,
                isSelf = true
            };

            leaderBoardData.Add(newData);

            var config = _config;
            foreach (var cf in config)
            {
                newData = new LeaderBoardUnitData()
                {
                    displayName = cf.name,
                    point = cf.point,
                    id = cf.id,
                };

                leaderBoardData.Add(newData);
            }

            leaderBoardData = leaderBoardData.OrderByDescending(x => x.point).ToList();

            for (int i = 0; i < leaderBoardData.Count; i++)
            {
                leaderBoardData[i].id = i + 1;
            }

            return leaderBoardData;
        }

        public void Start()
        {
            _config = Configs();
        }
    }
}
