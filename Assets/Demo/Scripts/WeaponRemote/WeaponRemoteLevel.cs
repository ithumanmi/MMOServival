using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponRemote.Data;

[DisallowMultipleComponent]
public class WeaponRemoteLevel : MonoBehaviour, ISerializationCallbackReceiver
{
    public GameObject buildEffectPrefab;

    public WeaponRemoteLevelData levelData;

    Affector[] m_Affectors;
    protected RemoteWeapon m_ParentTower;
    protected Affector[] Affectors
    {
        get
        {
            if (m_Affectors == null)
            {
                m_Affectors = GetComponentsInChildren<Affector>();
            }
            return m_Affectors;
        }
    }
    public LayerMask mask
    {
        get; protected set;
    }

    public void SetAffectorState(bool state)
    {
        foreach (Affector affector in Affectors)
        {
            if (affector != null)
            {
                affector.enabled = state;
            }
        }
    }
    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
    }
    void Start()
    {
        if (buildEffectPrefab != null)
        {
            Instantiate(buildEffectPrefab, transform);
        }

    }
    public virtual void Initialize(RemoteWeapon tower, LayerMask enemyMask, IAlignmentProvider alignment)
    {
        mask = enemyMask;

        foreach (Affector effect in Affectors)
        {
            effect.Initialize(alignment, mask);
        }
        m_ParentTower = tower;
    }
    public string GetDescription()
    {
        string desc = "currentLevel: " + levelData.currentLevel + "\n";
       if(levelData.currentLevel == levelData.nextLevel)
        {
            desc += "NextLevel: " + "Max Level" + "\n";
        }
        else
        {
            desc += "NextLevel: " + levelData.nextLevel + "\n";
        }
        
        desc += "Damage: " + levelData.Damage + "\n";
        desc += "Radius: " + levelData.Radius + "\n";
        desc += "Firerate: " + levelData.Firerate;

        return desc;
    }


}