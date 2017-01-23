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
    public float scaleUpMod = 1;
    public float negateAmount = 0.5f;
    public float scaleDecayRate = 0.05f;

    [Header("Wave Visuals")]
    public SkinnedMeshRenderer skin;
    public int crestBlend;
    public int heightBlend;
    public int flatBlend;
	public int crashBlend;

	public float crestScale = 10;
    protected float currentFlatness = 0;
	protected float currentHeight = 0;
	protected float currentCrash = 0;

    public GameObject onHit;

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
    public AudioClip collectClip;

    private int previousScale;
    private int nextScale;
    //Controllers
    protected SteeringMovement steeringMovement;

	protected bool destroyed = false;

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
			skin.SetBlendShapeWeight(heightBlend, currentHeight);
			skin.SetBlendShapeWeight(crashBlend, currentCrash);

            if (transform.localScale.x <= deathThreshold)
            {
                OnDeath();
                AudioObserver.instance.TriggerEndGame(false);
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

    void HitReaction()
    {
        /*GameObject hit = Instantiate(onHit, transform.position, Quaternion.identity);
        Vector3 scale = hit.transform.localScale;
        scale.x *= transform.localScale.x;
        scale.y *= transform.localScale.y;
        scale.z *= transform.localScale.z;
        hit.transform.localScale = scale;*/

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
				scale += waveController.scale * scaleUpMod;
			}
			else
			{
				//take damage based on wave scale
				scale = transform.localScale.x;
				scale -= waveController.scale * negateAmount;
			}
            HitReaction();
			if (collisionSource != null)
			{
				collisionSource.clip = collisionClip;
				collisionSource.Play();
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

				if (collisionSource != null)
				{
					collisionSource.clip = collectClip;
					collisionSource.Play();
				}
            }
            HitReaction();
        }

        //If Obstacle
        ObstacleController obstacle = collider.gameObject.GetComponent<ObstacleController>();
		if (obstacle)
		{
			if (collisionSource != null)
			{
				collisionSource.clip = collisionClip;
				collisionSource.Play();
			}
            HitReaction();
            OnDeath();
		}

		//If Shoreline
		if (collider.tag == "Shoreline" && !destroyed)
		{
			StartCoroutine(WaveCrash());
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

	protected IEnumerator WaveCrash()
	{
		destroyed = true;

        AudioObserver.instance.TriggerEndGame(true);
        if (GameController.instance != null)
			GameController.instance.SetFinalWaveHeight(scale);

		GameObject destructor = new GameObject("DestructionSpehere");
		destructor.transform.position = transform.position;
		destructor.layer = LayerMask.NameToLayer("DestructionSphere");
		SphereCollider destructorCollider = destructor.AddComponent<SphereCollider>();
		destructorCollider.isTrigger = true;
		Rigidbody destructorRigidbody = destructor.AddComponent<Rigidbody>();
		destructorRigidbody.useGravity = false;
		destructorRigidbody.isKinematic = true;
		DestructionController controller = destructor.AddComponent<DestructionController>();
		controller.StartInflation(scale * 5f, 5f);

		//Play Death Anim

		//Raise wave up
		float time = 0;
		float raiseUpTime = 2;	
		while (time < raiseUpTime)
		{
			//Scale Up
			scale += Time.deltaTime * scale;

			//Raise Up
			currentHeight = Mathf.Lerp(0, 100, time / raiseUpTime);

			time += Time.deltaTime;
			yield return null;
		}

		time = 0;
		float crashTime = scale / 100;
		Debug.Log("CRASH TIME: " + crashTime);
		while (time < crashTime)
		{
			//Still Scale Up
			scale += Time.deltaTime * 10;

			currentCrash = Mathf.Lerp(0, 100, time / crashTime);
			currentHeight = Mathf.Lerp(100, 0, time / crashTime);

			time += Time.deltaTime;
			yield return null;
		}

		Destroy(collectibleParent.gameObject);
		OnDeath();

		
	}
}
