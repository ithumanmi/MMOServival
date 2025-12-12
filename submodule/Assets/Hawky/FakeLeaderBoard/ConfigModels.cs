using Hawky.FakeLeaderBoard;
using System.Collections.Generic;

namespace Hawky.AllConfig
{
    public partial class ConfigAll
    {
        public List<FakeLeaderBoardConfig> FakeLeaderBoardConfig = new List<FakeLeaderBoardConfig>();
        public List<FakeMonthlyLeaderBoardConfig> FakeMonthlyLeaderBoardConfig = new List<FakeMonthlyLeaderBoardConfig>();
    }
}

namespace Hawky.FakeLeaderBoard
{
    public class FakeLeaderBoardConfig
    {
        public int id;
        public string name;
        public int point;
    }

    public class FakeMonthlyLeaderBoardConfig : FakeLeaderBoardConfig
    {

    }
}