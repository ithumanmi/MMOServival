using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CoreGameInformation", menuName = "GameInfo/CoreGameInformation")]

public class CoreGameInfomation : ScriptableObject
{
    [System.Serializable]
    public class GlobalAudioInfo
    {
        public AudioClip sfx_OpenPopup;
        public AudioClip sfx_ClickButton;
        public AudioClip sfx_PopupReward;
    }
    [System.Serializable]
    public class DeltaTimeToGetDataFromServer
    {
        [Tooltip("Tính theo phút")] public int deltaTime_GetMailData;
        [Tooltip("Tính theo phút")] public int deltaTime_SendCMD_GetMailData;
        [Tooltip("Tính theo phút")] public int deltaTime_GetSubUserDetail;
        [Tooltip("Tính theo phút")] public int deltaTime_SendCMD_GetSubUserDetail;
        [Tooltip("Tính theo phút")] public int deltaTime_GetFriendData;
        [Tooltip("Tính theo phút")] public int deltaTime_SendCMD_GetFriendData;

        [Tooltip("Tính theo phút")] public int deltaTime_GetShopData;
        [Tooltip("Tính theo phút")] public int deltaTime_SendCMD_GetShopData;

    }
    public SingletonPrefabInfo singletonPrefabInfo;



}
