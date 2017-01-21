using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform anchor;
    public Vector3 offset;
    public float followAccel;

    void LateUpdate()
    {
        Vector3 pos = anchor.position + offset;
        transform.position = Vector3.Lerp(transform.position, pos, followAccel * Time.deltaTime);
    }

}
