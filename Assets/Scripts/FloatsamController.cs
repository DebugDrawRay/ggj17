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
	protected bool pickupable = true;

    public SteeringMovement move;
    public Vector2 timeToRotateRange;
    public Vector2 pauseRotateRange;
    private float currentPauseRotate;
    private float currentTimeToRotate;

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
			attached = false;
			theCollider.enabled = true;
			theCollider.isTrigger = false;
			Detach();
		}

		if (transform.position.y < -10)
		{
			Destroy(gameObject);
		}
        UpdateMovement();
	}

    int currentDirection;
    void UpdateMovement()
    {
        if (move)
        {
            int direction = 0;

            if (currentPauseRotate < 0)
            {
                if (currentTimeToRotate > 0)
                {
                    direction = currentDirection;
                    currentTimeToRotate -= Time.deltaTime;
                }
                else
                {
                    if(Random.value >= .5f)
                    {
                        currentDirection = 1;
                    }
                    else
                    {
                        currentDirection = -1;
                    }
                    currentTimeToRotate = Random.Range(timeToRotateRange.x, timeToRotateRange.y);
                    currentPauseRotate = Random.Range(pauseRotateRange.x, pauseRotateRange.y);
                }
            }
            else
            {
                currentPauseRotate -= Time.deltaTime;
            }

            move.MoveDirection(direction);
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
		if (pickupable)
		{
			pickupable = false;
			
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
}
