using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : class
{
	public static T Instance { get; protected set; }

	protected virtual void Awake()
	{
		if (Instance == null)
		{
			Instance = this as T;
			if (transform.parent != transform.root)
			{
				DontDestroyOnLoad(gameObject);
			}
		}
		else
		{
			DestroyImmediate(gameObject);
		}
	}
}