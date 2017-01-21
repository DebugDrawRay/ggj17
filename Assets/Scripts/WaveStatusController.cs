using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveStatusController : MonoBehaviour
{
	public Collider thisCollider;

	[Header("Wave Properties")]
	public GameObject[] waveDisplays;
	public float[] waveChangeThresholds;
	protected int currentWaveThreshold = 0;
    public float deathThreshold;

    public float scaleSpeed = 1;
    public float negateAmount = 0.5f;
    public float scaleDecayRate = 0.05f;

    [Header("Collectibles")]
	public Transform CollectibleParent;

	[HideInInspector]
	public float scale = 1f;

    public static WaveStatusController instance;

	void Awake()
	{
        instance = this;
	}

	void Update()
	{
		//Scale Down over time
		scale -= scaleDecayRate * Time.deltaTime;

		//Move scale of wave toward desired scale
		transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(scale, scale, scale), scaleSpeed * Time.deltaTime);

		if (currentWaveThreshold < waveChangeThresholds.Length - 1 && scale > waveChangeThresholds[currentWaveThreshold + 1])
		{
			waveDisplays[currentWaveThreshold + 1].SetActive(true);
			waveDisplays[currentWaveThreshold].SetActive(false);
			currentWaveThreshold++;
		}
		else if(currentWaveThreshold > 0 && scale < waveChangeThresholds[currentWaveThreshold])
		{
			waveDisplays[currentWaveThreshold - 1].SetActive(true);
			waveDisplays[currentWaveThreshold].SetActive(false);
			currentWaveThreshold--;
		}

		if (transform.localScale.x <= deathThreshold)
		{
            //Death thing also here
            Debug.Log("You died, loser :c");
            Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		EnemyWaveController waveController = collider.gameObject.GetComponent<EnemyWaveController>();
		if (waveController != null)
		{
			//If you're bigger than the enemy wave
			if (scale > waveController.scale)
			{
				//Eat enemy wave
				scale += waveController.scale;
			}
			else
			{
				//take damage based on wave scale
				scale -= waveController.scale * negateAmount;
			}

			//Call for enemy wave destruction
			//TEMP CODE
			Destroy(waveController.gameObject);
            OceanBodySpawner.instance.RefillEnemies();
        }

        FloatsamController floatsamController = collider.gameObject.GetComponent<FloatsamController>();
        if (floatsamController != null)
        {
            //If wave is big enough to pick it up
            if (scale > floatsamController.collectThreshold)
            {
                //Mesh currentMesh = waveDisplays[currentWaveThreshold].GetComponent<MeshFilter>().mesh;
                RaycastHit hit = GetPointOnMesh();
                Vector3 position = hit.point;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                GameObject attachPoint = new GameObject("AttachPoint");
                attachPoint.transform.position = position;
                attachPoint.transform.rotation = rotation;
                attachPoint.transform.SetParent(CollectibleParent);

                floatsamController.AttachToWave(this, attachPoint);
            }
        }
        ObstacleController obstacle = collider.gameObject.GetComponent<ObstacleController>();
        if(obstacle)
        {
            scale = deathThreshold;
            //Death thing here
        }
	}

	protected RaycastHit GetPointOnMesh()
	{
		float length = 100f;
		float angleInRad = Random.Range(0f, 90f) * Mathf.Deg2Rad;
		Vector2 pointOnCircle = (Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
		Vector3 direction = new Vector3(pointOnCircle.y, Mathf.Cos(angleInRad), pointOnCircle.x);

		Ray ray = new Ray(transform.position + direction * length, -direction);
		RaycastHit hit;
		thisCollider.Raycast(ray, out hit, length * 2);
		return hit;
	}
}
