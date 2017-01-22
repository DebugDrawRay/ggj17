using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum State
    {
        Start,
        Pause,
        InGame,
        End
    }
    public State currentState;

    public static GameController instance;
    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        RunStates();
    }

    void RunStates()
    {
        switch(currentState)
        {
            case State.Start:
                break;
            case State.Pause:
                break;
            case State.InGame:
                break;
            case State.End:
                break;
        }
    }
}
