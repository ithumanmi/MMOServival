using UnityEngine;

namespace Hawky.HColor
{
    public abstract class ColorBase : MonoBehaviour
    {
        public abstract void SetColor(Color targetColor);

        protected virtual void OnValidate()
        {

        }
    }
}