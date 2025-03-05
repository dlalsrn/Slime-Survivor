using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance {get; private set;} = null;

    // Game Result Panel
    [SerializeField] private GameObject gameWinPanel;
    [SerializeField] private GameObject gameFailPanel;

    // Job Select Panel
    [SerializeField] private GameObject jobSelectPanel;

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
        jobSelectPanel.SetActive(false);
    }

    public void ShowGameWinPanel() {
        gameWinPanel.SetActive(true);
    }

    public void ShowGameFailPanel() {
        gameFailPanel.SetActive(true);
    }

    public void ShowJobSelectPanel() {
        jobSelectPanel.SetActive(true);
    }

    public void HideJobSelectPanel() {
        jobSelectPanel.SetActive(false);
    }
}
