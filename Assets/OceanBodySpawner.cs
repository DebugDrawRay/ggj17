using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanBodySpawner : MonoBehaviour
{
    [Header("Bodies")]
    public GameObject enemyWave;
    public GameObject[] flotsam;
    public GameObject[] obstacles;

    [Header("Ocean")]
    public MeshRenderer ocean;
    public float boundsReduction;

    [Header("Params")]
    [SerializeField]
    public IntRange enemySpawnRange;
    [SerializeField]
    public IntRange floatsamSpawnRange;
    [SerializeField]
    public IntRange obstacleSpawnRange;
    public float spawnRadius;

    void Start()
    {
        SpawnObjects(enemyWave, Random.Range(enemySpawnRange.min, enemySpawnRange.max));
        SpawnObjects(flotsam, Random.Range(floatsamSpawnRange.min, floatsamSpawnRange.max));
        SpawnObjects(obstacles, Random.Range(obstacleSpawnRange.min, obstacleSpawnRange.max));

        Vector2 pos = Random.insideUnitCircle * ocean.bounds.size.x;
    }

    void SpawnObjects(GameObject spawn, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = Random.insideUnitCircle * spawnRadius;
            pos.z = pos.y;
            pos.y = 0;
            pos += transform.position;
            Instantiate(spawn, pos, Quaternion.identity);
        }
    }
    void SpawnObjects(GameObject[] spawns, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            int select = Random.Range(0, spawns.Length);
            GameObject selected = spawns[select];
            Vector3 pos = Random.insideUnitCircle * spawnRadius;
            pos.z = pos.y;
            pos.y = 0;
            pos += transform.position;
            Instantiate(selected, pos, Quaternion.identity);
        }
    }
}

[System.Serializable]
public struct IntRange
{
    public int min, max;

    public IntRange(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}
