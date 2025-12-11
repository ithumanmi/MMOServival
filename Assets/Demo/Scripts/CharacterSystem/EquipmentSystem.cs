using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalEnum;

namespace CreatorKitCode 
{
    /// <summary>
    /// Handles the equipment stored inside an instance of CharacterData. Will take care of unequipping the previous
    /// item when equipping a new one in the same slot.
    /// </summary>
    /// 
    [System.Serializable]
    public class EquipmentSystem
    {
        

        public Action<EquipmentItem> OnEquiped { get; set; }
        public Action<EquipmentItem> OnUnequip { get; set; }

        CharacterData m_Owner;
        
        public EquipmentItem m_HeadSlot;
        public EquipmentItem m_TorsoSlot;
        public EquipmentItem m_LegsSlot;
        public EquipmentItem m_FeetSlot;
        public EquipmentItem m_AccessorySlot;
        public Weapon Weapon;
        public Weapon weaponDefault;
        public void Init(CharacterData owner)
        {
            m_Owner = owner;
        }
        public void Init(Weapon weapon, bool isEnemy)
        {
            if (isEnemy) return;
            if (Weapon == null)
            {
                Debug.Log("Init");
                Weapon = weapon;
                SetWeapon();
            }else
            {
                SetWeapon();
            }
        }

        public void SetWeapon()
        {
            if(Weapon != null)
            {
                Debug.Log("SetWeapon");
                Weapon.UsedBy(m_Owner);
            }
            else
            {
                if(weaponDefault != null)
                {
                    weaponDefault.UsedBy(m_Owner);
                }
            }
        }

        public void ResetWeapon()
        {
            weaponDefault.UsedBy(m_Owner);
        }

        public EquipmentItem GetItem(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Head:
                    return m_HeadSlot;
                case EquipmentSlot.Torso:
                    return m_TorsoSlot;
                case EquipmentSlot.Legs:
                    return m_LegsSlot;
                case EquipmentSlot.Feet:
                    return m_FeetSlot;
                case EquipmentSlot.Accessory:
                    return m_AccessorySlot;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Equip the given item for the given user. This won't check about requirement, this should be done by the
        /// inventory before calling equip!
        /// </summary>
        /// <param name="item">Which item to equip</param>
        public void Equip(EquipmentItem item)
        {
            Unequip(item.Slot);

            OnEquiped?.Invoke(item);

            switch (item.Slot)
            {
                case EquipmentSlot.Head:
                {
                    m_HeadSlot = item;
                    m_HeadSlot.EquippedBy(m_Owner);
                }
                    break;
                case EquipmentSlot.Torso:
                {
                    m_TorsoSlot = item;
                    m_TorsoSlot.EquippedBy(m_Owner);
                }
                    break;
                case EquipmentSlot.Legs:
                {
                    m_LegsSlot = item;
                    m_LegsSlot.EquippedBy(m_Owner);
                }
                    break;
                case EquipmentSlot.Feet:
                {
                    m_FeetSlot = item;
                    m_FeetSlot.EquippedBy(m_Owner);
                }
                    break;
                case EquipmentSlot.Accessory:
                {
                    Debug.Log(m_AccessorySlot);
                    m_AccessorySlot = item;
                    m_AccessorySlot.EquippedBy(m_Owner);
                }
                    break;
                //special value for weapon
                case EquipmentSlot.Weapon:
                    Weapon = item as Weapon;
                    Weapon.EquippedBy(m_Owner);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Unequip the item in the given slot. isReplacement is used to tell the system if this unequip was called
        /// because we equipped something new in that slot or just unequip to empty slot. This is for the weapon : the
        /// weapon slot can't be empty, so if this is not a replacement, this will auto-requip the base weapon.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="isReplacement"></param>
        public void Unequip(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Head:
                    if (m_HeadSlot != null)
                    {
                        m_HeadSlot.UnequippedBy(m_Owner);
                        GameDataManager.instance.InventoryData.AddItem(m_HeadSlot);
                        OnUnequip?.Invoke(m_HeadSlot);
                        m_HeadSlot = null;
                    }
                    break;
                case EquipmentSlot.Torso:
                    if (m_TorsoSlot != null)
                    {
                        m_TorsoSlot.UnequippedBy(m_Owner);
                        GameDataManager.instance.InventoryData.AddItem(m_TorsoSlot);
                        OnUnequip?.Invoke(m_TorsoSlot);
                        m_TorsoSlot = null;
                    }
                    break;
                case EquipmentSlot.Legs:
                    if (m_LegsSlot != null)
                    {
                        m_LegsSlot.UnequippedBy(m_Owner);
                        GameDataManager.instance.InventoryData.AddItem(m_LegsSlot);
                        OnUnequip?.Invoke(m_LegsSlot);
                        m_LegsSlot = null;
                    }
                    break;
                case EquipmentSlot.Feet:
                    if (m_FeetSlot != null)
                    {
                        m_FeetSlot.UnequippedBy(m_Owner);
                        GameDataManager.instance.InventoryData.AddItem(m_FeetSlot);
                        OnUnequip?.Invoke(m_FeetSlot);
                        m_FeetSlot = null;
                    }
                    break;
                case EquipmentSlot.Accessory:
                    if (m_AccessorySlot != null)
                    {
                        Debug.Log(m_AccessorySlot);
                        Debug.Log("Accessory");
                        m_AccessorySlot.UnequippedBy(m_Owner);
                        GameDataManager.instance.InventoryData.AddItem(m_AccessorySlot);
                        OnUnequip?.Invoke(m_AccessorySlot);
                        m_AccessorySlot = null;
                    }
                    break;
                case EquipmentSlot.Weapon:
                    if (Weapon != null ) // the only way to unequip the default weapon is through replacing it
                    {
                        Weapon.UnequippedBy(m_Owner);
                        GameDataManager.instance.InventoryData.AddItem(Weapon);
                        OnUnequip?.Invoke(Weapon);
                        Weapon = null;
                    }
                    break;
                default:
                    break;
            }
        }
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void LoadJson(string jsonFilepath)
        {
            JsonUtility.FromJsonOverwrite(jsonFilepath, this);
        }
    }
}