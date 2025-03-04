using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void LoadGameScene() {
        SceneManager.LoadScene("Scenes/GameScene");
    }

    public void Quit() {
        Application.Quit();
    }
}
