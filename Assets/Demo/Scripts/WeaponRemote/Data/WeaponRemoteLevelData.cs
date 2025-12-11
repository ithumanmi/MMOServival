using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WeaponRemote.Data
{
    [CreateAssetMenu(fileName = "Weapon.asset", menuName = "Weapon/Weapon Configuration", order = 1)]
    public class WeaponRemoteLevelData : ScriptableObject
    {
        public Sprite icon;
        public int currentLevel;
        public int nextLevel;
        public float Radius;
        public float Firerate;
        public int Damage;
    }
}

