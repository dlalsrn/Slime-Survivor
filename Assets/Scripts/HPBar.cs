using UnityEngine;

public class HPBar : MonoBehaviour
{
    private Transform HPGreen;
    private Vector3 originalScale;

    void Start() {
        HPGreen = transform.Find("HPGreen");
        originalScale = HPGreen.localScale;
    }

    public void SetHP(float hp, float maxHP) {
        float scale = Mathf.Clamp(hp / maxHP, 0, 1); // 현재 체력의 비율
        HPGreen.localScale = new Vector3(originalScale.x * scale, originalScale.y, originalScale.z);
    }
}
