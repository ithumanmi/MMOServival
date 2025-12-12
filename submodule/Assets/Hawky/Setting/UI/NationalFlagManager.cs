using UnityEngine;
using UnityEngine.U2D;

namespace Hawky.Setting.UI
{
    public class NationalFlagManager : RuntimeSingleton<NationalFlagManager>, IAwakeBehaviour
    {
        private SpriteAtlas _nationalFlagAtlat;

        public void Awake()
        {
            _nationalFlagAtlat = Resources.Load<SpriteAtlas>("Images/NationalFlag");
        }

        public Sprite GetFlag(string name)
        {
            if (_nationalFlagAtlat == null)
            {
                return null;
            }

            return _nationalFlagAtlat.GetSprite(name);
        }
    }
}
