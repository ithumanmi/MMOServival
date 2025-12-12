using System.Collections.Generic;

namespace Hawky.Stat
{
    public class SimpleStatData : IStat
    {
        private Dictionary<string, float> _dictStat = new Dictionary<string, float>();

        public float Get(string statId)
        {
            if (_dictStat.TryGetValue(statId, out var value))
            {
                return value;
            }

            return 0;
        }

        public void ModifyPercent(string statId, float value)
        {
            ModifyStatExtension.Modify(this, statId, value);
        }

        public void ModifyValue(string statId, float value)
        {
            ModifyStatExtension.Modify(this, statId, value);
        }

        public void ResetAll()
        {
            _dictStat.Clear();
        }

        public void Set(string statId, float value)
        {
            _dictStat[statId] = value;
        }
    }
}