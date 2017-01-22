using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionController : MonoBehaviour
{
	protected Vector3 forceOrigin;

	public void StartInflation(float speed, float lifespan)
	{
		StartCoroutine(StartInflationWork(speed, lifespan));
		forceOrigin = transform.position;
		forceOrigin.y -= 1;
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

		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider collider)
	{
		Rigidbody rigidBody = collider.gameObject.GetComponent<Rigidbody>();

		if (rigidBody != null)
		{
			rigidBody.AddForce((collider.gameObject.transform.position - forceOrigin) * 200f);
			collider.enabled = false;
		}
		else
			Debug.Log("Attached Rigid Body is Null");
	}
}
