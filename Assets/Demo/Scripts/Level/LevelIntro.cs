using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelIntro : MonoBehaviour
{
    public event Action introCompleted;
    protected void SafelyCallIntroCompleted()
    {
        if (introCompleted != null)
        {
            introCompleted();
        }
    }
}
