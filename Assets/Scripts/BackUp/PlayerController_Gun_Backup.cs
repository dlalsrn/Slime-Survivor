using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController_Gun_Backup : MonoBehaviour
{
    // Component Variable
    private Animator animator;
    private Animator overlayAnimator; // 팔의 Animator
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer spriteRenderer_Overlay;
    private HPBar hpBar;

    // Overlay Variable
    [SerializeField] private GameObject overlay;

    // Gun Variable
    [SerializeField] private GameObject gun;

    // Move Variable
    private float moveSpeed = 5f; // Player 이동속도
    private float xdir; // Player의 x 방향
    private float ydir; // Player의 y 방향

    // HP Variable
    [SerializeField] private float maxHP;
    private float hp;

    // Shooting Variable
    [SerializeField] private Bullet[] bulletPrefabs; // 사용할 수 있는 총알의 종류 (index가 높을 수록 데미지가 높은 총알)
    [SerializeField] private Transform firePos; // 총알이 발사되는 위치
    [SerializeField] private GameObject muzzleFlash;
    private int bulletIndex = 0; // 처음에는 가장 낮은 단계의 총알
    private float originShootInterval = 0.2f; // 발사 간격
    private float shootInterval; // 발사 간격
    private float feverShootInterval = 2; // 공격속도 배수
    private float bulletMoveSpeed = 7f; // 총알 속도
    private Coroutine shootingCoroutine; // 총알 발사 루틴

    // Hit Variable
    private Color originColor; // Player의 원래 Color
    [SerializeField] private Color hitColor; // Player의 Hit 시 Color
    private Color originColor_Overlay; // Overlay의 원래 Color
    [SerializeField] private Color hitColor_Overlay; // Overlay의 Hit 시 Color

    // Item Variable
    private float itemBreadHeal = 30f;
    private float feverTime = 5f;

    private void Start() {
        animator = GetComponent<Animator>();
        overlayAnimator = overlay.GetComponent<Animator>();   
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer_Overlay = overlay.GetComponent<SpriteRenderer>();
        hpBar = GetComponentInChildren<HPBar>();
        originColor = spriteRenderer.color;
        originColor_Overlay = spriteRenderer_Overlay.color;
        hp = maxHP;
        shootInterval = originShootInterval;
    }

    private void Update() {
        if (GameManager.instance.GetIsGameOver()) {
            return;
        }

        ydir = Input.GetAxisRaw("Vertical"); // W/S or 상하 방향키 입력 시 (-1, 0, 1)
        xdir = Input.GetAxisRaw("Horizontal"); // A/D or 좌우 방향키 입력 시 (-1, 0, 1)

        if (Input.GetMouseButtonDown(0)) {
            StartShootingRoutine();
        }
        
        if (Input.GetMouseButtonUp(0)) {
            StopShootingRoutine();
        }

        // Player 움직임
        Vector3 moveDir = new Vector3(xdir, ydir, 0).normalized; // (1, 1)의 경우 대각선 속도가 더 빨라지므로 normalized
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        
        // Player의 방향
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mousePos - transform.position; // 마우스가 있는 쪽으로 Player의 방향 전환, Mathf.Sign(value)는 value > 0 이면 1, < 0 이면 -1, 0이면 0
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(dir.x) * -1f, transform.localScale.y, transform.localScale.z);
        
        // HPBar의 방향
        Vector3 currentHPBarScale = hpBar.transform.localScale; // Player의 방향이 바뀌어도 HPBar는 방향이 바뀌면 안 됨.
        hpBar.transform.localScale = new Vector3(Mathf.Abs(currentHPBarScale.x) * Mathf.Sign(dir.x) * -1f, currentHPBarScale.y, currentHPBarScale.z);

        // Animator 전환
        bool isMoving = (xdir != 0 || ydir != 0);
        animator.SetBool("isMoving", isMoving); // 어느 한쪽 방향이라도 움직이고 있으면
        overlayAnimator.SetBool("isMoving", isMoving); // 어느 한쪽 방향이라도 움직이고 있으면
    }

    private void StartShootingRoutine() {
        shootingCoroutine = StartCoroutine(ShootingBullet());
        muzzleFlash.SetActive(true);
    }

    private void StopShootingRoutine() {
        StopCoroutine(shootingCoroutine);
        muzzleFlash.SetActive(false);
    }

    IEnumerator ShootingBullet() {
        while (true) {
            SpawnBullet();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    private void SpawnBullet() {
        // bullet 생성
        Bullet bullet = Instantiate<Bullet>(bulletPrefabs[bulletIndex], firePos.position, Quaternion.identity);

        // bullet 방향 설정
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // 혹시나 z축이 달라서 맵에서 사라지는 경우 방지
        Vector3 dir = (mousePos - firePos.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180f; // 총알과 마우스 사이의 각도 계산, 총알이 기본적으로 왼쪽으로 향하므로 180도 보정
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle); // 마우스 방향으로 z축 회전
        
        // bullet 속도 설정
        bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletMoveSpeed;
    }

    public void GetDamage(float damage) {
        hp -= damage;

        if (hp <= 0) {
            GameManager.instance.SetGameOver(false); // 이기지 못했으므로 false 전달
            GameFail();
        } else {
            ChangeColor(); // 피격 시 색상 변경
            Invoke("ResetColor", 0.1f); // 충돌이 끝났을 때 0.1초 후 원래 색으로 변경
        }

        hpBar.SetHP(hp, maxHP);
    }

    private void GameFail() {
        animator.SetTrigger("isDead");
        overlay.SetActive(false);
        gun.SetActive(false);
        Destroy(gameObject, 1.5f);
    }

    public void GameWin() {
        animator.SetBool("isMoving", false);
        overlayAnimator.SetBool("isMoving", false);
        StopShootingRoutine();
    }

    private void ChangeColor() { 
        spriteRenderer.color = hitColor;
        spriteRenderer_Overlay.color = hitColor_Overlay;
    }

    private void ResetColor() { 
        spriteRenderer.color = originColor;
        spriteRenderer_Overlay.color = originColor_Overlay;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("ItemBread")) {
            GetItemBread();
            Destroy(collider.gameObject);
        } else if (collider.CompareTag("ItemCrystal")) {
            GetItemCrystal();
            Destroy(collider.gameObject);
        } else if (collider.CompareTag("ItemBook")) {
            GetItemBook();
            Destroy(collider.gameObject);
        }
    }

    private void GetItemBread() { // Bread를 먹었을 경우 Heal
        hp = Mathf.Min(maxHP, hp + itemBreadHeal);
        hpBar.SetHP(hp, maxHP);
    }

    private void GetItemCrystal() { // Crystal을 먹었을 경우 일정시간 동안 공격속도 증가
        GameManager.instance.SetFeverMode(true);
        shootInterval /= feverShootInterval;
        CancelInvoke("ResetShootInterval"); // FeverMode 중에 Crystal을 또 먹었을 경우 FeverTime 갱신
        Invoke("ResetShootInterval", feverTime);
    }

    private void ResetShootInterval() { // 원래 발사 속도로 변경
        shootInterval = originShootInterval;
        GameManager.instance.SetFeverMode(false);
    }

    private void GetItemBook() { // Book을 먹었을 경우 총알 Upgrade
        if (bulletIndex < bulletPrefabs.Length - 1) {
            bulletIndex++;
        }
    }
}