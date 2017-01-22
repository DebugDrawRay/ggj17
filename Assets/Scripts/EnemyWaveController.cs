using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyWaveController : MonoBehaviour
{
	[Header("Things")]
	public float scale;
    public SteeringMovement move;

	[Header("Wave Visuals")]
	public SkinnedMeshRenderer skin;
	public int flatBlend;
	protected int currentFlatness = 100;

	void Start()
    {
        float startRot = Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(0, startRot, 0);
		currentFlatness = 100;
		OnBirth();
    }

    void Update()
    {
        scale = transform.localScale.x;
		skin.SetBlendShapeWeight(flatBlend, currentFlatness);

		if (move != null)
		{
			move.MoveDirection(0);
		}
    }

	public void OnBirth()
	{
		Tween birth = DOTween.To(() => currentFlatness, x => currentFlatness = x, 0, 2);
		birth.SetEase(Ease.InOutBack);
	}

	public void OnDeath()
	{
		Debug.Log("On death");
		Tween death = DOTween.To(() => currentFlatness, x => currentFlatness = x, 100, 2);
		death.SetEase(Ease.InOutBack);
		death.OnComplete(() => Destroy(gameObject));
	}
}
