using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameDataStoreBase : IDataStore
{

    public float masterVolume = 1;

    public float sfxVolume = 1;

    public float musicVolume = 1;

    /// <summary>
    /// Called just before we save
    /// </summary>
    public abstract void PreSave();

    /// <summary>
    /// Called just after load
    /// </summary>
    public abstract void PostLoad();
}