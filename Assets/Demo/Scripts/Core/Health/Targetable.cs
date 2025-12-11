using CreatorKitCode;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class Targetable : DamageableBehaviour
{
    protected Vector3 m_CurrentPosition, m_PreviousPosition;
    protected List<Targetable> m_TargetsInRange = new List<Targetable>();
    public virtual Vector3 velocity { get; protected set; }
    protected LevelManager m_LevelManager;
    public Transform targetableTransform
    {
        get
        {
            return targetTransform == null ? transform : targetTransform;
        }
    }
   
    public override Vector3 position
    {
        get { return targetableTransform.position; }
    }

    protected void Awake()
    {
        LazyLoad();
        ResetPositionData();
    }
    public virtual void Initialize()
    {
        Stats.ResetHealth();
        gameObject.SetActive(true);
        ResetPositionData();
        LazyLoad();
        m_LevelManager.IncrementNumberOfEnemies();
    }

    protected void ResetPositionData()
    {
        m_CurrentPosition = position;
        m_PreviousPosition = position;
    }
    void FixedUpdate()
    {
        m_CurrentPosition = position;
        velocity = (m_CurrentPosition - m_PreviousPosition) / Time.fixedDeltaTime;
        m_PreviousPosition = m_CurrentPosition;
    }
    public override void Remove()
    {
        base.Remove();
        m_LevelManager.DecrementNumberOfEnemies();
        Poolable.TryPool(gameObject);
    }
    protected virtual void LazyLoad()
    {
        if (m_LevelManager == null)
        {
            m_LevelManager = LevelManager.instance;
        }
    }
}
