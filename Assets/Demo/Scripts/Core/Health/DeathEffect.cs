using CreatorKitCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DeathEffect : MonoBehaviour
{
    /// <summary>
    /// The DamageableBehaviour that will be used to assign the damageable
    /// </summary>
    [Tooltip("This field does not need to be populated here, it can be set up in code using AssignDamageable")]
    public DamageableBehaviour damageableBehaviour;

    /// <summary>
    /// Death particle system
    /// </summary>
    public ParticleSystem deathParticleSystemPrefab;

    /// <summary>
    /// World space offset of the <see cref="deathParticleSystemPrefab"/> position
    /// </summary>
    public Vector3 deathEffectOffset;

    /// <summary>
    /// The damageable
    /// </summary>
    protected StatSystem m_Damageable;

    /// <summary>
    /// Subscribes to the damageable's died event
    /// </summary>
    /// <param name="damageable"></param>
    public void AssignDamageable(StatSystem damageable)
    {
        if (m_Damageable != null)
        {
            m_Damageable.Died -= OnDied;
        }
        m_Damageable = damageable;
        m_Damageable.Died += OnDied;
    }

    /// <summary>
    /// If damageableBehaviour is populated, assigns the damageable
    /// </summary>
    protected virtual void Awake()
    {
        if (damageableBehaviour != null)
        {
            AssignDamageable(damageableBehaviour.Stats);
        }
    }

    /// <summary>
    /// Instantiate a death particle system
    /// </summary>
    void OnDied()
    {
        if (deathParticleSystemPrefab == null)
        {
            return;
        }

        var pfx = Poolable.TryGetPoolable<ParticleSystem>(deathParticleSystemPrefab.gameObject);
        pfx.transform.position = transform.position + deathEffectOffset;
        pfx.Play();
    }
}