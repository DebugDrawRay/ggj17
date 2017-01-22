using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveStatusController : MonoBehaviour
{
	public Collider thisCollider;

	[Header("Wave Properties")]
    public float deathThreshold;

    public float scaleSpeed = 1;
    public float negateAmount = 0.5f;
    public float scaleDecayRate = 0.05f;

    [Header("Wave Visuals")]
    public SkinnedMeshRenderer skin;
    public int crestBlend;
    public int heightBlend;
    public int flatBlend;
    
    public float crestScale = 10;
    private float currentFlatness = 0;

    [Header("Collectibles")]
	public Transform collectibleParent;
	public Transform randomizer;
	protected float randomizerThreshold = 12;

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

        skin.SetBlendShapeWeight(crestBlend, transform.localScale.x * crestScale);
        skin.SetBlendShapeWeight(flatBlend, currentFlatness);

        if (transform.localScale.x <= deathThreshold)
		{
            OnDeath();
		}
	}

    void OnDeath()
    {
        Tween death = DOTween.To(() => currentFlatness, x => currentFlatness = x, 100, 1);
        death.SetEase(Ease.Linear);
        death.OnComplete(() => Destroy(gameObject));
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
			//Destroy(waveController.gameObject);
			OceanBodySpawner.instance.RefillEnemies();
			waveController.OnDeath();
        }

        FloatsamController floatsamController = collider.gameObject.GetComponent<FloatsamController>();
        if (floatsamController != null)
        {
            //If wave is big enough to pick it up
            if (scale > floatsamController.collectThreshold)
            {
                RaycastHit hit = GetPointOnMesh();
                Vector3 position = hit.point;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                GameObject attachPoint = new GameObject("AttachPoint");
                attachPoint.transform.position = position;
                attachPoint.transform.rotation = rotation;
                attachPoint.transform.SetParent(collectibleParent);

                floatsamController.AttachToWave(this, attachPoint);
            }
        }
        ObstacleController obstacle = collider.gameObject.GetComponent<ObstacleController>();
        if(obstacle)
        {
            OnDeath();
        }
	}

	protected RaycastHit GetPointOnMesh()
	{
		//Randomize position of center;
		Vector3 randomizerPos = randomizer.localPosition;
		randomizerPos.x = Random.Range(-12f, 12f);
		randomizer.localPosition = randomizerPos;

		float length = 100f;
		Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1, 1));
		
		Ray ray = new Ray(randomizer.position + direction * length, -direction);
		RaycastHit hit;
		thisCollider.Raycast(ray, out hit, length * 2);
		return hit;
	}
}
