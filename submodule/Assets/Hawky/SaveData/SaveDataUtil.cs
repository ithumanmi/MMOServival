namespace Hawky.SaveData
{
    public static class SaveDataUtil
    {
        public static string BuildKey(string userId, string dataId)
        {
            return $"{userId}_{dataId}";
        }
    }
}
