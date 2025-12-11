using CreatorKitCode;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class BoombSkill : MonoBehaviour, ISkill
{
    public ParticleSystem ParticleSystem;     // Hệ thống particle khi bom nổ
    public float explosionRadius = 5f;        // Bán kính nổ của bom
    public int damageAmount = 9;      // Số máu bị trừ
    public LayerMask enemyLayer;              // Layer của kẻ thù (đảm bảo kẻ thù có layer riêng)
    public float explosionForce = 5000f;

    void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra nếu va chạm với một đối tượng có lớp CharacterData (enemy)
        var targetable = collision.gameObject.GetComponent<CharacterData>();
        if (targetable != null)
        {
            // Thực hiện nổ bom và trừ máu cho tất cả kẻ thù trong phạm vi
            Execute();
        }
    }

    // Phương thức thực hiện nổ bom và trừ máu cho kẻ thù trong phạm vi
    public void Execute()
    {
        // Chơi hiệu ứng particle (nổ bom)
        ParticleSystem.Play();

        // Kiểm tra tất cả các collider trong phạm vi nổ của bom
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);

        // Duyệt qua tất cả các kẻ thù trong phạm vi và trừ máu
        foreach (var enemy in hitEnemies)
        {
            var enemyData = enemy.GetComponent<CharacterData>();
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            if (enemyData != null)
            {
                // Trừ máu cho enemy
                //rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                enemyData.damageableBehaviour.Stats.ChangeHealth(-damageAmount);
            }
        }
    }
}
