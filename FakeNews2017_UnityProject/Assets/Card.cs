using UnityEngine;

public class Card : ScriptableObject 
{
	[Header("News Content")]
	public string title = "";
	public string author = "";
	public Sprite image;

	[Header("News Type")]
	public CardFamily family = default(CardFamily);
	public bool isFakeNews;
	public string reference;
	
	[Header("Availability by Day")]
	public int firstDay;
	public int lastDay;

	[Header("Consequences for next day")]
	public Card[] inducedCards;
}
