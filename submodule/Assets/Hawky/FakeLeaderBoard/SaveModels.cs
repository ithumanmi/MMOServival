using Hawky.SaveData;

namespace Hawky.FakeLeaderBoard
{
    public partial class FakeLeaderBoardData : SaveDataBase<FakeLeaderBoardData>
    {
        public long point;
        public long monthlyPoint;

        public long nextTimeReseetMonthly;
    }
}