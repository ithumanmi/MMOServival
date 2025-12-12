using UnityEngine;

namespace Hawky.UI
{
    public abstract class CustomComponent<T> : MonoBehaviour where T : Component
    {
        [SerializeField] protected T _component;
        protected virtual void OnValidate()
        {
            if (_component == null)
            {
                _component = GetComponent<T>();
            }
        }
    }
}
