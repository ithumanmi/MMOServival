using UnityEngine;
using UnityEngine.Serialization;
namespace CreatorKitCode
{
    public class AddHealthUsageEffect : UsableItem.UsageEffect
    {
        [FormerlySerializedAs("HealthPurcentageAmount")]
        public int HealthPercentageAmount = 20;

        public override bool Use(CharacterData user)
        {
            if (user.damageableBehaviour.Stats.CurrentHealth == user.damageableBehaviour.Stats.stats.health)
                return false;

            VFXManager.PlayVFX(VFXType.Healing, user.transform.position);

            user.damageableBehaviour.Stats.ChangeHealth(Mathf.FloorToInt(HealthPercentageAmount / 100.0f * user.damageableBehaviour.Stats.stats.health));

            return true;
        }
    }
}