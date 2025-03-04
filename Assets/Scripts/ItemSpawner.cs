using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    // Item Spawn Variable
    [SerializeField] private GameObject[] itemPrefabs;
    private float[] itemWeights = {0.5f, 0.3f, 0.2f}; // 각 Item이 등장할 확률
    [SerializeField] private Transform[] spawnPoints;
    private Coroutine itemSpawnCoroutine;
    private float itemSpawnInterval = 7f; // 아이템 생성 주기
    private float existItemTime = 6.9f; // 아이템이 생성되고 삭제될 때까지 필드에 존재하는 시간

    public void StartItemSpawn() {
        itemSpawnCoroutine = StartCoroutine(ItemSpawnRoutine());
    }

    public void StopItemSpawn() {
        StopCoroutine(itemSpawnCoroutine);
    }

    IEnumerator ItemSpawnRoutine() {
        yield return new WaitForSeconds(5f);
        while (true) {
            SpawnItem();
            yield return new WaitForSeconds(itemSpawnInterval);
        }
    }

    private void SpawnItem() {
        int itemIndex = GetWeightRandomIndex(); // 확률에 기반하여 RandomIndex 설정
        int posIndex = Random.Range(0, spawnPoints.Length);
        GameObject item = Instantiate<GameObject>(itemPrefabs[itemIndex], spawnPoints[posIndex].position, Quaternion.identity);
        Destroy(item, existItemTime);
    }

    private int GetWeightRandomIndex() { // 각 Item의 확률 구간에서 랜덤한 지점에 해당하는 Item이 무엇인지 return
        int totalWeight = 0;
        foreach (float weight in itemWeights) {
            totalWeight += (int)(weight * 100f); // 소수점 제거, 모두 더하면 100
        }

        int randomIndex = Random.Range(0, totalWeight); // 모든 확률 구간 중 랜덤 Index 설정
        int sum = 0;

        for (int i = 0; i < itemWeights.Length; i++) {
            sum += (int)(itemWeights[i] * 100f);
            if (randomIndex < sum) { // 랜덤 Index가 해당 Item 확률 구간에 있는지
                return i;
            }
        }

        return 0; // 기본 값
    }
}
