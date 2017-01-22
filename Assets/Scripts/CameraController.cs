using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public Transform playerTarget;
    public float followDistance;
    public float followHeight;
    public float followAccel;
    public MeshFilter mesh;

    public static CameraController instance;

    public enum State
    {
        Start,
        InGame,
    }
    public State currentState;

    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        switch(GameController.instance.currentState)
        {
            case GameController.State.InGame:
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

    public void MoveToPlayer(TweenCallback Callback)
    {
        float adjHeight = followHeight * WaveStatusController.instance.transform.localScale.x;
        float adjDist = followDistance * WaveStatusController.instance.transform.localScale.x;

        Vector3 distance = playerTarget.position + (-transform.forward * adjDist);
        distance.y = playerTarget.position.y + adjHeight;
        Vector3 pos = distance;

        transform.DOMove(pos, 1f).SetEase(Ease.InOutSine).OnComplete(Callback);
    }
}