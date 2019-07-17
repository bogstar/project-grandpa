using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager<T> : Singleton<T> where T : class
{
	protected override void Awake()
	{
		base.Awake();
		
		if (transform.parent == transform.root)
		{
			Transform parent = transform.parent;

			transform.SetParent(null);

			if (parent.childCount < 1)
			{
				Destroy(parent.gameObject);
			}
		}
		DontDestroyOnLoad(gameObject);
	}
}
