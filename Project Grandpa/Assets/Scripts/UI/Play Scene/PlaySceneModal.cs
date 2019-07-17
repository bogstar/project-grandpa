using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grandpa.UI
{
	public abstract class PlaySceneModal : MonoBehaviour
	{
		public PlayGUI PlayGUI { get; private set; }

		protected readonly string animParam_show = "Show";
		protected Animator animator;

		protected virtual void Awake()
		{
			PlayGUI = FindObjectOfType<PlayGUI>();
			animator = GetComponent<Animator>();
			ShowModal(false, true);
		}

		protected virtual void Start()
		{
			
		}

		public virtual void ShowModal(bool show, bool immediate = false)
		{
			if (immediate)
			{
				gameObject.SetActive(show);
			}
			else
			{
				if (!show)
				{
					gameObject.SetActive(true);
					StartCoroutine(Hide());
				}
				else
				{
					gameObject.SetActive(true);
				}
				if (animator != null)
				{
					animator.SetBool(animParam_show, show);
				}
			}
		}

		protected virtual string HideAnimationName { get { return ""; } }

		private IEnumerator Hide()
		{
			yield return null;

			while	((animator.GetCurrentAnimatorStateInfo(0).IsName(HideAnimationName) ||
					animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) &&
					HideAnimationName != "")
			{
				print(animator.GetCurrentAnimatorStateInfo(0).IsName(HideAnimationName) + " " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
				yield return null;
			}

			gameObject.SetActive(false);
		}
	}
}