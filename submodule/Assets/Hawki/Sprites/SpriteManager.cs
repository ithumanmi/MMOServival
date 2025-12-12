using Hawki.GameFlow;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hawki.Sprites
{
    public partial class SpriteManager : RuntimeSingleton<SpriteManager>
    {
        private Dictionary<string, Sprite> _spritesLoaded = new Dictionary<string, Sprite>();

        public Sprite LoadSprite(string materialId)
        {
            if (_spritesLoaded.TryGetValue(materialId, out var sprite) == false)
            {
                var link = Path.Combine(ResourcesLink.SPRITES, materialId);
                sprite = Resources.Load<Sprite>(link);

                if (sprite == null)
                {
                    return null;
                }

                _spritesLoaded[materialId] = sprite;
            }

            return sprite;
        }
    }

}