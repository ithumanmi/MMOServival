using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalEnum;
public struct MagnetData
{
    // gold, gems, health potion, power potion
    public ShopItemType ItemType;

    // ParticleSystem pool
    public ParticleSystem FXPool;

}

public class CoinMagnet : MonoBehaviour
{
    [SerializeField] Camera m_Camera;
    [SerializeField] float m_ZDepth = 10f;

    [SerializeField] Vector3 m_SourceOffset = new Vector3(0f, 0.1f, 0f);
    [SerializeField] List<MagnetData> m_MagnetData;

    void OnEnable()
    {
        GameDataManager.TransactionProcessed += OnTransactionProcessed;
        //GameDataManager.RewardProcessed += OnRewardProcessed;
    }

    void OnDisable()
    {
        GameDataManager.TransactionProcessed -= OnTransactionProcessed;
        //GameDataManager.RewardProcessed -= OnRewardProcessed;
    }
    ParticleSystem GetFXPool(ShopItemType itemType)
    {
        MagnetData magnetData = m_MagnetData.Find(x => x.ItemType == itemType);
        return magnetData.FXPool;
    }
    void PlayPooledFX(Vector2 screenPos, ShopItemType contentType)
    {
        Vector3 worldPos = screenPos.ScreenPosToWorldPos(m_Camera, m_ZDepth) + m_SourceOffset;

        ParticleSystem fxPool = GetFXPool(contentType);
        var pfx = Poolable.TryGetPoolable<ParticleSystem>(fxPool.gameObject);
        if (pfx == null)
            return;

        pfx.gameObject.SetActive(true);
        pfx.gameObject.transform.position = worldPos;
        pfx.Play();

    }
    void OnTransactionProcessed(ShopItemSO shopItem)
    {
        // only play effect for gold or gem purchases
        if (shopItem.contentType == ShopItemType.HealthPotion || shopItem.contentType == ShopItemType.LevelUpPotion)
            return;

        //PlayPooledFX(screenPos, shopItem.contentType);
    }

}
