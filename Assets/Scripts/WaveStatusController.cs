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
        if (GameController.instance.currentState == GameController.State.InGame)
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
	}

    void OnDeath()
    {
        Tween death = DOTween.To(() => currentFlatness, x => currentFlatness = x, 100, 1);
        death.SetEase(Ease.Linear);
        death.OnComplete(() => { Destroy(gameObject); UIController.instance.DisplayResults(); });
    }

	void OnTriggerEnter(Collider collider)
	{
		//If Enemy Wave
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

			OceanBodySpawner.instance.RefillEnemies();
			waveController.OnDeath();
        }

		//If Floatsam
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

		//If Obstacle
		ObstacleController obstacle = collider.gameObject.GetComponent<ObstacleController>();
		if (obstacle)
		{
			OnDeath();
		}

		//If Shoreline
		if (collider.tag == "Shoreline")
		{
			WaveCrash();
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

	protected void WaveCrash()
	{
		if (GameController.instance != null)
			GameController.instance.SetFinalWaveHeight(scale);

		GameObject destructor = new GameObject("DestructionSpehere");
		destructor.transform.position = transform.position;
		SphereCollider destructorCollider = destructor.AddComponent<SphereCollider>();
		destructorCollider.isTrigger = true;
		Rigidbody destructorRigidbody = destructor.AddComponent<Rigidbody>();
		destructorRigidbody.useGravity = false;
		destructorRigidbody.isKinematic = true;
		DestructionController controller = destructor.AddComponent<DestructionController>();
		controller.StartInflation(scale / 2);

		//Play Death Anim
		Destroy(collectibleParent.gameObject);
		OnDeath();

		
	}
}
