using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private PolygonCollider2D polygonCollider2d;
    
    private float originDamage; // 원래의 Sword Damage
    private float damageIncreaseRate = 0.1f; // 책 Item 먹었을 시 Sword Damage 상승률
    [SerializeField] private float damage; // 현재 Sword Damage

    private HashSet<Collider2D> alreadyHitEnemies = new HashSet<Collider2D>();

    private void Start() {
        polygonCollider2d = GetComponent<PolygonCollider2D>();
        originDamage = damage;
    }

    public void EnableSwordCollsion() {
        alreadyHitEnemies.Clear();
        polygonCollider2d.enabled = true;
    }

    public void DisableSwordCollsion() {
        polygonCollider2d.enabled = false;
        alreadyHitEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if ((collider.CompareTag("Enemy") || collider.CompareTag("Boss")) && !alreadyHitEnemies.Contains(collider)) {
            collider.GetComponent<Enemy>().GetDamage(damage);
            alreadyHitEnemies.Add(collider); // 충돌한 적 기록
        }
    }

    public void UpgradeDamage() {
        if (damage < originDamage * 2) { // 업그레이드를 통해 원래 데미지의 2배는 넘지 못하게
            damage += originDamage * damageIncreaseRate; // 첫 데미지의 10%만큼 증가
        }
    }
}
