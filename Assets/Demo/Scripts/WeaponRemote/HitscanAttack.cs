using Core.Utilities;
using CreatorKitCodeInternal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanAttack : MonoBehaviour
{
    public float delay;

    protected Timer m_Timer;
    protected Targetable m_Enemy;
    protected Damager m_Damager;
    protected Vector3 m_Origin;
    protected bool m_PauseTimer;
    public void AttackEnemy(Vector3 origin, Targetable enemy)
    {
        //Debug.Log("AttackEnemy");
        m_Enemy = enemy;
        m_Origin = origin;
        m_Timer.Reset();
        m_PauseTimer = false;
    }
    protected void DealDamage()
    {
        Poolable.TryPool(gameObject);

        if (m_Enemy == null)
        {
            return;
        }

        // effects
        ParticleSystem pfxPrefab = m_Damager.collisionParticles;
        var attackEffect = Poolable.TryGetPoolable<ParticleSystem>(pfxPrefab.gameObject);
        attackEffect.transform.position = m_Enemy.position;
        attackEffect.Play();

        m_Enemy.Stats.Damage(m_Damager.damage);
        //m_Enemy.TakeDamage(m_Damager.damage, m_Enemy.position, m_Damager.alignmentProvider);

        Rigidbody enemyRigidbody = m_Enemy.GetComponent<Rigidbody>();
        if (enemyRigidbody != null)
        {
            // Tính toán hướng lực đẩy (từ kẻ tấn công tới kẻ thù)
            Vector3 direction = (m_Enemy.position - transform.position).normalized;

            // Đảm bảo rằng bạn áp dụng lực đẩy đúng hướng và với cường độ mong muốn
            float pushForce = 10f;  // Cường độ lực đẩy, có thể điều chỉnh
            float adjustedForce = pushForce / enemyRigidbody.mass;
            enemyRigidbody.AddForce(direction * adjustedForce, ForceMode.Impulse);// Áp dụng lực đẩy lên enemy
        }

        m_PauseTimer = true;
    }
    protected virtual void Awake()
    {
        m_Damager = GetComponent<Damager>();
        m_Timer = new Timer(delay, DealDamage);
    }
    protected virtual void Update()
    {
        if (!m_PauseTimer)
        {
            m_Timer.Tick(Time.deltaTime);
        }
    }
}
