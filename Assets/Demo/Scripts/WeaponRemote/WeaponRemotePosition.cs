using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRemotePosition : MonoBehaviour
{
    public RemoteWeapon remoteWeapon;
    public bool HaveRemoteWeapon
    {
        get
        {
            return remoteWeapon != null;
        }
    }
    public Transform GetTransform()
    {
        return transform;
    }
}
