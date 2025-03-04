using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Component Variable
    private HPBar hpBar;

    // Move Variable
    private float moveSpeed = 5f; // Player 이동속도
    private float xdir; // Player의 x 방향
    private float ydir; // Player의 y 방향
    protected bool isMoving;

    // HP Variable
    [SerializeField] protected float maxHP;
    protected float hp;

    // Item Variable
    private float itemBreadHeal = 30f;
    protected float feverTime = 5f;

    protected virtual void Start() {
        hpBar = GetComponentInChildren<HPBar>();
        hp = maxHP;
    }

    protected virtual void Update() {
        ydir = Input.GetAxisRaw("Vertical"); // W/S or 상하 방향키 입력 시 (-1, 0, 1)
        xdir = Input.GetAxisRaw("Horizontal"); // A/D or 좌우 방향키 입력 시 (-1, 0, 1)

        Move(); // Player 움직임
        Flip(); // Player 방향 전환
        
        // Animator 전환
        isMoving = (xdir != 0 || ydir != 0);
    }

    private void Move() {
        // Player 움직임
        Vector3 moveDir = new Vector3(xdir, ydir, 0).normalized; // (1, 1)의 경우 대각선 속도가 더 빨라지므로 normalized
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void Flip() {
        // Player의 방향
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mousePos - transform.position; // 마우스가 있는 쪽으로 Player의 방향 전환, Mathf.Sign(value)는 value > 0 이면 1, < 0 이면 -1, 0이면 0
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(dir.x) * -1f, transform.localScale.y, transform.localScale.z);
        
        // HPBar의 방향
        Vector3 currentHPBarScale = hpBar.transform.localScale; // Player의 방향이 바뀌어도 HPBar는 방향이 바뀌면 안 됨.
        hpBar.transform.localScale = new Vector3(Mathf.Abs(currentHPBarScale.x) * Mathf.Sign(dir.x) * -1f, currentHPBarScale.y, currentHPBarScale.z);
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
    
    protected virtual void GameFail() {
    }

    public virtual void GameWin() {
    }

    protected virtual void ChangeColor() {
    }

    protected virtual void ResetColor() { 
    }

    protected virtual void GetItemCrystal() { // Crystal을 먹었을 경우 일정시간 동안 공격속도 증가
    }

    protected virtual void GetItemBook() { // Book을 먹었을 경우 Upgrade
    }
}
