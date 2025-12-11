using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SingletonPrefabInfo", menuName = "GameInfo/SingletonPrefabInfo")]

public class SingletonPrefabInfo : ScriptableObject
{
    public SceneLoaderManager sceneLoaderManagerPrefab;
    public AudioManager AudioManager;

}
