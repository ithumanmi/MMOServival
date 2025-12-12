using UnityEngine;

namespace Hawky.UI
{
    public static class MaterialUtil
    {
        public static void SetSmoothness(this Material material, float smoothness)
        {
            material.SetFloat("_Glossiness", smoothness);
        }
    }
}