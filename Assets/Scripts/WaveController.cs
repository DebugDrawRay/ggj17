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

    void Update()
    {
        if (GameController.instance != null && 	GameController.instance.currentState == GameController.State.InGame)
        {
            steering.MoveDirection(input.Steer.Value);
        }
		else if (GameController.instance == null)
		{
			steering.MoveDirection(input.Steer.Value);
		}
    }
}
