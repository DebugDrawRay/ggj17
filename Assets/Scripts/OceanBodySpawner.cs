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

    [Header("Body Params")]
    public float bodySpawnRadius;
    public Transform bodySpawnCenter;

    public Transform spawnOrigin;

    [Header("Enemy Params")]
    //public IntRange enemySpawnCountRange;
    public Vector2 initialEnemySpawnRadius;
	public IntRange minMaxEnemies;
	public float maxEnemySize = 60;
    public Vector2 newEnemySpawnRadius;
    public float spawnRangeMod;
    public float spawnTimeMod;
    public Vector2[] scaleLevels;
    public Vector2 timeToRefillRange;
    private float currentTimeToRefill;

	private int maxEnemies
	{
		get
		{
			if (WaveStatusController.instance != null)
			{
				int count = Mathf.Clamp(Mathf.CeilToInt(minMaxEnemies.max / (WaveStatusController.instance.scale * 0.5f)), minMaxEnemies.min, minMaxEnemies.max);
				return count;
			}
			else
			{
				return minMaxEnemies.max;
			}
		}
	}

    private GameObject enemyContainer;
    public static OceanBodySpawner instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        spawnOrigin = WaveStatusController.instance.transform;
        enemyContainer = new GameObject("Enemies");
        SpawnEnemies(enemyWave, minMaxEnemies.max, initialEnemySpawnRadius, 1);
		SpawnObjects(flotsamSpawnGroups);
	}

    void Update()
    {
        RefillEnemies();
    }

    protected void SpawnEnemies(GameObject spawn, int amount, Vector2 range, float rangeMod)
    {
		if (spawnOrigin != null)
		{
			for (int i = 0; i < amount; i++)
			{
				SpawnEnemy(spawn, range, rangeMod);
			}
		}
    }

	public void RefillEnemies()
	{
		if (GameController.instance != null && GameController.instance.currentState == GameController.State.InGame)
		{
			if (WaveStatusController.instance != null)
			{
				if (currentTimeToRefill > 0)
				{
					currentTimeToRefill -= Time.deltaTime;
				}
				else
				{
					if (enemyContainer.transform.childCount < maxEnemies)
						SpawnEnemy(enemyWave, newEnemySpawnRadius, spawnRangeMod);

					currentTimeToRefill = Random.Range(timeToRefillRange.x, timeToRefillRange.y);
				}
			}
		}
	}

	protected void SpawnEnemy(GameObject spawn, Vector2 range, float rangeMod)
	{
		Vector3 pos = GetRandomEnemyPosition(spawnOrigin.position, range);
		GameObject newEnemy = Instantiate(spawn, pos, Quaternion.identity);
		Vector3 scale = AddRandomScale(spawnOrigin.localScale);
		newEnemy.transform.localScale = scale;
		newEnemy.transform.SetParent(enemyContainer.transform);
	}

	protected void SpawnObjects(FlotsamSpawn[] spawns)
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

	protected Vector3 GetRandomEnemyPosition(Vector3 center, Vector2 range)
    {
		float scale = WaveStatusController.instance == null ? 1 : WaveStatusController.instance.scale;
		float min = 50 + (scale * 5);
		float max = Mathf.Min(1000, 150 + (scale * 10));

        float dist = Random.Range(min, max);
        Vector3 point = new Vector3(dist, 0, 0) + center;
        point = Quaternion.Euler(0, Random.Range(0, 360f), 0) * (point - center) + center;
        return point;
    }

	protected Vector3 GetRandomPosition()
    {
        float dist = Random.Range(0, bodySpawnRadius);
        Vector3 point = new Vector3(dist, 0, 0) + bodySpawnCenter.position;
        point = Quaternion.Euler(0, Random.Range(0, 360f), 0) * (point - bodySpawnCenter.position) + bodySpawnCenter.position;
        return point;
    }

	protected Vector3 AddRandomScale(Vector3 current)
    {
        float totalWeights = 0;
        foreach(Vector2 scale in scaleLevels)
        {
            totalWeights += scale.x;
        }
        float ran = Random.Range(0, totalWeights);
        int index = -1;
        for(int i = 0; i< scaleLevels.Length; i++)
        {
            if(ran <= scaleLevels[i].x)
            {
                index = i;
                break;
            }
            ran -= scaleLevels[i].x;
        }
        Vector2 selected = scaleLevels[index];
        current.x = Mathf.Min(maxEnemySize, current.x * selected.y);
		current.y = Mathf.Min(maxEnemySize, current.y * selected.y);
		current.z = Mathf.Min(maxEnemySize, current.z * selected.y);

        return current;
    }
}

[System.Serializable]
public class FlotsamSpawn
{
	public GameObject flotsam;
	[SerializeField]
	public IntRange range;
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
