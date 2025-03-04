using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefabs;
    
    [SerializeField] private float damage;

    private void Start() {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (GameManager.instance.GetIsGameOver()) {
            return;
        }
        
        if (collider.CompareTag("Enemy") || collider.CompareTag("Boss")) { // 적이랑 충돌 시
            collider.GetComponent<Enemy>().GetDamage(damage);
            CreateExlposion();
            Destroy(gameObject);
        } else if (collider.CompareTag("Obstacle")) { // 장애물이랑 충돌 시 총알 삭제
            Destroy(gameObject);
        }
    }

    private void CreateExlposion() {
        GameObject explosion = Instantiate<GameObject>(explosionPrefabs, transform.position, Quaternion.identity);
        Destroy(explosion, 0.3f);
    }
}
