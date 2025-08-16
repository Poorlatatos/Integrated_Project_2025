using UnityEngine;
using System.Collections.Generic;
public class RandomItemSpawner : MonoBehaviour
{
    /*
    * Author: Jaasper Lee Zong Hng
    * Date: 3/08/2025
    * Description: Random item spawner script for Unity
      Spawns random items at designated spawn points in the game world.
    */

    [Header("Assign all possible item prefabs here")]
    public GameObject[] itemPrefabs; /// Assign item prefabs in Inspector

    [Header("Assign all possible spawn points here")]
    public Transform[] spawnPoints; /// Assign spawn points in Inspector

    [Header("How many items to spawn? (<= spawnPoints.Length)")]
    public int itemsToSpawn = 5; /// How many items to spawn

    void Start()
    {
        SpawnRandomItems();
    }

    void SpawnRandomItems() /// Spawn random items at designated spawn points
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
        List<GameObject> spawnedItems = new List<GameObject>();
        for (int i = 0; i < Mathf.Min(itemsToSpawn, shuffledPoints.Length, shuffledItems.Length); i++)
        {
            GameObject spawned = Instantiate(shuffledItems[i], shuffledPoints[i].position, shuffledPoints[i].rotation);
            spawned.name = shuffledItems[i].name;
            spawnedItems.Add(spawned);
        }

        // Notify ChecklistManager about the spawned items
        FindFirstObjectByType<ChecklistManager>()?.PopulateChecklist(spawnedItems.ToArray());
    }
    
}