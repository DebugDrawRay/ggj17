using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveStatusController : MonoBehaviour
{
	public Collider thisCollider;

	[Header("Wave Properties")]
    public float deathThreshold;
    public float[] scaleThresholds;
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

    [Header("Audio")]
    public AudioSource movementSource;
    public AudioClip[] movementClips;
    public AudioSource scaleSource;
    public AudioClip[] scaleUpClips;
    public AudioClip[] scaleDownClips;
    public AudioSource collisionSource;
    public AudioClip collisionClip;

    private int previousScale;
    private int nextScale;
    //Controllers
    protected SteeringMovement steeringMovement;

	[HideInInspector]
	public float scale = 1f;
	public float maxScale = 300;

    public static WaveStatusController instance;

	void Awake()
	{
        instance = this;

		steeringMovement = GetComponent<SteeringMovement>();
        previousScale = -1;
        nextScale = 0;
	}

	void Update()
	{
		scale = Mathf.Min(maxScale, scale);
        if (GameController.instance.currentState == GameController.State.InGame)
        {
            //Scale Down over time
            scale -= scaleDecayRate * Time.deltaTime;

            //Move scale of wave toward desired scale
			if (scale < maxScale)
	            transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(scale, scale, scale), Mathf.Max(1, scale * 0.05f) * Time.deltaTime);

            skin.SetBlendShapeWeight(crestBlend, transform.localScale.x * crestScale);
            skin.SetBlendShapeWeight(flatBlend, currentFlatness);

            if (transform.localScale.x <= deathThreshold)
            {
                OnDeath();
            }
            if (!movementSource.isPlaying)
            {
                movementSource.clip = movementClips[0];
                movementSource.Play();
            }
            CheckScale();
        }
	}

    void CheckScale()
    {
        if(nextScale < scaleThresholds.Length && transform.localScale.x > scaleThresholds[nextScale] && nextScale < scaleThresholds.Length)
        {
            AudioObserver.instance.ChangeTrack(nextScale+1);
            movementSource.clip = movementClips[nextScale+1];
            movementSource.Play();
            scaleSource.clip = scaleUpClips[nextScale];
            scaleSource.Play();
            previousScale = nextScale;
            nextScale++;
        }
        if(previousScale < 0 && previousScale >= 0 && transform.localScale.x < scaleThresholds[previousScale])
        {
            AudioObserver.instance.ChangeTrack(previousScale+1);
            movementSource.clip = movementClips[previousScale+1];
            movementSource.Play();
            scaleSource.clip = scaleDownClips[previousScale];
            scaleSource.Play();
            nextScale = previousScale;
            previousScale--;
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
			if (scale > waveController.scale  && scale < maxScale)
			{
				//Eat enemy wave
				scale += waveController.scale;
			}
			else
			{
				//take damage based on wave scale
				scale = transform.localScale.x;
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
