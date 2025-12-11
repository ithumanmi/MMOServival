using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalEnum;
/// <summary>
/// The level manager - handles the level states and tracks the player's currency
/// </summary>
[RequireComponent(typeof(WaveManager))]
public class LevelManager : Singleton<LevelManager>
{
    /// <summary>
    /// The configured level intro. If this is null the LevelManager will fall through to the gameplay state (i.e. SpawningEnemies)
    /// </summary>
    public LevelIntro intro;

    public WeaponRemoteLibrary weaponLibrary;


    /// <summary>
    /// The currency that the player starts with
    /// </summary>
    public int startingCurrency;


    /// <summary>
    /// Configuration for if the player gains currency even in pre-build phase
    /// </summary>
    [Header("Setting this will allow currency gain during the Intro and Pre-Build phase")]
    public bool alwaysGainCurrency;

    public Collider[] environmentColliders;

    /// <summary>
    /// The attached wave manager
    /// </summary>
    public WaveManager waveManager { get; protected set; }

    /// <summary>
    /// Number of enemies currently in the level
    /// </summary>
    public int numberOfEnemies { get; protected set; }

    /// <summary>
    /// The current state of the level
    /// </summary>
    public LevelState levelState { get; protected set; }

    public int numberOfHomeBasesLeft { get; protected set; }
    public int numberOfHomeBases { get; protected set; }
    public bool isGameOver
    {
        get { return (levelState == LevelState.Win) || (levelState == LevelState.Lose); }
    }
    public event Action levelCompleted;
    public event Action levelFailed;
    public event Action<LevelState, LevelState> levelStateChanged;
    public event Action<int> numberOfEnemiesChanged;
    public event Action homeBaseDestroyed;
    public virtual void IncrementNumberOfEnemies()
    {
        numberOfEnemies++;
        SafelyCallNumberOfEnemiesChanged();
    }
    public virtual void DecrementNumberOfEnemies()
    {
        numberOfEnemies--;
        SafelyCallNumberOfEnemiesChanged();
        if (numberOfEnemies < 0)
        {
            Debug.LogError("[LEVEL] There should never be a negative number of enemies. Something broke!");
            numberOfEnemies = 0;
        }

        if (numberOfEnemies == 0 && levelState == LevelState.AllEnemiesSpawned)
        {
            ChangeLevelState(LevelState.Win);
        }
    }

    /// <summary>
    /// Completes building phase, setting state to spawn enemies
    /// </summary>
    public virtual void BuildingCompleted()
    {
        ChangeLevelState(LevelState.SpawningEnemies);
    }

    /// <summary>
    /// Caches the attached wave manager and subscribes to the spawning completed event
    /// Sets the level state to intro and ensures that the number of enemies is set to 0
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        waveManager = GetComponent<WaveManager>();
        waveManager.spawningCompleted += OnSpawningCompleted;

        // Does not use the change state function as we don't need to broadcast the event for this default value
        levelState = LevelState.Intro;
        numberOfEnemies = 0;
        if (intro != null)
        {
            intro.introCompleted += IntroCompleted;
        }
        else
        {
            IntroCompleted();
        }

        numberOfHomeBasesLeft = numberOfHomeBases;
    }

    /// <summary>
    /// Updates the currency gain controller
    /// </summary>
    protected virtual void Update()
    {

    }

    /// <summary>
    /// Unsubscribes from events
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (waveManager != null)
        {
            waveManager.spawningCompleted -= OnSpawningCompleted;
        }
        if (intro != null)
        {
            intro.introCompleted -= IntroCompleted;
        }
    }

    /// <summary>
    /// Fired when Intro is completed or immediately, if no intro is specified
    /// </summary>
    protected virtual void IntroCompleted()
    {
        ChangeLevelState(LevelState.Building);
    }

    /// <summary>
    /// Fired when the WaveManager has finished spawning enemies
    /// </summary>
    protected virtual void OnSpawningCompleted()
    {
        ChangeLevelState(LevelState.AllEnemiesSpawned);
    }

    /// <summary>
    /// Changes the state and broadcasts the event
    /// </summary>
    /// <param name="newState">The new state to transitioned to</param>
    protected virtual void ChangeLevelState(LevelState newState)
    {

        // If the state hasn't changed then return
        if (levelState == newState)
        {
            return;
        }
        LevelState oldState = levelState;
        levelState = newState;
        if (levelStateChanged != null)
        {
            levelStateChanged(oldState, newState);
        }
        switch (newState)
        {
            case LevelState.SpawningEnemies:
                waveManager.StartWaves();
                break;
            case LevelState.AllEnemiesSpawned:
                // Win immediately if all enemies are already dead
                if (numberOfEnemies == 0)
                {
                    ChangeLevelState(LevelState.Win);
                }
                break;
            case LevelState.Lose:
                SafelyCallLevelFailed();
                break;
            case LevelState.Win:
                SafelyCallLevelCompleted();
                break;
            case LevelState.Out:
                waveManager.waves.Clear();
                break;

        }
    }

    public void CallLevelFaild()
    {
        ChangeLevelState(LevelState.Lose);
    }
    public void CallOutGame()
    {
        ChangeLevelState(LevelState.Out);
    }
    /// <summary>
    /// Fired when a home base is destroyed
    /// </summary>
    protected virtual void OnHomeBaseDestroyed(DamageableBehaviour homeBase)
    {
        // Decrement the number of home bases
        numberOfHomeBasesLeft--;

        // Call the destroyed event
        if (homeBaseDestroyed != null)
        {
            homeBaseDestroyed();
        }

        // If there are no home bases left and the level is not over then set the level to lost
        if ((numberOfHomeBasesLeft == 0) && !isGameOver)
        {
            ChangeLevelState(LevelState.Lose);
        }
    }

    /// <summary>
    /// Calls the <see cref="levelCompleted"/> event
    /// </summary>
    protected virtual void SafelyCallLevelCompleted()
    {
        if (levelCompleted != null)
        {
            levelCompleted();
        }
    }

    /// <summary>
    /// Calls the <see cref="numberOfEnemiesChanged"/> event
    /// </summary>
    protected virtual void SafelyCallNumberOfEnemiesChanged()
    {
        if (numberOfEnemiesChanged != null)
        {
            numberOfEnemiesChanged(numberOfEnemies);
        }
    }

    /// <summary>
    /// Calls the <see cref="levelFailed"/> event
    /// </summary>
    protected virtual void SafelyCallLevelFailed()
    {
        if (levelFailed != null)
        {
            levelFailed();
        }
    }
}
