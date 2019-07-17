using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float xTranslation;
	public float smoothing;

	private Vector3 currentVelocity;
	private Vector3 targetPosition;

	private void Start()
	{
		targetPosition = transform.localPosition;
	}

	public void SetLane(int lane)
	{
		targetPosition = new Vector3(xTranslation * (lane - 1), transform.localPosition.y, transform.localPosition.z);
	}

	private void Update()
	{
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref currentVelocity, smoothing);
	}
}
