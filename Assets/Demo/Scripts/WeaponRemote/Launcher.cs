using CreatorKitCodeInternal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Launcher : MonoBehaviour, ILauncher
{
    public abstract void Launch(Targetable enemy, GameObject attack, Transform firingPoint);

    public virtual void Launch(Targetable enemy, GameObject attack, Transform[] firingPoints)
    {
        var poolable = Poolable.TryGetPoolable<Poolable>(attack);
        if (poolable == null)
        {
            return;
        }
        Launch(enemy, poolable.gameObject, GetRandomTransform(firingPoints));
    }

    public virtual void Launch(List<Targetable> enemies, GameObject attack, Transform[] firingPoints)
    {
        int count = enemies.Count;
        int currentFiringPointIndex = 0;
        int firingPointLength = firingPoints.Length;
        for (int i = 0; i < count; i++)
        {
            Targetable enemy = enemies[i];
            Transform firingPoint = firingPoints[currentFiringPointIndex];
            currentFiringPointIndex = (currentFiringPointIndex + 1) % firingPointLength;
            var poolable = Poolable.TryGetPoolable<Poolable>(attack);
            if (poolable == null)
            {
                return;
            }
            Launch(enemy, poolable.gameObject, firingPoint);
        }
    }
    public void PlayParticles(ParticleSystem particleSystemToPlay, Vector3 origin, Vector3 lookPosition)
    {
        if (particleSystemToPlay == null)
        {
            return;
        }
        particleSystemToPlay.transform.position = origin;
        particleSystemToPlay.transform.LookAt(lookPosition);
        particleSystemToPlay.Play();
    }
    public Transform GetRandomTransform(Transform[] launchPoints)
    {
        int index = Random.Range(0, launchPoints.Length);
        return launchPoints[index];
    }
}
