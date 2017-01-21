using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform anchor;
    public Vector3 offset;
    public float followAccel;
    public MeshFilter mesh;
    void LateUpdate()
    {
        Vector3 adjOffset = offset * WaveStatusController.instance.transform.localScale.x;
        Vector3 pos = anchor.position + adjOffset;
        transform.position = pos;
    }

}
