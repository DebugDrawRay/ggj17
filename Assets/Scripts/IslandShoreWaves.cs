using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandShoreWaves : MonoBehaviour
{
	[Header("Island Stuff")]
	public MeshRenderer meshRenderer;
	public int materialIndex;

	[Header("Wave Stuff")]
	public float minYVal;
	public float maxYVal;
	public float speed = 2;
	public AnimationCurve movementCurve;

	protected Material waveMaterial;
	protected bool grow = true;
	protected float time = 0;

	public void Awake()
	{
		waveMaterial = meshRenderer.materials[materialIndex];
		Debug.Log("Wave material:" + waveMaterial.name);
	}

	public void Update()
	{
		if (grow)
		{
			if (waveMaterial.mainTextureOffset.y >= maxYVal)
			{
				grow = false;
				time = 0;
			}
			else
			{
				Vector2 offset = new Vector2(0, Mathf.Lerp(minYVal, maxYVal, movementCurve.Evaluate(time)));
				waveMaterial.mainTextureOffset = offset;
				time += Time.deltaTime * speed;
			}
		}
		else
		{
			if (waveMaterial.mainTextureOffset.y <= minYVal)
			{
				grow = true;
				time = 0;
			}
			else
			{
				Vector2 offset = new Vector2(0, Mathf.Lerp(maxYVal, minYVal, movementCurve.Evaluate(time)));
				waveMaterial.mainTextureOffset = offset;
				time += Time.deltaTime * speed;
			}
		}
	}
}
