using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionController : MonoBehaviour
{
	protected Vector3 forceOrigin;
	protected float destructionLifespan;

	protected List<Collider> alreadyThrown;

	void Awake()
	{
		alreadyThrown = new List<Collider>();
	}

	public void StartInflation(float speed, float lifespan)
	{
		destructionLifespan = lifespan;
		StartCoroutine(StartInflationWork(speed, lifespan));
		forceOrigin = transform.position;
		forceOrigin.y = -100;
	}

	protected IEnumerator StartInflationWork(float speed, float lifespan)
	{
		float time = 0;
		float s = 0;
		while (time < lifespan)
		{
			transform.localScale = new Vector3(s, s, s);
			s += Time.deltaTime * speed;
			time += Time.deltaTime;
			yield return null;
		}

		Debug.Log("DESTRUCTION OVER");
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider collider)
	{
		collider.gameObject.layer = LayerMask.NameToLayer("TossedObjects");
		Rigidbody rigidBody = collider.gameObject.GetComponent<Rigidbody>();

		if (rigidBody != null)
		{
			rigidBody.isKinematic = false;
			rigidBody.useGravity = true;

			//Vector3 forcePoint = transform.position + ((collider.gameObject.transform.position - transform.position) * 0.5f);
			//forcePoint.y -= 200f;

			//rigidBody.AddForce((collider.gameObject.transform.position - forcePoint) * 80f);
			//rigidBody.AddTorque(Vector3.left * 50);
		}
		else
			Debug.Log("Attached Rigid Body is Null");
	}
}
