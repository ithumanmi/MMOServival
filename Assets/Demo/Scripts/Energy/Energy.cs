using CreatorKitCode;
using System;
using UnityEngine;
public class Energy : MonoBehaviour
{
    public int amountEnergy;

    LootSpawner m_LootSpawner;
    private void Start()
    {
        m_LootSpawner = GetComponent<LootSpawner>();
    }
    public int UseEnergy()
    {
        if (m_LootSpawner != null)
            m_LootSpawner.SpawnLoot();
        gameObject.SetActive(false);

        return amountEnergy;
    }


}
