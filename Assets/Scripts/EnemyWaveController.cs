using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveController : MonoBehaviour
{
	public float scale;
    public SteeringMovement move;
    void Start()
    {
        float startRot = Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(0, startRot, 0);
    }

    void Update()
    {
        scale = transform.localScale.x;
        move.MoveDirection(0);
    }
}
