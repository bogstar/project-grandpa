using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

	public void Init()
	{
		Instance = this;
	}
}