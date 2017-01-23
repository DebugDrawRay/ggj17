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

	public float currentSpeed
	{
		get
		{
			return moveSpeed * transform.localScale.x;
		}
	}

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void MoveDirection(float direction)
    {
        float totalSpeed = moveSpeed;
        float totalTurnSpeed = turnSpeed;
        if (status)
        {
            totalSpeed *= transform.localScale.x;
			totalTurnSpeed /= Mathf.Max(1, transform.localScale.x / 15);
        }
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, totalSpeed * Time.deltaTime);
        Vector3 euler = transform.rotation.eulerAngles;
        euler.y = euler.y + (direction * totalTurnSpeed);
        Quaternion rot = Quaternion.Euler(euler);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.1f);
    }
}
