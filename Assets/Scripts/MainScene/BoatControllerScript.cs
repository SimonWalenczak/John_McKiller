using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FloatObjectScript))]
public class BoatControllerScript : MonoBehaviour
{
	public Vector3 COM;
	[Space(15)]
	public Rigidbody rigidbodyA;
	public float stopVelocity = 1.0f;
	public float speed = 1.0f;
	public float steerSpeed_low = 1.0f;
	public float steerSpeed_high = 1.0f;
	public float movementThresold = 10.0f;

	Transform m_COM;
	float verticalInput;
	float movementFactor;
	float horizontalInput;
	float steerFactor;

	// Update is called once per frame
	void Update()
	{
		Balance();
		Movement();
		Steer();

		if (rigidbodyA.velocity.magnitude < stopVelocity)
		{
			Stop();
		}
	}

	void Balance()
	{
		if (!m_COM)
		{
			m_COM = new GameObject("COM").transform;
			m_COM.SetParent(transform);
		}

		m_COM.position = COM;
		GetComponent<Rigidbody>().centerOfMass = m_COM.position;
	}

	void Movement()
	{
		verticalInput = Input.GetAxis("Vertical");
		movementFactor = Mathf.Lerp(movementFactor, verticalInput, Time.deltaTime / movementThresold);
		transform.Translate(0.0f, 0.0f, movementFactor * speed);
	}

	void Steer()
	{
        if (!Input.GetKey(KeyCode.UpArrow))
        {
			horizontalInput = Input.GetAxis("Horizontal");
			steerFactor = Mathf.Lerp(steerFactor, horizontalInput, Time.deltaTime);
			transform.Rotate(0.0f, steerFactor * steerSpeed_low, 0.0f);
        }

        else if (Input.GetKey(KeyCode.UpArrow))
        {
			horizontalInput = Input.GetAxis("Horizontal");
			steerFactor = Mathf.Lerp(steerFactor, horizontalInput, Time.deltaTime);
			transform.Rotate(0.0f, steerFactor * steerSpeed_high, 0.0f);
		}
	}
	private void Stop()
	{
		rigidbodyA.velocity = Vector3.zero;
		rigidbodyA.angularVelocity = Vector3.zero;
	}
}