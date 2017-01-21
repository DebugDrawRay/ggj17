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

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void MoveDirection(float direction)
    {
        Debug.Log(direction);

        Vector3 forward = transform.forward * moveSpeed;
        rigid.AddForce(forward, ForceMode.Acceleration);
        //Vector3 euler = transform.rotation.eulerAngles;
        //euler.y = euler.y + (direction * turnSpeed);
        //Quaternion rot = Quaternion.Euler(euler);
        //transform.rotation = Quaternion.Lerp(transform.rotation, rot, turnAccel);

        rigid.AddTorque(Vector3.up * (direction * turnSpeed));
    }
}
