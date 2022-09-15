using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


	public Rigidbody rb;

	public float MovementSpeedWalk = 200f;
	public float MovementSpeedRun = 300f;
	public float MoveSpeedMax = 5f;
	public float JumpHeight = 10f;
	float MoveSpeed;
	public bool JumpAllowed = true;

	// Update is called once per frame
	void Start()
	{
		//Initializer
	}


	void FixedUpdate()
	{

		MoveSpeed = rb.velocity.magnitude;
		/*
        if (Input.GetKey("d") && MoveSpeed < MoveSpeedMax)
        {
            rb.AddForce(MovementSpeedWalk * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        };

        if (Input.GetKey("a") && MoveSpeed < MoveSpeedMax)
        {
            rb.AddForce(-MovementSpeedWalk * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        };

        if (Input.GetKey("w") && MoveSpeed < MoveSpeedMax)
        {
           // rb.AddForce(0, 0, MovementSpeedWalk * Time.deltaTime, ForceMode.VelocityChange);
           transform.forward + MovementSpeedWalk;
        }

        if (Input.GetKey("s") && MoveSpeed < MoveSpeedMax)
        {
            rb.AddForce(0, 0, -MovementSpeedWalk * Time.deltaTime, ForceMode.VelocityChange);
        }
        */

		float h = Input.GetAxis("Horizontal") * MovementSpeedWalk * Time.deltaTime;
		float v = Input.GetAxis("Vertical") * MovementSpeedWalk * Time.deltaTime;

		//        _transform.localPosition += _transform.right * h;
		//        _transform.localPosition += _transform.forward * v;

		Vector3 RIGHT = transform.TransformDirection(Vector3.right);
		Vector3 FORWARD = transform.TransformDirection(Vector3.forward);

		transform.localPosition += RIGHT * h;
		transform.localPosition += FORWARD * v;
	}
}