using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveController : MonoBehaviour
{
	public float scale
    {
        get
        {
            return transform.localScale.x;
        }
        private set
        {

        }
    }
    public SteeringMovement move;
    void Start()
    {
        float startRot = Random.Range(0, 360);
        transform.rotation = Quaternion.Euler(0, startRot, 0);
    }

    void Update()
    {
        move.MoveDirection(0);
    }
}
