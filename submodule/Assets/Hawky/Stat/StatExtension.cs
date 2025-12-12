using UnityEngine;

namespace Hawky.Stat
{
    public static class StatExtension
    {
        public static bool GetB(this IStat stat, string statId)
        {
            return stat.Get(statId) > 0;
        }

        public static void SetB(this IStat stat, string statId, bool value)
        {
            stat.Set(statId, value ? 1f : 0);
        }

        public static void SetVector3(this IStat stat, string statId, Vector3 vector3)
        {
            stat.Set($"{statId}_vector3_x", vector3.x);
            stat.Set($"{statId}_vector3_y", vector3.y);
            stat.Set($"{statId}_vector3_z", vector3.z);
        }

        public static Vector3 GetVector3(this IStat stat, string statId)
        {
            var x = stat.Get($"{statId}_vector3_x");
            var y = stat.Get($"{statId}_vector3_y");
            var z = stat.Get($"{statId}_vector3_z");

            return new Vector3(x, y, z);
        }
    }
}
