using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
	private event System.Action<Collision> OnCollisionAction;

	public void RegisterCallback(System.Action<Collision> cb)
	{
		OnCollisionAction += cb;
	}

	public void UnregisterCallback(System.Action<Collision> cb)
	{
		OnCollisionAction -= cb;
	}

	private void OnCollisionEnter(Collision collision)
	{
		OnCollisionAction?.Invoke(collision);
	}
}
