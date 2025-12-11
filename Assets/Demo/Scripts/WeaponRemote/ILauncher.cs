using CreatorKitCodeInternal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILauncher
{

    void Launch(Targetable enemy, GameObject attack, Transform firingPoint);
    void Launch(Targetable enemy, GameObject attack, Transform[] firingPoints);
    void Launch(List<Targetable> enemies, GameObject attack, Transform[] firingPoints);
}
