namespace Hawky.Stat
{
    public class StatData : IStat
    {
        private IStat _bastStat;
        private IModifyStatByValue _modifyStatByValue;
        private IModifyStatByPercent _modifyStatByPercent;

        public StatData()
        {
            _bastStat = new SimpleStatData();

            _modifyStatByValue = new ModifyStatData();

            _modifyStatByPercent = new ModifyStatData();
        }

        public float Get(string statId)
        {
            var baseValue = _bastStat.Get(statId);

            var modifyPercentValue = _modifyStatByPercent.Get(statId);

            var modifyValue = _modifyStatByValue.Get(statId);

            return baseValue + modifyValue + modifyPercentValue * baseValue;
        }

        public void ModifyPercent(string statId, float value)
        {
            _modifyStatByPercent.ModifyPercent(statId, value);
        }

        public void ModifyValue(string statId, float value)
        {
            _modifyStatByValue.ModifyValue(statId, value);
        }

        public void ResetAll()
        {
            _bastStat.ResetAll();
            _modifyStatByValue.ResetAll();
            _modifyStatByPercent.ResetAll();
        }

        public void Set(string statId, float value)
        {
            _bastStat.Set(statId, value);
        }
    }
}