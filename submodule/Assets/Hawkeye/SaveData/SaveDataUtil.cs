using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hawkeye.SaveData
{
    public static class SaveDataUtil
    {
        public static string BuildKey(string userId, string dataId)
        {
            return $"{userId}_{dataId}";
        }
    }
}
