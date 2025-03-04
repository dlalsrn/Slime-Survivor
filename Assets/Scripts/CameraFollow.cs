using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject player;

    // 카메라의 최대 이동거리
    private float maxPosX = 5.2f;
    private float minPosX = -5.2f;
    private float maxPosY = 2.7f;
    private float minPosY = -5.2f;

    void Update() {
        if (player != null) {
            float posX = Mathf.Clamp(player.transform.position.x, minPosX, maxPosX);
            float posY = Mathf.Clamp(player.transform.position.y, minPosY, maxPosY);
            transform.position = new Vector3(posX, posY, -10f);
        }
    }

    public void FollowPlayer() {
        player = FindObjectOfType<PlayerController>().gameObject;
    }
}
