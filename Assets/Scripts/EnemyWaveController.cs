using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyWaveController : MonoBehaviour
{
	[Header("Things")]
	public float scale;
    public SteeringMovement move;
	public Collider waveCollider;

	[Header("Wave Visuals")]
	public SkinnedMeshRenderer skin;
	public int flatBlend;
    public float timeToBirth = 2f;
    public float timeToDeath = .5f;
	protected int currentFlatness = 100;

	[Header("Colors")]
    private Color originalColor;
    private Color currentColor;
    public Color goodColor;
    public Color badColor;

    protected float killDistance = 1000f;
	protected bool alive = true;

	protected Tween birth;
	protected Tween death;

	void Start()
    {
        float startRot = Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(0, startRot, 0);
		currentFlatness = 100;
		OnBirth();
        originalColor = skin.material.GetColor("_Emission");
        currentColor = originalColor;
    }

    void Update()
    {
        if (GameController.instance.currentState == GameController.State.InGame)
        {
            scale = transform.localScale.x;
            skin.SetBlendShapeWeight(flatBlend, currentFlatness);

			if (currentFlatness == 0 && !waveCollider.enabled)
				waveCollider.enabled = true;

            if (move != null)
            {
                move.MoveDirection(0);
            }

			if (alive)
			{
				//Die if you're too smol
				if (WaveStatusController.instance != null)
				{
					if (scale < WaveStatusController.instance.scale * 0.4f)
					{
						StartCoroutine(DelayedDeath());
					}
				}

                if(Vector3.Distance(transform.position, WaveStatusController.instance.transform.position) > killDistance)
                {
					StartCoroutine(DelayedDeath());
                }
			}
        }
        skin.material.SetColor("_Emission", currentColor);
        UpdateSmoothness();
    }

    bool tweening = false;
    Tween current;
    void UpdateSmoothness()
    {
        if(WaveStatusController.instance && transform.localScale.x <= WaveStatusController.instance.transform.localScale.x)
        {
            if(!tweening)
            {
                current = DOTween.To(() => currentColor, x => currentColor = x, goodColor, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                tweening = true;
            }
        }
        else
        {
            current.Kill(true);
            currentColor = originalColor;
            tweening = false;
        }
    }

	public void OnBirth()
	{
		birth = DOTween.To(() => currentFlatness, x => currentFlatness = x, 0, timeToBirth);
		birth.SetEase(Ease.InOutBack);
	}

	protected IEnumerator DelayedDeath()
	{
		float time = Random.Range(1f, 5f);
		yield return new WaitForSeconds(time);
		OnDeath();
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
