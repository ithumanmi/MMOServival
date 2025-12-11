using CreatorKitCodeInternal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashDamager : MonoBehaviour
{
    public float attackRange = 0.6f;
    public int damageAmount;

    public LayerMask mask = -1;
    public SerializableIAlignmentProvider alignment;

    static readonly Collider[] s_Enemies = new Collider[64];

    public int damage
    {
        get { return damageAmount; }
    }
    public IAlignmentProvider alignmentProvider
    {
        get { return alignment != null ? alignment.GetInterface() : null; }
    }
    protected virtual void OnCollisionEnter(Collision other)
    {
        int number = Physics.OverlapSphereNonAlloc(transform.position, attackRange, s_Enemies, mask);
        for (int index = 0; index < number; index++)
        {
            Collider enemy = s_Enemies[index];
            var damageable = enemy.GetComponent<Targetable>();
            if (damageable == null)
            {
                continue;
            }
            damageable.Stats.ChangeHealth(-damage);
            DamageUI.Instance.NewDamage(damage, damageable.transform.position);
            //damageable.Damage(damageAmount, damageable.position, alignmentProvider);
        }
    }
}
