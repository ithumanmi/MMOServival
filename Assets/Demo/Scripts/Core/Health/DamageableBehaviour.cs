using CreatorKitCode;
using System;
using UnityEngine;

public class DamageableBehaviour : MonoBehaviour
{
    public StatSystem Stats;
    public Transform targetTransform;
    public virtual Vector3 position
    {
        get { return transform.position; }
    }
    public event Action<DamageableBehaviour> removed;

    public event Action<DamageableBehaviour> died;
    public virtual void Remove()
    {
        // Set health to zero so that this behaviour appears to be dead. This will not fire death events
        Stats.Death();
        OnRemoved();
    }

    void OnDeath()
    {
        if (died != null)
        {
            died(this);
        }
    }
    void OnRemoved()
    {
        if (removed != null)
        {
            removed(this);
        }
    }
    public void OnConfigurationDied()
    {
        OnDeath();
        Remove();
    }
}
