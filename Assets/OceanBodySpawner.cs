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
    public float refillRadius;
    public float scaleMatchRange;

    public static OceanBodySpawner instance;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        SpawnObjects(enemyWave, Random.Range(enemySpawnRange.min, enemySpawnRange.max));
        SpawnObjects(flotsam, Random.Range(floatsamSpawnRange.min, floatsamSpawnRange.max));
        SpawnObjects(obstacles, Random.Range(obstacleSpawnRange.min, obstacleSpawnRange.max));
    }

    void SpawnObjects(GameObject spawn, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = Random.insideUnitCircle * spawnRadius;
            pos.z = pos.y;
            pos.y = 0;
            pos += transform.position;
            GameObject newEnemy = Instantiate(spawn, pos, Quaternion.identity);
            float scaleMod = Random.Range(-scaleMatchRange, scaleMatchRange);
            Vector3 scale = WaveStatusController.instance.transform.localScale;
            scale.x += scaleMod;
            scale.y += scaleMod;
            scale.z += scaleMod;
            newEnemy.transform.localScale = scale;
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
    public void RefillEnemies()
    {
        Vector3 pos = Random.insideUnitCircle * refillRadius;
        pos.z = pos.y;
        pos.y = 0;
        pos += WaveStatusController.instance.transform.position;
        GameObject newEnemy = Instantiate(enemyWave, pos, Quaternion.identity);
        float scaleMod = Random.Range(-scaleMatchRange, scaleMatchRange);
        Vector3 scale = WaveStatusController.instance.transform.localScale;
        scale.x += scaleMod;
        scale.y += scaleMod;
        scale.z += scaleMod;
        newEnemy.transform.localScale = scale;
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
