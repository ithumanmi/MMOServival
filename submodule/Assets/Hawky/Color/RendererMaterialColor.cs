using UnityEngine;

namespace Hawky.HColor
{
    public class RendererMaterialColor : MaterialColor
    {
        [SerializeField] private Renderer _render;

        protected override Material[] GetMaterials()
        {
            return _render.materials;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_render == null)
            {
                _render = GetComponent<Renderer>();
            }
        }
    }

}