using UnityEngine;

namespace Hawky.Stat
{
    public static class ModifyStatExtension
    {
        public static void Modify(this IStat modifyStat, string statId, float value)
        {
            var oldValue = modifyStat.Get(statId);

            modifyStat.Set(statId, oldValue + value);
        }
    }
}
