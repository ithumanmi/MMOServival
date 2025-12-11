using CreatorKitCode;
using System;
using UnityEngine;
using UnityEngine.UI;
using static CreatorKitCode.InventorySystem;

namespace CreatorKitCodeInternal 
{
    public class ItemTooltip : MonoBehaviour
    {
        public Text Name;
        public Text DescriptionText;
        public Image imageWeapon;
        public GameObject panel;
        public event Action<int> ItemEquipmentClicked;

        public int indexItem;

        RectTransform m_RectTransform;
        CanvasScaler m_CanvasScaler;
        Canvas m_Canvas;
        void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_CanvasScaler = GetComponentInParent<CanvasScaler>();
            m_Canvas = GetComponentInParent<Canvas>();
        }

        public void OnHide()
        {
            gameObject.SetActive(false);
        }
        public void OnEquipment()
        {
            AudioManager.instance.PlayAltButtonSound();
            Debug.Log("OnBuy");
            OnHide();
            ItemEquipmentClicked?.Invoke(indexItem);
        }
    }
}