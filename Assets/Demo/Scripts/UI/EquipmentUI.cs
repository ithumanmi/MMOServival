using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.UI;

namespace CreatorKitCodeInternal 
{
    /// <summary>
    /// Keep reference and update the Equipment entry (the 6 icons around the character in the Inventory)
    /// </summary>
    public class EquipmentUI : MonoBehaviour
    {
        public ItemEntryUI HeadSlot;
        public ItemEntryUI TorsoSlot;
        public ItemEntryUI LegsSlot;
        public ItemEntryUI FeetSlot;
        public ItemEntryUI AccessorySlot;
        public ItemEntryUI WeaponSlot;
        
        public void Init(InventoryUI owner)
        {
            HeadSlot.Owner = owner;
            TorsoSlot.Owner = owner;
            LegsSlot.Owner = owner;
            FeetSlot.Owner = owner;
            AccessorySlot.Owner = owner;
            WeaponSlot.Owner = owner;
        }

        public void UpdateEquipment(EquipmentSystem equipment)
        {
            var head = equipment.GetItem(GlobalEnum.EquipmentSlot.Head);
            var torso = equipment.GetItem(GlobalEnum.EquipmentSlot.Torso);
            var legs = equipment.GetItem(GlobalEnum.EquipmentSlot.Legs);
            var feet = equipment.GetItem(GlobalEnum.EquipmentSlot.Feet);
            var accessory = equipment.GetItem(GlobalEnum.EquipmentSlot.Accessory);
            var weapon = equipment.Weapon;

            HeadSlot.SetupEquipment(head);
            TorsoSlot.SetupEquipment(torso);
            LegsSlot.SetupEquipment(legs);
            FeetSlot.SetupEquipment(feet);
            AccessorySlot.SetupEquipment(accessory);
            WeaponSlot.SetupEquipment(weapon);
        }
    }
}