using System.Reflection;
using UnityEditor.Search;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Component
    private Transform targetTransform;
    private HPBar hpBar;
    private SpriteRenderer spriteRenderer;

    // HP Variable
    [SerializeField] private float collsionDamage; // Player와 충돌했을 때 입히는 damage
    [SerializeField] private float maxHP;
    private float hp;

    // Move Variable
    private float moveSpeed = 0.7f;

    // Hit Variable
    [SerializeField] private Color hitColor;
    private Color originColor;
    private float hitInterval = 0.2f;
    private float lastHitTime = 0;

    // Explosion Variable
    [SerializeField] private GameObject explosionPrefab;

    void Start() {
        hp = maxHP;
        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        hpBar = GetComponentInChildren<HPBar>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
    }

    void Update() {
        if (GameManager.instance.GetIsGameOver()) {
            return;
        }

        Vector3 moveDir = (targetTransform.position - transform.position).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        Vector3 currentScale = transform.localScale;
        Vector3 currentHPBarScale = hpBar.transform.localScale;

        transform.localScale = new Vector3(Mathf.Abs(currentScale.x) * Mathf.Sign(moveDir.x), currentScale.y, currentScale.z);
        // hpBar는 방향에 따라서 xScale이 변하면 안 됨.
        hpBar.transform.localScale = new Vector3(Mathf.Abs(currentHPBarScale.x) * Mathf.Sign(moveDir.x), currentHPBarScale.y, currentHPBarScale.z);
    }

    public void GetDamage(float damage) {
        hp -= damage;
        if (hp <= 0) {
            if (gameObject.CompareTag("Boss")) {
                GameManager.instance.SetGameOver(true);
            }
            DestroySelf();
        } else {
            spriteRenderer.color = hitColor; // 총알에 피격 시 피격 효과를 주기 위해 색상 변경
            Invoke("ResetColor", 0.1f); // 0.1초 뒤에 ResetColor 함수 실행
        }
        hpBar.SetHP(hp, maxHP);
    }

    private void ResetColor() { // 몸체를 원래 색으로 변경
        spriteRenderer.color = originColor;
    }

    private void OnCollisionEnter2D(Collision2D collision) { // 첫 충돌 시
        if (GameManager.instance.GetIsGameOver()) {
            return;
        }

        if (collision.collider.CompareTag("Player")) {
            collision.collider.GetComponent<PlayerController>().GetDamage(collsionDamage);
            lastHitTime = Time.time;
        }
    }

    private void OnCollisionStay2D(Collision2D collision) { // 충돌이 지속되면
        if (GameManager.instance.GetIsGameOver()) {
            return;
        }

        if (collision.collider.CompareTag("Player")) {
            if (Time.time - lastHitTime >= hitInterval) { // 일정 간격으로만 Damage를 입힐 수 있음
                collision.collider.GetComponent<PlayerController>().GetDamage(collsionDamage);
                lastHitTime = Time.time;
            }
        }
    }

    public void DestroySelf() {
        Instantiate<GameObject>(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
