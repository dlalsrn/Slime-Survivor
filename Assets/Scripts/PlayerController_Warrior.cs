using System.Collections;
using UnityEngine;

public class PlayerController_Warrior : PlayerController
{
    // Component Variable
    private Animator animator;
    private Animator overlayAnimator; // 팔의 Animator
    private Animator swordAnimator; // 검의 Animator
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer spriteRenderer_Overlay;

    // Overlay Variable
    [SerializeField] private GameObject overlay;

    // sword Variable
    [SerializeField] private GameObject sword;

    // Attack Variable
    private float feverAttackInterval = 2f; // 공격속도 배수

    // Hit Variable
    private Color originColor; // Player의 원래 Color
    private Color originColor_Overlay; // Overlay의 원래 Color
    [SerializeField] private Color hitColor; // Player의 Hit 시 Color
    [SerializeField] private Color hitColor_Overlay; // Overlay의 Hit 시 Color

    protected override void Start() {
        base.Start();

        animator = GetComponent<Animator>();
        overlayAnimator = overlay.GetComponent<Animator>();
        swordAnimator = sword.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer_Overlay = overlay.GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
        originColor_Overlay = spriteRenderer_Overlay.color;
        hp = maxHP;
    }

    protected override void Update() {
        if (GameManager.instance.GetIsGameOver()) {
            return;
        }

        base.Update();

        if (Input.GetMouseButtonDown(0)) {
            StartAttack();
        }
        
        if (Input.GetMouseButtonUp(0)) {
            StopAttack();
        }

        animator.SetBool("isMoving", isMoving); // 어느 한쪽 방향이라도 움직이고 있으면
        overlayAnimator.SetBool("isMoving", isMoving);
        swordAnimator.SetBool("isMoving", isMoving);
    }

    private void StartAttack() {
        animator.SetBool("isAttack", true);
        overlayAnimator.SetBool("isAttack", true);
        swordAnimator.SetBool("isAttack", true);
    }

    private void StopAttack() {
        animator.SetBool("isAttack", false);
        overlayAnimator.SetBool("isAttack", false);
        swordAnimator.SetBool("isAttack", false);
    }

    protected override void GameFail() {
        animator.SetTrigger("isDead");
        overlay.SetActive(false);
        sword.SetActive(false);
        Destroy(gameObject, 1.5f);
    }

    public override void GameWin() {
        animator.SetBool("isMoving", false);
        overlayAnimator.SetBool("isMoving", false);
        swordAnimator.SetBool("isMoving", false);
        StopAttack();
    }

    protected override void ChangeColor() { 
        spriteRenderer.color = hitColor;
        spriteRenderer_Overlay.color = hitColor_Overlay;
    }

    protected override void ResetColor() { 
        spriteRenderer.color = originColor;
        spriteRenderer_Overlay.color = originColor_Overlay;
    }

    protected override void GetItemCrystal() { // Crystal을 먹었을 경우 일정시간 동안 이동속도, 공격속도 증가
        GameManager.instance.SetFeverMode(true);
        IncreaseAttackSpeed();
        CancelInvoke("ResetAttackSpeed"); // FeverMode 중에 Crystal을 또 먹었을 경우 FeverTime 갱신
        Invoke("ResetAttackSpeed", feverTime);
    }

    protected override void GetItemBook() { // Book을 먹었을 경우 Sword Damage Up
        sword.GetComponent<Sword>().UpgradeDamage();
    }

    private void IncreaseAttackSpeed() {
        animator.speed = feverAttackInterval;
        overlayAnimator.speed = feverAttackInterval;
        swordAnimator.speed = feverAttackInterval;
    }

    private void ResetAttackSpeed() {
        animator.speed = 1f;
        overlayAnimator.speed = 1f;
        swordAnimator.speed = 1f;
        GameManager.instance.SetFeverMode(false);
    }
}