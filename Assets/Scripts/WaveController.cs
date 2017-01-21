using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public SteeringMovement steering;
    private PlayerActions input;

    void Awake()
    {
        input = PlayerActions.BindAll();
    }

    void FixedUpdate()
    {
        steering.MoveDirection(input.Steer.Value);
    }
}
