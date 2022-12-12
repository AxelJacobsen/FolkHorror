using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform player;
	public Vector3 	offset;
	public float	ScreenshakeFrequency = 10f;

	private float 	durationLeft = 0f,
					intensity = 0f,
					screenshakeC = 0f;

	// Update is called once per frame
	void FixedUpdate()
	{
		Vector3 offsetExtra = Vector3.zero;
		screenshakeC += Time.deltaTime * ScreenshakeFrequency;

		// Screenshake
		if (durationLeft > 0f)
		{
			durationLeft -= Time.deltaTime;
			if ( screenshakeC >= 1f) 
			{
				screenshakeC = 0f;
				float intensity0 = intensity;
				if (durationLeft <= 0.1f) intensity0 *= durationLeft / 0.1f;
				offsetExtra += new Vector3(Random.Range(-intensity0, intensity0), Random.Range(-intensity0, intensity0), Random.Range(-intensity0, intensity0));
			}
		}
		transform.position = (player.position + offset + offsetExtra);
	}

	/// <summary>
	/// Applies a screenshake effect to the screen.
	/// </summary>
	/// <param name="duration">The duration of the screenshake.</param>
	/// <param name="intensity">The intensity of the screenshake.</param>
	public void Screenshake(float duration, float intensity)
	{
		durationLeft = duration;
		this.intensity = intensity;
	}
}
