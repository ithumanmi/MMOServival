using Hawky.GameFlow;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hawky.Materials
{
    public partial class MaterialManager : RuntimeSingleton<MaterialManager>
    {
        private Dictionary<string, Material> _materialLoaded = new Dictionary<string, Material>();

        public Material LoadMaterial(string materialId)
        {
            if (_materialLoaded.TryGetValue(materialId, out var material) == false)
            {
                var link = Path.Combine(ResourcesLink.MATERIALS, materialId);
                material = Resources.Load<Material>(link);

                if (material == null)
                {
                    return null;
                }

                _materialLoaded[materialId] = material;
            }

            var newMaterial = new Material(material);
            newMaterial.name = materialId;
            return newMaterial;
        }
    }
}
