using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
	[Header("Values")]
	[SerializeField] private float playerStartingSpeed = 12.5f;
	[SerializeField] private float acceleration = 0.05f;
	[SerializeField] private float smoothDamp = 0.3f;
	[SerializeField] private float cameraOrbitSpeed = 4f;
	[SerializeField] private AnimationCurve animationSpeed;

	[Header("References")]
	[SerializeField] private Animator animator;
	[SerializeField] private Transform grandpaGameObject;
	[SerializeField] private PlayerCollider playerCollider;
	[SerializeField] private Camera mainCamera;
	[SerializeField] private GameObject poseCamera;
	[SerializeField] private CameraController cameraController;
	[SerializeField] private AnimationClip defaultDeathClip;

	public State CurrentState { get; private set; }
	public float DistanceTravelled { get; private set; }
	public float MovementSpeed { get; private set; }

	private Vector2 colliderTargetDimensions;

	private AnimatorOverrideController overrideController;
	private bool isSliding;
	private float currentSpeed;
	private int currentLane;
	private new CapsuleCollider collider;

	private Vector3 currentPos;

	private System.Action OnCrashedCallback;


	public void Init()
	{
		StartPose();
		collider = grandpaGameObject.GetComponent<CapsuleCollider>();
		overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
		animator.runtimeAnimatorController = overrideController;
	}

	public void RegisterCrashedCallback(System.Action OnCrashedCallback)
	{
		this.OnCrashedCallback += OnCrashedCallback;
	}

	public void UnregisterCrashedCallback(System.Action OnCrashedCallback)
	{
		this.OnCrashedCallback -= OnCrashedCallback;
	}

	private void Update()
	{
		if (FindObjectOfType<RunManager>().Paused)
		{
			animator.SetFloat("Locomotion", 0);
			return;
		}

		Vector3 targetPos = new Vector3(4f * (currentLane - 1), 0, 0);
		grandpaGameObject.localPosition = Vector3.SmoothDamp(grandpaGameObject.localPosition, targetPos, ref currentPos, smoothDamp);
		if (!isSliding)
		{
			animator.speed = animationSpeed.Evaluate(MovementSpeed);
		}
		else
		{
			animator.speed = 1f;
		}
		animator.SetFloat("Locomotion", 1);

		switch (CurrentState)
		{
			case State.Running:
				MovementSpeed += acceleration * Time.deltaTime;
				break;

			case State.Posing:
				poseCamera.transform.eulerAngles += new Vector3(0, cameraOrbitSpeed, 0f) * Time.deltaTime;
				break;
		}

		DistanceTravelled += MovementSpeed * Time.deltaTime;

		/*
		if (CurrentState != State.Dead)
		{
			animator.SetFloat("Locomotion", 0);
			animator.speed = 1f;
			return;
		}*/
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (CurrentState != State.Running)
		{
			return;
		}

		var obstacle = collision.gameObject.GetComponent<Obstacle>();
		if (obstacle != null)
		{
			OnCrashedCallback?.Invoke();
			AudioManager.PlayClipStackable(AudioManager.Audio.Death);

			overrideController[defaultDeathClip.name] = defaultDeathClip;
			if (obstacle.ObstacleInfo.deathClip != null)
			{
				overrideController[defaultDeathClip.name] = obstacle.ObstacleInfo.deathClip;
			}
			
			animator.SetBool("Death", true);
			ChangeState(State.Dead);
		}
	}
	
	private IEnumerator SlideCoroutine()
	{
		AudioManager.PlayClipStackable(AudioManager.Audio.Click);
		isSliding = true;
		animator.SetBool("Crouch", true);
		colliderTargetDimensions = new Vector2(1f, 1.8f);
		StartCoroutine(Crouch(10f / 60f));
		
		float timer = (FindObjectOfType<RunManager>().segmentLength / MovementSpeed) - (10f / 60f) + Time.time;

		while (Time.time < timer)
		{
			yield return null;
		}

		animator.SetBool("Crouch", false);
		colliderTargetDimensions = new Vector2(1.6f, 3f);
		StartCoroutine(Crouch(25f / 60f));

		yield return new WaitForSeconds(25f / 60f);

		isSliding = false;
	}

	private IEnumerator Crouch(float duration)
	{
		float startTime = Time.time;
		float endTime = startTime + duration;

		while (Time.time < endTime)
		{
			float percDone = Mathf.InverseLerp(startTime, endTime, Time.time);
			var lerpedVec = Vector2.Lerp(new Vector2(collider.center.y, collider.height), colliderTargetDimensions, percDone);
			collider.height = lerpedVec.y;
			collider.center = new Vector3(collider.center.x, lerpedVec.x, collider.center.z);
			yield return null;
		}

		collider.height = colliderTargetDimensions.y;
		collider.center = new Vector3(collider.center.x, colliderTargetDimensions.x, collider.center.z);
	}
	
	public void ChangeLaneLeft()
	{
		if (currentLane < 1)
		{
			return;
		}

		currentLane--;

		AudioManager.PlayClipStackable(AudioManager.Audio.Click);
		cameraController.SetLane(currentLane);
	}
	
	public void ChangeLaneRight()
	{
		if (currentLane > 1)
		{
			return;
		}

		currentLane++;

		AudioManager.PlayClipStackable(AudioManager.Audio.Click);
		cameraController.SetLane(currentLane);
	}

	public void Left()
	{
		if (CurrentState != State.Running)
		{
			return;
		}
		
		ChangeLaneLeft();
	}

	public void Right()
	{
		if (CurrentState != State.Running)
		{
			return;
		}

		ChangeLaneRight();
	}

	public void Down()
	{
		if (CurrentState != State.Running || isSliding)
		{
			return;
		}
		
		StartCoroutine(SlideCoroutine());
	}

	private void ChangeState(State newState)
	{
		CurrentState = newState;

		switch (newState)
		{
			case State.Posing:
				DistanceTravelled = 0;
				animator.SetBool("Death", false);
				currentLane = 1;
				MovementSpeed = playerStartingSpeed;
				poseCamera.SetActive(true);
				mainCamera.enabled = false;
				transform.position = Vector3.zero;
				grandpaGameObject.gameObject.SetActive(true);
				break;

			case State.Running:
				DistanceTravelled = 0;
				MovementSpeed = playerStartingSpeed;
				poseCamera.SetActive(false);
				mainCamera.enabled = true;
				transform.position = Vector3.zero;
				grandpaGameObject.gameObject.SetActive(true);
				playerCollider.RegisterCallback(OnCollisionEnter);
				break;

			case State.Dead:
				animator.speed = 1f;
				playerCollider.UnregisterCallback(OnCollisionEnter);
				MovementSpeed = 0;
				break;
		}
	}

	public void StartPose()
	{
		if (CurrentState != State.Dead)
		{
			return;
		}

		currentLane = 1;
		cameraController.SetLane(currentLane);
		ChangeState(State.Posing);
		
		//playerCollider.transform.GetChild(0).gameObject.SetActive(true);
	}

	public void StartRunning()
	{
		if (CurrentState != State.Posing)
		{
			return;
		}

		currentLane = 1;
		cameraController.SetLane(currentLane);
		ChangeState(State.Running);
	}

	public enum State
	{
		Dead, Posing, Running
	}
}