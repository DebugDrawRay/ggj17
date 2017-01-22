using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatsamController : MonoBehaviour
{
	public float collectThreshold;

	protected Collider theCollider;
	protected Rigidbody theRigidBody;

	protected WaveStatusController attachedWave;
	protected GameObject attachPoint;
	protected float speed = 50;

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
			transform.position = Vector3.MoveTowards(transform.position, attachPoint.transform.position, speed * Time.deltaTime);
			transform.rotation = attachPoint.transform.rotation;
			
			if (attachedWave.scale < collectThreshold)
			{
				Debug.Log("DETATCH!");
				Destroy(attachPoint);
				attachPoint = null;
				theRigidBody.isKinematic = false;
				theRigidBody.useGravity = true;
			}
		}

		if (transform.position.y < -10)
		{
			Destroy(gameObject);
		}
	}

	public void AttachToWave(WaveStatusController waveController, GameObject waveAttachPoint)
	{
		attachedWave = waveController;
		attachPoint = waveAttachPoint;
		Debug.Log("Wave Attach:" + waveAttachPoint.name);
		theCollider.enabled = false;
	}
}
