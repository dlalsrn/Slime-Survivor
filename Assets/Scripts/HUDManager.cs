using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance {get; private set;} = null;

    [SerializeField] private GameObject gameWinPanel;
    [SerializeField] private GameObject gameFailPanel;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        DisableAllPanel();
    }
    
    public void DisableAllPanel() {
        gameWinPanel.SetActive(false);
        gameFailPanel.SetActive(false);
    }

    public void ShowGameWinPanel() {
        gameWinPanel.SetActive(true);
    }

    public void ShowGameFailPanel() {
        gameFailPanel.SetActive(true);
    }
}
