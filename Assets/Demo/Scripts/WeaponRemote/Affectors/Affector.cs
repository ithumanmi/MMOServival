using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Affector : MonoBehaviour
{
    public string description;

    public IAlignmentProvider alignment { get; protected set; }
    public LayerMask enemyMask { get; protected set; }
    public virtual void Initialize(IAlignmentProvider affectorAlignment, LayerMask mask)
    {
        alignment = affectorAlignment;
        enemyMask = mask;
    }
    public virtual void Initialize(IAlignmentProvider affectorAlignment)
    {
        Initialize(affectorAlignment, -1);
    }
}
