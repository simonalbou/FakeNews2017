using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClickableNews : MonoBehaviour {

	[Header("Colors")]
	public Sprite alarmistColor;
	public Sprite evenementialColor;
	public Sprite hipsterColor;
	public Sprite nostalgiaColor;
	public Sprite metaColor;

	[Header("Audio")]
	public AudioClip onSpawnedSFX;
	public AudioClip onClickedSFX;

	[Header("References")]
	public Transform self;
	public Collider selfCollider;
	public AudioSource selfAudio;
	public AnimationHandler selfAnim;
	public GameObject content;
	public Image selfRenderer, picture;
	public Text title, author;
	public UnityEvent OnSelected;

	// Inherited/Loaded from the Card object
	[System.NonSerialized]
	public CardFamily family;
	[System.NonSerialized]
	public bool isFakeNews;
	[System.NonSerialized]
	public Card[] inducedCards;

	// For pooling
	[System.NonSerialized]
	public bool available;

	public void BillboardToZero()
	{
		self.LookAt(Vector3.zero);
		self.Rotate(Vector3.up, 180, Space.Self);
	}

	public void Activate()
	{
		selfAudio.clip = onClickedSFX;
		selfAudio.Play();
		selfCollider.enabled = false;
		selfAnim.AlphaTo(0, 1);
		selfAnim.AltitudeTo(2, 1);
		NewsPoolManager.instance.ClickSomeNews(this);
	}

	public void Kill()
	{
		if (selfCollider.enabled)
		{
			selfAnim.AlphaTo(0, 0.3f);
			selfAnim.AltitudeTo(-1, 0.3f);
			selfAnim.ScaleTo(0, 0.3f);
		}
		available = true;
		selfCollider.enabled = false;
		//content.SetActive(false);
	}

	public void Spawn(Vector3 pos)
	{
		content.SetActive(true);
		selfRenderer.enabled = true;
		self.position = pos;
		BillboardToZero();
		selfCollider.enabled = true;
		available = false;
		selfAudio.clip = onSpawnedSFX;
		selfAudio.Play();
		selfAnim.AlphaTo(1, 1);
		selfAnim.ScaleTo(1, 0);
		selfAnim.AltitudeTo(2, 0);
		selfAnim.AltitudeTo(0, 1);
	}

	public void Load(Card card)
	{
		title.text = card.title;

		family = card.family;

		if (family == CardFamily.ALARMISTE)
			selfRenderer.sprite = alarmistColor;
		if (family == CardFamily.EVENEMENTIEL)
			selfRenderer.sprite = evenementialColor;
		if (family == CardFamily.HIPSTER)
			selfRenderer.sprite = hipsterColor;
		if (family == CardFamily.NOSTALGIE)
			selfRenderer.sprite = nostalgiaColor;
		if (family == CardFamily.META)
			selfRenderer.sprite = metaColor;

		isFakeNews = card.isFakeNews;
		inducedCards = card.inducedCards;

		author.text = card.author;
		picture.sprite = card.image;
	}
}
