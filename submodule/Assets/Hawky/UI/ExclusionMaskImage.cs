using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Hawky.UI
{
    public class ExclusionMaskImage : Image
    {
        private Material modifiedMaterial = null;
        private bool isMaterialModified = false;

        public override Material materialForRendering
        {
            get
            {
                if (!isMaterialModified)
                {
                    modifiedMaterial = new Material(base.materialForRendering);
                    modifiedMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                    isMaterialModified = true;
                }
                return modifiedMaterial;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (modifiedMaterial != null)
            {
                Destroy(modifiedMaterial);
                isMaterialModified = false;
            }
        }
    }
}

