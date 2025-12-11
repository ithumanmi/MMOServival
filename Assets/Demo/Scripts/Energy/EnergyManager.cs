using Core.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CreatorKitCode;
using CreatorKitCodeInternal;
using Random = UnityEngine.Random;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance { get; protected set; }
    public List<InfoEnergyLevel> levelEnergies;
    public Energy EnergyPrefab;
    protected int currentAmountEnegry;
    protected int m_CurrentIndexLevel;
    protected int m_CurrentAmountEnergyNextLevel;

    [Header("Health")]
    public Slider EnergySlider;
    public Text LevelDetail;
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        currentAmountEnegry = 0;
        m_CurrentIndexLevel = 0;
        m_CurrentAmountEnergyNextLevel = levelEnergies[m_CurrentIndexLevel].amountEnergyNextLevel;
    }

    void Update()
    {
        UpdatePlayerUI();
    }

    public void SpawnMultipleEnergy(Transform transform, int amountEnergy)
    {
        for (int i = 0; i < amountEnergy; i++)
        {
            Energy energy = SpawnEnegry();
            Vector3 alphaPositionRamdon = transform.position;
            alphaPositionRamdon.x = transform.position.x + Random.Range(2, -2);
            alphaPositionRamdon.z = transform.position.z + Random.Range(2, -2);
            energy.transform.position = alphaPositionRamdon;
            energy.gameObject.SetActive(true);
        }
    }

    public Energy SpawnEnegry()
    {
        var poolable = Poolable.TryGetPoolable<Energy>(EnergyPrefab.gameObject);
        if (poolable == null)
        {
            return null;
        }
        return poolable;
    }
    public void AddEnergy(int amountEnergy)
    {
        currentAmountEnegry += amountEnergy;
        if (currentAmountEnegry >= levelEnergies[m_CurrentIndexLevel].amountEnergyNextLevel)
        {
            int _amountEnergyTemp = currentAmountEnegry % levelEnergies[m_CurrentIndexLevel].amountEnergyNextLevel;
            if (TrySetupNextSpawn())
            {
                VFXManager.PlayVFX(VFXType.Healing, CharacterControl.Instance.transform.position);
                ResetCurrentAmountEnergy(_amountEnergyTemp);
                SelectWeaponManager.Instance.Show();

            }
        }
    }

    public void ResetCurrentAmountEnergy(int amountEnergy)
    {
        currentAmountEnegry = 0;
        currentAmountEnegry += amountEnergy;
    }
    protected bool TrySetupNextSpawn()
    {
        bool hasNext = levelEnergies.Next(ref m_CurrentIndexLevel);
        if (hasNext)
        {
            m_CurrentAmountEnergyNextLevel = levelEnergies[m_CurrentIndexLevel].amountEnergyNextLevel;
        }

        return hasNext;
    }

    void UpdatePlayerUI()
    {
        EnergySlider.value = currentAmountEnegry / (float)m_CurrentAmountEnergyNextLevel;
        LevelDetail.text = "Level " + (m_CurrentIndexLevel + 1).ToString();
    }
}
[Serializable]
public class InfoEnergyLevel
{
    public int amountEnergyNextLevel;
}
