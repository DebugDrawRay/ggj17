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
	protected float speed = 5;
	protected float speedIncrament = 25;
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
        if (GameController.instance.currentState == GameController.State.InGame)
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
                }

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
	}

	public void AttachToWave(WaveStatusController waveController, GameObject waveAttachPoint)
	{
		attachedWave = waveController;
		attachPoint = waveAttachPoint;
		Debug.Log("Wave Attach:" + waveAttachPoint.name);
		theCollider.enabled = false;
	}
}
