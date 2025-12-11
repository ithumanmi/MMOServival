using CreatorKitCodeInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeaponRemote;

public class SelectWeaponManager : MonoBehaviour
{
    public static SelectWeaponManager Instance { get; protected set; }
    public Transform container;
    public Transform popup;
    public VariableJoystick variableJoystick;
    public List<RemoteWeapon> selectWeapon = new List<RemoteWeapon>();
    public List<ButtonSelectRemoteWeapon> listButtonSelectRemoteWeapon = new List<ButtonSelectRemoteWeapon>();
    void Awake()
    {
        Instance = this;
    }
    protected List<RemoteWeapon> SelectWeaponsForLevelUp()
    {
        List<RemoteWeapon> selectedWeapons = new List<RemoteWeapon>();
        HashSet<int> usedIndices = new HashSet<int>();

        // Lấy vũ khí hiện có chưa max level
        List<RemoteWeapon> upgradeableWeapons = CharacterControl.Instance.weaponRemotes
            .FindAll(weapon => !weapon.isAtMaxLevel);

        // Kiểm tra nếu nhân vật đã có đủ vũ khí
        bool isFullWeapons = CharacterControl.Instance.FullWeaponRemote();

        if (isFullWeapons)
        {
            // Nếu nhân vật đã đủ số lượng vũ khí, chỉ thêm các vũ khí có thể nâng cấp
            int upgradeableCount = Mathf.Min(3, upgradeableWeapons.Count);
            while (selectedWeapons.Count < upgradeableCount)
            {
                int randomIndex = UnityEngine.Random.Range(0, upgradeableWeapons.Count);
                if (!usedIndices.Contains(randomIndex))
                {
                    usedIndices.Add(randomIndex);
                    selectedWeapons.Add(upgradeableWeapons[randomIndex]);
                }
            }
        }
        else
        {
            // Nếu chưa đủ số lượng vũ khí, thêm vũ khí mới từ thư viện
            int upgradeableCount = Mathf.Min(2, upgradeableWeapons.Count);
            while (selectedWeapons.Count < upgradeableCount)
            {
                int randomIndex = UnityEngine.Random.Range(0, upgradeableWeapons.Count);
                if (!usedIndices.Contains(randomIndex))
                {
                    usedIndices.Add(randomIndex);
                    selectedWeapons.Add(upgradeableWeapons[randomIndex]);
                }
            }

            // Thêm vũ khí mới từ thư viện nếu còn chỗ
            while (selectedWeapons.Count < 3)
            {
                int randomIndex = UnityEngine.Random.Range(0, LevelManager.instance.weaponLibrary.configurations.Count);
                RemoteWeapon newWeapon = LevelManager.instance.weaponLibrary.configurations[randomIndex];
                if (!selectedWeapons.Contains(newWeapon))
                {
                    selectedWeapons.Add(newWeapon);
                }
            }
        }

        return selectedWeapons;
    }

    public void CreateButtonSelectWeapon()
    {
        selectWeapon = SelectWeaponsForLevelUp();
        for (int i = 0; i < listButtonSelectRemoteWeapon.Count; i++)
        {
            ButtonSelectRemoteWeapon button = listButtonSelectRemoteWeapon[i];
            button.button.onClick.RemoveAllListeners();

            if (selectWeapon.Count > i)
            {
                button.gameObject.SetActive(true);
                RemoteWeapon remoteWeapon = selectWeapon[i];
                bool isNewWeapon = remoteWeapon.CharacterData == null;

                WeaponRemoteLevel weaponLevel = isNewWeapon
                    ? remoteWeapon.levels[0]
                    : remoteWeapon.levels[remoteWeapon.currentLevel + 1];

                button.icon.sprite = weaponLevel.levelData.icon;
                if (isNewWeapon)
                {
                    button.textUpgrade.text = "NEW WEAPON";
                    button.textUpgrade.color = Color.green;
                }
                else
                {
                    button.textUpgrade.text = "UPGRACE";
                    button.textUpgrade.color = Color.yellow;
                }
                button.description.text = weaponLevel.GetDescription();

                if (isNewWeapon)
                {
                    button.button.onClick.AddListener(() => CreateWeaponRemote(remoteWeapon));
                }
                else
                {
                    button.button.onClick.AddListener(() => UpgradeSelectedTower(remoteWeapon));
                }
            }
            else
            {
                button.gameObject.SetActive(false);
                button.icon.sprite = null;
                button.description.text = string.Empty;
            }
        }
    }

    public void CreateWeaponRemote(RemoteWeapon weaponRemotePrefab)
    {
        // Tạo một vũ khí mới
        if (CharacterControl.Instance.FullWeaponRemote())
        {
            return;
        }
        RemoteWeapon remoteWeapon = Instantiate(weaponRemotePrefab, CharacterControl.Instance.PosWeaponRemote.GetTransform(), false);
        CharacterControl.Instance.PosWeaponRemote.remoteWeapon = remoteWeapon;
        CharacterControl.Instance.weaponRemotes.Add(remoteWeapon);
        remoteWeapon.Initialize(CharacterControl.Instance.Data);
        remoteWeapon.transform.localPosition = Vector3.zero;
        Hide();
    }
    public void UpgradeSelectedTower(RemoteWeapon remoteWeapon)
    {
        if (remoteWeapon == null)
        {
            throw new InvalidOperationException("Selected Tower is null");
        }

        if (remoteWeapon.isAtMaxLevel)
        {
            return;
        }

        remoteWeapon.UpgradeTower();
        Hide();
    }

    public void Show()
    {
        if (!ShouldShowWeaponSelection())
        {
            return;
        }
        variableJoystick.gameObject.SetActive(false);
        GameUI.instance.Pause();
        CreateButtonSelectWeapon();
        popup.gameObject.SetActive(true);
    }
    private bool ShouldShowWeaponSelection()
    {
        // Kiểm tra nếu nhân vật đã có đủ số lượng vũ khí
        if (CharacterControl.Instance.FullWeaponRemote())
        {
            // Kiểm tra nếu tất cả vũ khí đều đạt max level
            foreach (var weapon in CharacterControl.Instance.weaponRemotes)
            {
                if (!weapon.isAtMaxLevel)
                {
                    return true; // Có ít nhất một vũ khí chưa max level
                }
            }
            return false; // Tất cả vũ khí đều max level
        }

        return true; // Chưa đủ số lượng vũ khí
    }

    public void Hide()
    {
        AudioManager.instance.PlayAltButtonSound();
        variableJoystick.gameObject.SetActive(true);   
        selectWeapon.Clear();
        popup.gameObject.SetActive(false);
        GameUI.instance.Unpause();
    }

}
