using CreatorKitCodeInternal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanLauncher : Launcher
{
    public ParticleSystem fireParticleSystem;
    public override void Launch(Targetable enemy, GameObject attack, Transform firingPoint)
    {
        var hitscanAttack = attack.GetComponent<HitscanAttack>();
        if (hitscanAttack == null)
        {
            return;
        }
        hitscanAttack.transform.position = firingPoint.position;
        hitscanAttack.AttackEnemy(firingPoint.position, enemy);
        PlayParticles(fireParticleSystem, firingPoint.position, enemy.position);
    }
}
