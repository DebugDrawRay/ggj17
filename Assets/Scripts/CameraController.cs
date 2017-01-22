using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTarget;
    public Vector3 offset;
    public float followAccel;
    public MeshFilter mesh;

    public enum State
    {
        Start,
        InGame,
    }
    public State currentState;

    void Update()
    {
        switch(currentState)
        {
            case State.Start:
                break;
            case State.InGame:
                FollowPlayer();
                break;
        }
    }

    void FollowPlayer()
    {
        Vector3 adjOffset = offset * WaveStatusController.instance.transform.localScale.x;
        Vector3 pos = playerTarget.position + adjOffset;
        transform.position = Vector3.Lerp(transform.position, pos, followAccel);
    }
}
