using CreatorKitCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalEnum;

public class RemoteWeapon : Targetable
{
    public CharacterData CharacterData;
    public WeaponRemoteLevel[] levels;

    public string towerName;

    public LayerMask enemyLayerMask;
    public int currentLevel { get; protected set; }
    public WeaponRemoteLevel currentTowerLevel { get; protected set; }
    public bool isAtMaxLevel
    {
        get { return currentLevel == levels.Length - 1; }
    }
    public void Initialize(CharacterData onwer)
    {
        CharacterData = onwer;
        SetLevel(0);
        if (LevelManager.instanceExists)
        {
            LevelManager.instance.levelStateChanged += OnLevelStateChanged;
        }
    }
    public virtual bool UpgradeTower()
    {
        if (isAtMaxLevel)
        {
            return false;
        }
        SetLevel(currentLevel + 1);
        return true;
    }
    protected void SetLevel(int level)
    {
        if (level < 0 || level >= levels.Length)
        {
            return;
        }
        currentLevel = level;
        if (currentTowerLevel != null)
        {
            Destroy(currentTowerLevel.gameObject);
        }

        // instantiate the visual representation
        currentTowerLevel = Instantiate(levels[currentLevel], transform);

        // initialize TowerLevel
        currentTowerLevel.Initialize(this, enemyLayerMask, Stats.alignmentProvider);

        // disable affectors
        LevelState levelState = LevelManager.instance.levelState;
        bool initialise = levelState == LevelState.AllEnemiesSpawned || levelState == LevelState.SpawningEnemies;
        currentTowerLevel.SetAffectorState(initialise);
    }

    protected virtual void OnDestroy()
    {
        if (LevelManager.instanceExists)
        {
            LevelManager.instance.levelStateChanged += OnLevelStateChanged;
        }
    }
    protected virtual void OnLevelStateChanged(LevelState previous, LevelState current)
    {
        bool initialise = current == LevelState.AllEnemiesSpawned || current == LevelState.SpawningEnemies;
        currentTowerLevel.SetAffectorState(initialise);
    }
}
