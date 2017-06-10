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

	[Header("References")]
	public Transform self;
	public Collider selfCollider;
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
		selfCollider.enabled = false;
		selfRenderer.color = Color.green;
		NewsPoolManager.instance.ClickSomeNews(this);
	}

	public void Kill()
	{
		available = true;
		selfCollider.enabled = false;
		content.SetActive(false);
	}

	public void Spawn(Vector3 pos)
	{
		content.SetActive(true);
		selfRenderer.enabled = true;
		selfRenderer.color = Color.white;
		self.position = pos;
		BillboardToZero();
		selfCollider.enabled = true;
		available = false;
	}

	public void Load(Card card)
	{
		author.text = card.author;
		title.text = card.title;
		picture.sprite = card.image; // we should change template if no image is loaded

		if (card.family == CardFamily.ALARMISTE)	selfRenderer.color = alarmistColor;
		if (card.family == CardFamily.EVENEMENTIEL) selfRenderer.color = evenementialColor;
		if (card.family == CardFamily.HIPSTER)		selfRenderer.color = hipsterColor;
		if (card.family == CardFamily.NOSTALGIE)	selfRenderer.color = nostalgiaColor;
		if (card.family == CardFamily.META)			selfRenderer.color = metaColor;

		family = card.family;
		isFakeNews = card.isFakeNews;
		inducedCards = card.inducedCards;
	}
}
