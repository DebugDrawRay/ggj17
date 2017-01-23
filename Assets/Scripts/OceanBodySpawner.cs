using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanBodySpawner : MonoBehaviour
{
    [Header("Bodies")]
    public GameObject enemyWave;
    [SerializeField]
    public FlotsamSpawn[] flotsamSpawnGroups;
    public GameObject[] obstacles;

    [Header("Params")]
    [SerializeField]
    public IntRange enemySpawnRange;
    [SerializeField]
    public IntRange obstacleSpawnRange;
    public Vector2 enemySpawnRadius;
    public float bodySpawnRadius;
    public Transform bodySpawnCenter;
    public float refillRadius;
    public Vector2 scaleMatchRange;

    public Transform spawnOrigin;
    public static OceanBodySpawner instance;

    [System.Serializable]
    public class FlotsamSpawn
    {
        public GameObject flotsam;
        [SerializeField]
        public IntRange range;
    }
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        spawnOrigin = WaveStatusController.instance.transform;

        SpawnObjects(enemyWave, Random.Range(enemySpawnRange.min, enemySpawnRange.max));
        SpawnObjects(flotsamSpawnGroups);
        SpawnObjects(obstacles, Random.Range(obstacleSpawnRange.min, obstacleSpawnRange.max));
    }

    void SpawnObjects(GameObject spawn, int amount)
    {
        GameObject enemies = new GameObject("Enemies");
        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = GetRandomPosition(spawnOrigin.position);
            GameObject newEnemy = Instantiate(spawn, pos, Quaternion.identity);
            Vector3 scale = AddRandomScale(spawnOrigin.localScale);
            newEnemy.transform.localScale = scale;
            newEnemy.transform.SetParent(enemies.transform);
        }
    }

    void SpawnObjects(FlotsamSpawn[] spawns)
    {
        GameObject flotsam = new GameObject("Flotsam");
        for (int i = 0; i < spawns.Length; i++)
        {
            int count = Random.Range(spawns[i].range.min, spawns[i].range.max);
            for(int j = 0; j < count; j++)
            {
                GameObject selected = spawns[i].flotsam;
                Vector3 pos = GetRandomPosition();
                Instantiate(selected, pos, Quaternion.identity).transform.SetParent(flotsam.transform);
            }
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
		if (Random.Range(0f, 1f) < 0.2f)
		{
			Vector3 pos = GetRandomPosition(spawnOrigin.position);
			GameObject newEnemy = Instantiate(enemyWave, pos, Quaternion.identity);
			Vector3 scale = AddRandomScale(spawnOrigin.localScale);
			newEnemy.transform.localScale = scale;
		}
    }

    Vector3 GetRandomPosition(Vector3 center)
    {
        float dist = Random.Range(enemySpawnRadius.x, enemySpawnRadius.y);
        Vector3 point = new Vector3(dist, 0, 0) + center;
        point = Quaternion.Euler(0, Random.Range(0, 360f), 0) * (point - center) + center;
        return point;
    }
    Vector3 GetRandomPosition()
    {
        float dist = Random.Range(0, bodySpawnRadius);
        Vector3 point = new Vector3(dist, 0, 0) + bodySpawnCenter.position;
        point = Quaternion.Euler(0, Random.Range(0, 360f), 0) * (point - bodySpawnCenter.position) + bodySpawnCenter.position;
        return point;
    }

    Vector3 AddRandomScale(Vector3 current)
    {
        float scaleMod = Random.Range(scaleMatchRange.x, scaleMatchRange.y);
        current.x *= scaleMod;
        current.y *= scaleMod;
        current.z *= scaleMod;
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
