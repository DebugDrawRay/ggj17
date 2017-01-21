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
    public Vector2 spawnRange;
    public float refillRadius;
    public Vector2 scaleMatchRange;

    public Transform spawnOrigin;
    public static OceanBodySpawner instance;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        spawnOrigin = WaveStatusController.instance.transform;

        SpawnObjects(enemyWave, Random.Range(enemySpawnRange.min, enemySpawnRange.max));
        SpawnObjects(flotsam, Random.Range(floatsamSpawnRange.min, floatsamSpawnRange.max));
        SpawnObjects(obstacles, Random.Range(obstacleSpawnRange.min, obstacleSpawnRange.max));
    }

    void SpawnObjects(GameObject spawn, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = GetRandomPosition(spawnOrigin.position);
            GameObject newEnemy = Instantiate(spawn, pos, Quaternion.identity);
            Vector3 scale = AddRandomScale(WaveStatusController.instance.transform.localScale);
            newEnemy.transform.localScale = scale;
        }
    }

    void SpawnObjects(GameObject[] spawns, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            int select = Random.Range(0, spawns.Length);
            GameObject selected = spawns[select];
            Vector3 pos = GetRandomPosition(spawnOrigin.position);
            Instantiate(selected, pos, Quaternion.identity);
        }
    }

    public void RefillEnemies()
    {
        Vector3 pos = GetRandomPosition(spawnOrigin.position);
        GameObject newEnemy = Instantiate(enemyWave, pos, Quaternion.identity);
        Vector3 scale = AddRandomScale(WaveStatusController.instance.transform.localScale);
        newEnemy.transform.localScale = scale;
    }

    Vector3 GetRandomPosition(Vector3 center)
    {
        float dist = Random.Range(spawnRange.x, spawnRange.y);
        Vector3 point = new Vector3(dist, 0, 0);

        Vector3 dir = point - center;
        point = Quaternion.Euler(0, Random.Range(0, 360f), 0) * (point - center) + center;
        return point;
    }

    Vector3 AddRandomScale(Vector3 current)
    {
        float scaleMod = Random.Range(-scaleMatchRange.x, scaleMatchRange.y);
        scaleMod = Mathf.Clamp(scaleMod, .1f, Mathf.Infinity);
        current.x += scaleMod;
        current.y += scaleMod;
        current.z += scaleMod;
        return current;
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
