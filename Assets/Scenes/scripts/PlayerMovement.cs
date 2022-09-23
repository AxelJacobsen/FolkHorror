using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	private Animator anim;
	public Rigidbody rb;
	private Vector3 vel;

	private bool facingRight;
	private bool turning;

	private int spaceHeld;

	public float MovementSpeedWalk = 100f;
	public float MovementSpeedRun = 200f;
	public float MoveSpeedMax = 5f;
	public float JumpHeight = 10f;
	float MoveSpeed;
	public bool JumpAllowed = true;

	// Update is called once per frame
	void Start()
	{
		anim = GetComponent<Animator>();
		vel = new Vector3 (0, 0, 0);
		spaceHeld = 0;
		facingRight = true;
		turning = false;
	}

	void Turn() {
		turning = true;
		anim.SetTrigger("turn");
	}

	void FixedUpdate()
	{
		// Turn!
		if (turning && anim.GetCurrentAnimatorStateInfo(0).IsName("stickman2")) {
			Vector3 temp =  transform.localScale;
			temp.x *= -1;
			transform.localScale = temp;
			turning = false;
		}

		// Update
		Vector3 dir = new Vector3 (Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
		vel += (dir*MovementSpeedWalk - vel) * 10f  * Time.deltaTime;
		transform.localPosition += vel * Time.deltaTime;

		// Flip if we're heading left
		if (vel.x>0 && facingRight) {
			Turn();
			facingRight = false;

		} else if (vel.x<0 && !facingRight) {
			Turn();
			facingRight = true;
		}
		
		// Toggle spacebar
		if (Input.GetKey("space")) {
			spaceHeld++;
		} else {
			spaceHeld = 0;
		}

		if (spaceHeld == 1) {
			print("Space!");
		}
	}
}