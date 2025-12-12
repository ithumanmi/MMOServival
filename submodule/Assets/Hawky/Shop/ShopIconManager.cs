using Hawky.GameFlow;
using UnityEngine;
using UnityEngine.U2D;

namespace Hawky.Shop
{
    public class ShopIconManager : RuntimeSingleton<ShopIconManager>, IAwakeBehaviour
    {
        private SpriteAtlas _iconAtlat;

        public void Awake()
        {
            _iconAtlat = Resources.Load<SpriteAtlas>(ResourcesLink.SHOPICON);
        }

        public Sprite GetIcon(string name)
        {
            var sprite = _iconAtlat.GetSprite(name);

            if (sprite == null)
            {
                Debug.LogError($"Không tồn tại Sprite with Name = {name} trong resources {ResourcesLink.SHOPICON}");
            }

            return sprite;
        }
    }
}
