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

    private Color originalColor;
    private Color currentColor;
    public Color toColor;

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

            if (move != null)
            {
                move.MoveDirection(0);
            }
        }
        skin.material.SetColor("_Emission", currentColor);
        UpdateSmoothness();
    }
    bool tweening = false;
    Tween current;
    void UpdateSmoothness()
    {
        if(transform.localScale.x <= WaveStatusController.instance.transform.localScale.x)
        {
            if(!tweening)
            {
                current = DOTween.To(() => currentColor, x => currentColor = x, toColor, .5f).SetLoops(-1, LoopType.Yoyo);
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
		Tween birth = DOTween.To(() => currentFlatness, x => currentFlatness = x, 0, timeToBirth);
		birth.SetEase(Ease.InOutBack);
	}

	public void OnDeath()
	{
		Tween death = DOTween.To(() => currentFlatness, x => currentFlatness = x, 100, timeToDeath);
		death.SetEase(Ease.InOutBack);
		death.OnComplete(() => Destroy(gameObject));
	}
}
