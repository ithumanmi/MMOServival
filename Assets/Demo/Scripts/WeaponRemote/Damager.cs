using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;
public class Damager : MonoBehaviour
{
    public int damage;
    public Action<Vector3> hasDamaged;
    [Range(0, 1)]
    public float chanceToSpawnCollisionPrefab = 1.0f;
    public ParticleSystem collisionParticles;
    public SerializableIAlignmentProvider alignment;
    public IAlignmentProvider alignmentProvider
    {
        get { return alignment != null ? alignment.GetInterface() : null; }
    }
    public void SetDamage(int damageAmount)
    {
        if (damageAmount < 0)
        {
            return;
        }
        damage = damageAmount;
    }
    public void HasDamaged(Vector3 point, IAlignmentProvider otherAlignment)
    {
        if (hasDamaged != null)
        {
            hasDamaged(point);
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (collisionParticles == null || Random.value > chanceToSpawnCollisionPrefab)
        {
            return;
        }

        var pfx = Poolable.TryGetPoolable<ParticleSystem>(collisionParticles.gameObject);

        pfx.transform.position = transform.position;
        pfx.Play();
    }
}
