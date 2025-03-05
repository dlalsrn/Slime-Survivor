using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set;} = null;

    // Job Variable
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private GameObject gunnerPrefab;

    // FeverMode Variable
    private GameObject background;
    private GameObject desert;
    private bool isFeverMode = false;
    private Color originColor = new Color(1f, 1f, 1f);
    private Color feverModeColor = new Color(0.5f, 0.5f, 0.5f);

    // Game Variable
    private bool isGameOver = false;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        background = GameObject.Find("Background");
        desert = GameObject.Find("Desert");
        HUDManager.instance.ShowJobSelectPanel();
    }

    public void SetFeverMode(bool mode) { // false면 FeverMode 종료, true면 FeverMode 
        isFeverMode = mode;
        ChangeColor(background);
        ChangeColor(desert);
    }

    private void ChangeColor(GameObject targetObject) {
        if (targetObject != null) {
            if (isFeverMode) {
                targetObject.GetComponent<SpriteRenderer>().color = feverModeColor;
            } else {
                targetObject.GetComponent<SpriteRenderer>().color = originColor;
            }
        }
    }

    public void SetGameOver(bool isWin) {
        if (!isGameOver) {
            isGameOver = true;

            EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
            if (enemySpawner != null) {
                enemySpawner.StopEnemySpawn();
            }

            ItemSpawner itemSpawner = FindObjectOfType<ItemSpawner>();
            if (itemSpawner != null) {
                itemSpawner.StopItemSpawn();
            }

            if (isWin) { // 보스 처치
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null) {
                    player.GameWin();
                }
                Invoke("DestroyAllEnemies", 1f);
                Invoke("ShowGameWinPanel", 2f);
            } else { // Player 사망
                Invoke("ShowGameFailPanel", 2f);
            }
        }
    }

    private void ShowGameWinPanel() {
        HUDManager.instance.ShowGameWinPanel();
    }

    private void ShowGameFailPanel() {
        HUDManager.instance.ShowGameFailPanel();
    }

    public bool GetIsGameOver() {
        return isGameOver;
    }

    private void DestroyAllEnemies() { // Boss 제거 시 남아있는 모든 Slime 제거
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in enemies) {
            enemy.DestroySelf();
        }
    }

    public void CreateWarrior() {
        Instantiate<GameObject>(warriorPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        GameStart();
    }

    public void CreateGunner() {
        Instantiate<GameObject>(gunnerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        GameStart();
    }

    public void GameStart() {
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null) {
            enemySpawner.StartEnemySpawn();
        }

        ItemSpawner itemSpawner = FindObjectOfType<ItemSpawner>();
        if (itemSpawner != null) {
            itemSpawner.StartItemSpawn();
        }

        HUDManager.instance.HideJobSelectPanel();
        FindObjectOfType<CameraFollow>().FollowPlayer();
    }

    public void LoadGameScene() {
        SceneManager.LoadScene("Scenes/GameScene");
    }

    public void LoadMenuScene() {
        SceneManager.LoadScene("Scenes/MenuScene");
    }
}
