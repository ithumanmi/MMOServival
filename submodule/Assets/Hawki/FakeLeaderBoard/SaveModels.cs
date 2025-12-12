using Hawki.SaveData;

namespace Hawki.FakeLeaderBoard
{
    public partial class FakeLeaderBoardData : SaveDataBase<FakeLeaderBoardData>
    {
        public long point;
        public long monthlyPoint;

        public long nextTimeReseetMonthly;
    }
}