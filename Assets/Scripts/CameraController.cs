using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTarget;
    public float followDistance;
    public float followHeight;
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
        float adjHeight = followHeight * WaveStatusController.instance.transform.localScale.x;
        float adjDist = followDistance * WaveStatusController.instance.transform.localScale.x;

        Vector3 distance = playerTarget.position + (-transform.forward * adjDist);
        distance.y = playerTarget.position.y + adjHeight;
        transform.position = Vector3.Lerp(transform.position, distance, followAccel * Time.deltaTime);
    }
}