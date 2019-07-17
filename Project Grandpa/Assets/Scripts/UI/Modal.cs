using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Modal : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private TextMeshProUGUI title;
	[SerializeField] private TextMeshProUGUI content;
	[SerializeField] private Button backgroundButton;
	[SerializeField] private GameObject modal;

	private Animator animator;
	private MainMenuLevelManager levelManager;


	private void Start()
	{
		levelManager = (MainMenuLevelManager)LevelManager.Instance;
		animator = GetComponent<Animator>();
		modal.SetActive(false);
	}

	public void Display(string title, string content, System.Action onFinishedCallback = null, System.Action onClicked = null, float delay = 0)
	{
		gameObject.SetActive(true);
		StartCoroutine(DisplayC(title, content, onFinishedCallback, onClicked, delay));
	}

	public void OnButtonContinue()
	{
		AudioManager.PlayClipStackable(AudioManager.Audio.Click);
		StartCoroutine(Disappear());
	}

	public void OnButtonPlay()
	{
		levelManager.PlayLevel();
	}

	public void OnButtonCredits()
	{
		StartCoroutine(DisplayC("Credits", "This game has been brought to you through the curtesy of:\n\nBogomil Kruzic\nLead Developer, Game Designer, Programmer\n\nPaolo Biasiol\nLead Artist, Game Designer\n\nPaolo Mudrovcic\nLead Game Designer, Quality Assurance"));
	}

	public void OnButtonOptions()
	{
		StartCoroutine(DisplayC("Unavailable", "This feature is not yet available in this version."));
	}

	System.Action onClicked = null;

	private IEnumerator DisplayC(string title, string content, System.Action onFinishedCallback = null, System.Action onClicked = null, float delay = 0)
	{
		yield return new WaitForSeconds(delay);

		AudioManager.PlayClipStackable(AudioManager.Audio.Click);
		backgroundButton.gameObject.SetActive(true);
		animator.Rebind();
		animator.SetTrigger("Appear");

		this.title.text = title;
		this.content.text = content;

		modal.SetActive(true);

		float expire = Time.time + 0.2f;
		this.onClicked = onClicked;

		while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0) || Time.time < expire)
		{
			yield return null;
		}

		onFinishedCallback?.Invoke();
		backgroundButton.enabled = true;
	}

	private IEnumerator Disappear()
	{
		backgroundButton.gameObject.SetActive(false);
		animator.SetTrigger("Disappear");

		float expire = Time.time + 0.3f;

		while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0) || Time.time < expire)
		{
			yield return null;
		}

		onClicked?.Invoke();
		modal.SetActive(false);
	}
}
