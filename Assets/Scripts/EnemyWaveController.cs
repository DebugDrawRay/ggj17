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
    public float timeToBirth = 2f;
    public float timeToDeath = .5f;
	protected int currentFlatness = 100;

	protected float lifetime = 0;
	protected float maxLifetime = 15f;
	protected bool alive = true;

	protected Tween birth;
	protected Tween death;

	void Start()
    {
        float startRot = Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(0, startRot, 0);
		currentFlatness = 100;
		OnBirth();
    }

    void Update()
    {
        if (GameController.instance.currentState == GameController.State.InGame)
        {
            scale = transform.localScale.x;
            skin.SetBlendShapeWeight(flatBlend, currentFlatness);

            if (move != null)
            {
                move.MoveDirection(0);
            }

			if (alive)
			{
				//Die if you're too smol
				if (WaveStatusController.instance != null)
				{
					if (scale < WaveStatusController.instance.scale * 0.1f)
					{
						OceanBodySpawner.instance.RefillEnemies();
						OnDeath();
					}
				}

				//Die after certain amount of time
				lifetime += Time.deltaTime;
				if (lifetime > maxLifetime)
				{
					OceanBodySpawner.instance.RefillEnemies();
					OnDeath();
				}
			}
        }
    }

	public void OnBirth()
	{
		birth = DOTween.To(() => currentFlatness, x => currentFlatness = x, 0, timeToBirth);
		birth.SetEase(Ease.InOutBack);
	}

	public void OnDeath()
	{
		alive = false;
		birth.Kill();
		death = DOTween.To(() => currentFlatness, x => currentFlatness = x, 100, timeToDeath);
		death.SetEase(Ease.InOutBack);
		death.OnComplete(() => DestroyTheObject());
	}

	protected void DestroyTheObject()
	{
		death.Kill();
		Destroy(gameObject);
	}
}
