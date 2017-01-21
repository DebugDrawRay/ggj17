using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SteeringMovement : MonoBehaviour
{
    public float moveSpeed;
    public float turnSpeed;
    public float turnAccel;
    private Rigidbody rigid;
    public WaveStatusController status;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void MoveDirection(float direction)
    {
        float totalSpeed = moveSpeed;
        float totalAccel = turnAccel;
        if (status)
        {
            totalSpeed *= transform.localScale.x;
            totalAccel /= transform.localScale.x;
        }
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, totalSpeed * Time.deltaTime);
        Vector3 euler = transform.rotation.eulerAngles;
        euler.y = euler.y + (direction * turnSpeed);
        Quaternion rot = Quaternion.Euler(euler);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, turnAccel);
    }
}
