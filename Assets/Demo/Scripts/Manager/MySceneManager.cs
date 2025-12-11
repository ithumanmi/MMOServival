using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySceneManager : MonoBehaviour
{
    public virtual GlobalEnum.SceneIndex mySceneIndex => GlobalEnum.SceneIndex.LoginScene;
    public bool canShowScene { get; set; }
    public virtual void InitAllCallback() { }
    public virtual void RemoveAllCallback() { }
}
