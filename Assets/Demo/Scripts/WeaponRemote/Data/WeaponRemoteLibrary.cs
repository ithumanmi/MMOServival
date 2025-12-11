using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponLibrary.asset", menuName = "Weapon/Weapon Library", order = 1)]

public class WeaponRemoteLibrary : ScriptableObject, IList<RemoteWeapon>, IDictionary<string, RemoteWeapon>
{
    public List<RemoteWeapon> configurations;
    Dictionary<string, RemoteWeapon> m_ConfigurationDictionary;

    public RemoteWeapon this[int index] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public RemoteWeapon this[string key] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public int Count => throw new System.NotImplementedException();

    public bool IsReadOnly => throw new System.NotImplementedException();

    public ICollection<string> Keys => throw new System.NotImplementedException();

    public ICollection<RemoteWeapon> Values => throw new System.NotImplementedException();

    public void Add(RemoteWeapon item)
    {
        throw new System.NotImplementedException();
    }

    public void Add(string key, RemoteWeapon value)
    {
        throw new System.NotImplementedException();
    }

    public void Add(KeyValuePair<string, RemoteWeapon> item)
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public bool Contains(RemoteWeapon item)
    {
        throw new System.NotImplementedException();
    }

    public bool Contains(KeyValuePair<string, RemoteWeapon> item)
    {
        throw new System.NotImplementedException();
    }

    public bool ContainsKey(string key)
    {
        throw new System.NotImplementedException();
    }

    public void CopyTo(RemoteWeapon[] array, int arrayIndex)
    {
        throw new System.NotImplementedException();
    }

    public void CopyTo(KeyValuePair<string, RemoteWeapon>[] array, int arrayIndex)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator<RemoteWeapon> GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    public int IndexOf(RemoteWeapon item)
    {
        throw new System.NotImplementedException();
    }

    public void Insert(int index, RemoteWeapon item)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(RemoteWeapon item)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(string key)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(KeyValuePair<string, RemoteWeapon> item)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetValue(string key, out RemoteWeapon value)
    {
        throw new System.NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator<KeyValuePair<string, RemoteWeapon>> IEnumerable<KeyValuePair<string, RemoteWeapon>>.GetEnumerator()
    {
        throw new System.NotImplementedException();
    }
}
