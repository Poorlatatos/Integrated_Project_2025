using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
    [Header("Assign all possible item prefabs here")]
    public GameObject[] itemPrefabs;

    [Header("Assign all possible spawn points here")]
    public Transform[] spawnPoints;

    [Header("How many items to spawn? (<= spawnPoints.Length)")]
    public int itemsToSpawn = 5;

    void Start()
    {
        SpawnRandomItems();
    }

    void SpawnRandomItems()
    {
        // Shuffle spawn points
        Transform[] shuffledPoints = (Transform[])spawnPoints.Clone();
        for (int i = 0; i < shuffledPoints.Length; i++)
        {
            int rand = Random.Range(i, shuffledPoints.Length);
            var temp = shuffledPoints[i];
            shuffledPoints[i] = shuffledPoints[rand];
            shuffledPoints[rand] = temp;
        }

        // Shuffle item prefabs
        GameObject[] shuffledItems = (GameObject[])itemPrefabs.Clone();
        for (int i = 0; i < shuffledItems.Length; i++)
        {
            int rand = Random.Range(i, shuffledItems.Length);
            var temp = shuffledItems[i];
            shuffledItems[i] = shuffledItems[rand];
            shuffledItems[rand] = temp;
        }

        // Spawn items
        for (int i = 0; i < Mathf.Min(itemsToSpawn, shuffledPoints.Length, shuffledItems.Length); i++)
        {
            Instantiate(shuffledItems[i], shuffledPoints[i].position, shuffledPoints[i].rotation);
        }
    }
}