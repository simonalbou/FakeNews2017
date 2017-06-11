using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClickableNews : MonoBehaviour {

	[Header("Colors")]
	public Color alarmistColor;
	public Color evenementialColor;
	public Color hipsterColor;
	public Color nostalgiaColor;
	public Color metaColor;

	[Header("Audio")]
	public AudioClip onSpawnedSFX;
	public AudioClip onClickedSFX;

	[Header("References")]
	public Transform self;
	public Collider selfCollider;
	public AudioSource selfAudio;
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
		// animation goes here
		selfRenderer.color = Color.black;
		NewsPoolManager.instance.ClickSomeNews(this);
	}

	public void Kill()
	{
		// should play an animation here (coroutine ?) if selfCollider is still enabled
		available = true;
		selfCollider.enabled = false;
		content.SetActive(false);
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
	}

	public void Load(Card card)
	{
		title.text = card.title;

		family = card.family;

		Debug.Log(card.family);

		if (family == CardFamily.ALARMISTE)
			selfRenderer.color = alarmistColor;
		if (family == CardFamily.EVENEMENTIEL)
			selfRenderer.color = evenementialColor;
		if (family == CardFamily.HIPSTER)
			selfRenderer.color = hipsterColor;
		if (family == CardFamily.NOSTALGIE)
			selfRenderer.color = nostalgiaColor;
		if (family == CardFamily.META)
			selfRenderer.color = metaColor;

		isFakeNews = card.isFakeNews;
		inducedCards = card.inducedCards;

		// TODO
		//author.text = card.author;
		//picture.sprite = card.image; // we should also change template if no image is loaded, like, alternate with two contents
	}
}
