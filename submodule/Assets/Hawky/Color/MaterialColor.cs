using UnityEngine;

namespace Hawky.HColor
{
    public abstract class MaterialColor : ColorBase
    {
        [SerializeField] private string _propertyName = "_Color";
        public override void SetColor(Color targetColor)
        {
            var materials = GetMaterials();

            foreach (var material in GetMaterials())
            {
                material.SetColor(_propertyName, targetColor);
            }
        }

        protected abstract Material[] GetMaterials();
    }
}