using Hawky.SaveData;
using UnityEngine;

namespace Hawky.Setting
{
    public class SettingData : SaveDataBase<SettingData>
    {
        public bool sound;
        public bool music;
        public bool vibrate;
        public string name;
        public SystemLanguage language = SystemLanguage.English;

        public override void Default()
        {
            base.Default();

            sound = true;
            music = true;
            vibrate = true;
            language = SystemLanguage.English;
            name = "Player";
        }
    }
}
