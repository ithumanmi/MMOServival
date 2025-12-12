using UnityEngine;

namespace Hawky.UI
{
    public class AutoRotate : MonoBehaviour
    {
        [SerializeField] private float speed = 120f;

        private void Update()
        {
            var newZ = speed * Time.time % 360;

            transform.rotation = Quaternion.Euler(0, 0, newZ);
        }
    }
}
