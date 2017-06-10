﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BubbleDay
{
	public string name;
	public int newsToValid;
	public int numberOfRandomNews;
	public List<Card> forcedNews;
	public bool isSpecialDay;
	public float fixedExpIfSpecialDay;
}

public class NewsPoolManager : MonoBehaviour
{
	// pooling
	public static NewsPoolManager instance;
	public Spawner spawner;
	public ClickableNews[] pool;

	// day manager
	[System.NonSerialized]
	public int curDay, curActivatedNews;
	[System.NonSerialized]
	public float evenementialScore, alarmistScore, nostalgiaScore, hipsterScore, alarmWeight, eventWeight, nostalWeight, hipsterWeight, invTotal;
	[System.NonSerialized]
	public List<Card> cardsOfTheDay;

	[Header("Gameplay")]
	public float startScore = 4;
	public float multiplierPerArticle = 1;
	public float fakeNewsMultiplier = 2;
	public BubbleDay[] days;
	public Card[] allRandomCards;

	public void Awake()
	{
		cardsOfTheDay = new List<Card>();
		instance = this;
		curDay = 0;
		curActivatedNews = 0;

		for (int i = 0; i < pool.Length; i++)
			pool[i].Kill();

		evenementialScore = alarmistScore = nostalgiaScore = hipsterScore = startScore;

		spawner.InitSpawn ();

		SpawnNews();
	}

	// Pooling : gets available news object
	public ClickableNews GetNews()
	{
		for (int i = 0; i < pool.Length; i++)
			if (pool[i].available) return pool[i];

		return null;
	}

	// Called when a news is liked
	public void ClickSomeNews(ClickableNews cn)
	{
		// How much exp does the color gain ?
		float exp = curDay * multiplierPerArticle;
		if (cn.isFakeNews) exp *= fakeNewsMultiplier;
		if (days[curDay].isSpecialDay) exp = days[curDay].fixedExpIfSpecialDay;

		// Which color benefits from this ?
		CardFamily cf = cn.family;
		if (cf == CardFamily.ALARMISTE) alarmistScore += exp;
		if (cf == CardFamily.EVENEMENTIEL) evenementialScore += exp;
		if (cf == CardFamily.HIPSTER) hipsterScore += exp;
		if (cf == CardFamily.NOSTALGIE) nostalgiaScore += exp;

		// Recalculate spawn weights
		invTotal = 1 / (alarmistScore + evenementialScore + hipsterScore + nostalgiaScore);
		alarmWeight = alarmistScore * invTotal;
		eventWeight = evenementialScore * invTotal;
		hipsterWeight = hipsterScore * invTotal;
		nostalWeight = nostalgiaScore * invTotal;

		// Does this unlock something for the next day ?
		if (curDay + 1 < days.Length)
			if (cn.inducedCards != null)
				if (cn.inducedCards.Length > 0)
					for (int i = 0; i < cn.inducedCards.Length; i++)
					{
						days[curDay + 1].forcedNews.Add(cn.inducedCards[i]);
						days[curDay + 1].numberOfRandomNews--;
					}

		// Are we done with this day yet ?
		curActivatedNews++;
		if (curActivatedNews == days[curDay].newsToValid)
		{
			curDay++;
			curActivatedNews = 0;

			for (int i = 0; i < pool.Length; i++)
				pool[i].Kill();

			SpawnNews();
		}
	}

	// Called at the beginning of each day
	public void SpawnNews()
	{
		// Determine how many news there are, and where they should spawn
		int forced = days[curDay].forcedNews == null ? 0 : days[curDay].forcedNews.Count;
		int randomNews = Mathf.Max(days[curDay].numberOfRandomNews, 0);
		int total = forced + randomNews;
		Vector3[] spawnPositions = spawner.GetPositions(total);

		int spawned = 0;

		// Spawning forced news
		if (forced > 0)
			for (int i = 0; i < forced; i++)
			{
				ClickableNews news = GetNews();
				news.Load(days[curDay].forcedNews[i]);
				news.Spawn(spawnPositions[spawned]);
				spawned++;
			}

		// Spawning random news : also refreshing pool of today's cards
		if (randomNews > 0)
		{
			cardsOfTheDay.Clear();
			cardsOfTheDay.TrimExcess();
			cardsOfTheDay = new List<Card>();
			for (int i = 0; i < allRandomCards.Length; i++)
				if (allRandomCards[i].firstDay <= curDay && allRandomCards[i].lastDay >= curDay)
					cardsOfTheDay.Add(allRandomCards[i]);

			for (int i = 0; i < randomNews; i++)
			{
				ClickableNews news = GetNews();
				news.Load(GetRandomCard());
				news.Spawn(spawnPositions[spawned]);
				spawned++;
			}
		}
	}

	// Rolls a card for random news
	public Card GetRandomCard()
	{
		CardFamily family;
		float roll = Random.value;
		if (roll < alarmWeight) family = CardFamily.ALARMISTE;
		else if (roll < alarmWeight + hipsterWeight) family = CardFamily.HIPSTER;
		else if (roll < alarmWeight + hipsterWeight + nostalWeight) family = CardFamily.NOSTALGIE;
		else family = CardFamily.EVENEMENTIEL;

		float fakeNewsChance = 0;
		if (curDay > 4)
		{
			if (family == CardFamily.ALARMISTE)
				fakeNewsChance = Mathf.Min(alarmistScore * 0.01f, 0.7f);

			if (family == CardFamily.HIPSTER)
				fakeNewsChance = Mathf.Min(hipsterScore * 0.01f, 0.7f);

			if (family == CardFamily.EVENEMENTIEL)
				fakeNewsChance = Mathf.Min(evenementialScore * 0.01f, 0.7f);

			if (family == CardFamily.NOSTALGIE)
				fakeNewsChance = Mathf.Min(nostalgiaScore * 0.01f, 0.7f);
		}
		bool rollFakeNews = Random.value < fakeNewsChance;

		List<Card> possibleCards = GetPossibleCards(family, rollFakeNews);

		return possibleCards[Random.Range(0, possibleCards.Count)];
	}

	// Get possible cards in a specific situation, among today's available ones
	List<Card> GetPossibleCards(CardFamily fam, bool mustBeFake)
	{
		List<Card> result = new List<Card>();

		for (int i = 0; i < cardsOfTheDay.Count; i++)
			if (cardsOfTheDay[i].family == fam && cardsOfTheDay[i].isFakeNews == mustBeFake)
				result.Add(cardsOfTheDay[i]);

		return result;
	}
}
