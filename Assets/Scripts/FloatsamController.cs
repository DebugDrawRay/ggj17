using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatsamController : MonoBehaviour
{
	public float collectThreshold;
	public int pickupScore;

	protected Collider theCollider;
	protected Rigidbody theRigidBody;

	protected WaveStatusController attachedWave;
	protected GameObject attachPoint;
	protected float speed = 10;
	protected float speedIncrament = 50;
	protected bool attached = false;

    public SteeringMovement move;

	void Awake()
	{
		theCollider = gameObject.GetComponent<Collider>();
		theRigidBody = gameObject.GetComponent<Rigidbody>();
	}

    void Start()
    {
        float startRot = Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(0, startRot, 0);
    }

    void Update()
	{
		if (attachPoint != null)
		{
			if (!attached)
			{
				transform.position = Vector3.MoveTowards(transform.position, attachPoint.transform.position, speed * Time.deltaTime);
				transform.rotation = attachPoint.transform.rotation;
				speed += Time.deltaTime * speedIncrament;

				if (transform.position == attachPoint.transform.position)
					attached = true;
			}
			else
			{
				transform.position = attachPoint.transform.position;
				transform.rotation = attachPoint.transform.rotation;
			}
			
			if (attachedWave.scale < collectThreshold)
			{
				if (GameController.instance != null)
				{
					GameController.instance.RemoveFromScore(pickupScore);
					GameController.instance.DecramentFloatsam();
				}
				Detach();
			}
		}

		if (attached && attachPoint == null)
		{
			theCollider.enabled = true;
			Detach();
		}

		if (transform.position.y < -10)
		{
			Destroy(gameObject);
		}
	}

	protected void Detach()
	{
		Destroy(attachPoint);
		attachPoint = null;
		theRigidBody.isKinematic = false;
		theRigidBody.useGravity = true;
	}

	public void AttachToWave(WaveStatusController waveController, GameObject waveAttachPoint)
	{
		Debug.Log("Attach to wave: " + gameObject.name);
		attachedWave = waveController;
		attachPoint = waveAttachPoint;
		theCollider.enabled = false;

		if (GameController.instance != null)
		{
			GameController.instance.AddToScore(pickupScore);
			GameController.instance.IncramentFloatsam();
		}
	}
}
