using System;
using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.UI;

namespace CreatorKitCodeInternal 
{
    /// <summary>
    /// Main class that handle the Game UI (health, open/close inventory)
    /// </summary>
    public class UISystem : MonoBehaviour
    {
        public static UISystem Instance { get; private set; }
    
        [Header("Player")]
        public CharacterControl PlayerCharacter;
        public Slider PlayerHealthSlider;
        public Text MaxHealth;
        public Text CurrentHealth;
        public Button BtnSetttings;
        public Canvas Canvas;

        void Awake()
        {
            Instance = this;
        
        }

        private void OnEnable()
        {
            BtnSetttings.onClick.AddListener(ShowSetting);
        }
        void Start()
        {
            if (CoreGameManager.instance)
            {
                CoreGameManager.instance.canShowScene = true;
            }
       
            StartWaveButtonPressed();
            HideSetting();
            SelectWeaponManager.Instance.Show();
           

        }

        void Update()
        {
            UpdatePlayerUI();
        }

        void UpdatePlayerUI()
        {
            PlayerHealthSlider.value = PlayerCharacter.Data.damageableBehaviour.Stats.CurrentHealth / (float)PlayerCharacter.Data.damageableBehaviour.Stats.stats.health;
            MaxHealth.text = PlayerCharacter.Data.damageableBehaviour.Stats.stats.health.ToString();
            CurrentHealth.text = PlayerCharacter.Data.damageableBehaviour.Stats.CurrentHealth.ToString();

        }

    
        public void StartWaveButtonPressed()
        {
            if (LevelManager.instanceExists)
            {
                LevelManager.instance.BuildingCompleted();
            }
        }
        public void UpgradeSelectedTower(RemoteWeapon remoteWeapon)
        {
            if (remoteWeapon == null)
            {
                throw new InvalidOperationException("Selected Tower is null");
            }

            if (remoteWeapon.isAtMaxLevel)
            {
                Debug.Log("This weapon is already at max level.");
                return;
            }

            remoteWeapon.UpgradeTower();
            Debug.Log($"Weapon {remoteWeapon.name} upgraded!");
        }

        public void ShowSetting()
        {
            AudioManager.instance.PlayAltButtonSound();
            GameUI.instance.Pause();
            Canvas.enabled = true;
        }
        public void HideSetting()
        {
            AudioManager.instance.PlayAltButtonSound();
            GameUI.instance.Unpause();
            Canvas.enabled = false;
        }
    }
}