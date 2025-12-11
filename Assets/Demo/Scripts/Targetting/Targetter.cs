using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Targetter : MonoBehaviour
{
    public event Action<Targetable> targetEntersRange;
    public event Action<Targetable> targetExitsRange;
    public event Action<Targetable> acquiredTarget;
    public event Action lostTarget;
    public Transform turret;
    public Vector2 turretXRotationRange = new Vector2(0, 359);
    public bool onlyYTurretRotation;
    public float idleRotationSpeed = 39f;
    public float searchRate;
    public float idleCorrectionTime = 2.0f;
    public Collider attachedCollider;
    public float idleWaitTime = 2.0f;

    protected List<Targetable> m_TargetsInRange = new List<Targetable>();
    protected float m_SearchTimer = 0.0f;
    protected float m_WaitTimer = 0.0f;
    protected Targetable m_CurrrentTargetable;
    protected float m_XRotationCorrectionTime;
    protected bool m_HadTarget;
    protected float m_CurrentRotationSpeed;
    public IAlignmentProvider alignment;
    public float effectRadius
    {
        get
        {
            var sphere = attachedCollider as SphereCollider;
            if (sphere != null)
            {
                return sphere.radius;
            }
            var capsule = attachedCollider as CapsuleCollider;
            if (capsule != null)
            {
                return capsule.radius;
            }
            return 0;
        }
    }
    public Targetable GetTarget()
    {
        //Debug.Log("GetTarget");
        return m_CurrrentTargetable;
    }
    public List<Targetable> GetAllTargets()
    {
        return m_TargetsInRange;
    }
    protected virtual void Update()
    {
        m_SearchTimer -= Time.deltaTime;
        if (m_SearchTimer <= 0.0f && m_CurrrentTargetable == null && m_TargetsInRange.Count > 0)
        {
            m_CurrrentTargetable = GetNearestTargetable();
            if (m_CurrrentTargetable != null)
            {
                if (acquiredTarget != null)
                {

                    acquiredTarget(m_CurrrentTargetable);
                }

                m_SearchTimer = searchRate;
            }
        }

        AimTurret();

        m_HadTarget = m_CurrrentTargetable != null;
    }
    public GameObject laser;
    public void ActiveLaser()
    {
        laser.SetActive(true);
    }
    public void UnActiveLaser()
    {
        laser.SetActive(false);
    }
    protected virtual Targetable GetNearestTargetable()
    {
        int length = m_TargetsInRange.Count;

        if (length == 0)
        {
            return null;
        }

        Targetable nearest = null;
        float distance = float.MaxValue;
        for (int i = length - 1; i >= 0; i--)
        {
            Targetable targetable = m_TargetsInRange[i];
            if (targetable == null || targetable.Stats.IsDeal())
            {
                m_TargetsInRange.RemoveAt(i);
                continue;
            }
            float currentDistance = Vector3.Distance(transform.position, targetable.transform.position);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                nearest = targetable;
            }
        }

        return nearest;
    }
    public void ResetTargetter()
    {
        m_TargetsInRange.Clear();
        m_CurrrentTargetable = null;

        targetEntersRange = null;
        targetExitsRange = null;
        acquiredTarget = null;
        lostTarget = null;

        // Reset turret facing
        if (turret != null)
        {
            turret.localRotation = Quaternion.identity;
        }
    }
    protected virtual bool IsTargetableValid(Targetable targetable)
    {
        if (targetable == null)
        {
            return false;
        }

        IAlignmentProvider targetAlignment = targetable.Stats.alignmentProvider;
        bool canDamage = alignment == null || targetAlignment == null ||
                         alignment.CanHarm(targetAlignment);

        return canDamage;
    }
    protected virtual bool IsContain(Targetable targetable)
    {
        if (m_TargetsInRange.Contains(targetable)) return false;
        return true;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        var targetable = other.GetComponent<Targetable>();
        if (!IsTargetableValid(targetable))
        {
            return;
        }
        targetable.removed += OnTargetRemoved;
        m_TargetsInRange.Add(targetable);

        //Debug.Log("m_TargetsInRange.Add" + m_TargetsInRange.Count);
        if (targetEntersRange != null)
        {
            targetEntersRange(targetable);
        }
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        var targetable = other.GetComponent<Targetable>();
        if (!IsTargetableValid(targetable))
        {
            return;
        }

        m_TargetsInRange.Remove(targetable);
        if (targetExitsRange != null)
        {
            targetExitsRange(targetable);
        }
        if (targetable == m_CurrrentTargetable)
        {
            OnTargetRemoved(targetable);
        }
        else
        {
            // Only need to remove if we're not our actual target, otherwise OnTargetRemoved will do the work above
            targetable.removed -= OnTargetRemoved;
        }
    }
    void OnTargetRemoved(DamageableBehaviour target)
    {
        target.removed -= OnTargetRemoved;
        if (m_CurrrentTargetable != null && target == m_CurrrentTargetable)
        {
            if (lostTarget != null)
            {
                lostTarget();
            }
            m_HadTarget = false;
            m_TargetsInRange.Remove(m_CurrrentTargetable);
            m_CurrrentTargetable = null;
            m_XRotationCorrectionTime = 0.0f;
        }
        else //wasnt the current target, find and remove from targets list
        {
            for (int i = 0; i < m_TargetsInRange.Count; i++)
            {
                if (m_TargetsInRange[i] == target)
                {
                    m_TargetsInRange.RemoveAt(i);
                    break;
                }
            }
        }
    }
    protected virtual void AimTurret()
    {
        if (turret == null)
        {
            return;
        }
        if (m_CurrrentTargetable == null) // do idle rotation
        {
            if (laser != null)
            {
                UnActiveLaser();
            }
            if (m_WaitTimer > 0)
            {
                m_WaitTimer -= Time.deltaTime;
                if (m_WaitTimer <= 0)
                {
                    m_CurrentRotationSpeed = (Random.value * 2 - 1) * idleRotationSpeed;
                }
            }
            else
            {
                Vector3 euler = turret.rotation.eulerAngles;
                euler.x = Mathf.Lerp(Wrap180(euler.x), 0, m_XRotationCorrectionTime);
                m_XRotationCorrectionTime = Mathf.Clamp01((m_XRotationCorrectionTime + Time.deltaTime) / idleCorrectionTime);
                euler.y += m_CurrentRotationSpeed * Time.deltaTime;

                turret.eulerAngles = euler;
            }
            //turret.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            if (laser != null)
            {
                ActiveLaser();
            }
            m_WaitTimer = idleWaitTime;

            Vector3 targetPosition = m_CurrrentTargetable.transform.position;
            if (onlyYTurretRotation)
            {
                targetPosition.y = turret.position.y;
            }
            Vector3 direction = targetPosition - turret.position;
            Quaternion look = Quaternion.LookRotation(direction, Vector3.up);
            Vector3 lookEuler = look.eulerAngles;
            // We need to convert the rotation to a -180/180 wrap so that we can clamp the angle with a min/max
            float x = Wrap180(lookEuler.x);
            lookEuler.x = Mathf.Clamp(x, turretXRotationRange.x, turretXRotationRange.y);
            look.eulerAngles = lookEuler;
            turret.rotation = look;
        }
    }

    static float Wrap180(float angle)
    {
        angle %= 360;
        if (angle < -180)
        {
            angle += 360;
        }
        else if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }


}


