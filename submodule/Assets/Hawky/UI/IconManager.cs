using Hawky.GameFlow;
using UnityEngine;
using UnityEngine.U2D;

namespace Hawky.UI
{
    public partial class IconManager : RuntimeSingleton<IconManager>, IAwakeBehaviour
    {
        private SpriteAtlas _iconAtlat;

        public void Awake()
        {
            _iconAtlat = Resources.Load<SpriteAtlas>(ResourcesLink.ICON);
        }

        public Sprite GetIcon(string name)
        {
            var sprite = _iconAtlat.GetSprite(name);

            if (sprite == null)
            {
                Debug.LogError($"Không tồn tại Sprite with Name = {name} trong resources {ResourcesLink.ICON}");
            }

            return sprite;
        }
    }
}