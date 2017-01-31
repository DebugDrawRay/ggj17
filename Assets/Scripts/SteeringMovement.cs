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
			return moveSpeed * (Mathf.Max(1, transform.localScale.x * 0.5f));
		}
	}

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void MoveDirection(float direction)
    {
        float totalTurnSpeed = turnSpeed;
        if (status)
        {
			totalTurnSpeed /= Mathf.Max(1, transform.localScale.x / 25);
        }
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, currentSpeed * Time.deltaTime);
        Vector3 euler = transform.rotation.eulerAngles;
        euler.y = euler.y + (direction * totalTurnSpeed);
        Quaternion rot = Quaternion.Euler(euler);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.1f);
    }
}
