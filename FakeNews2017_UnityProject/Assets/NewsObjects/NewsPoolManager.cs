using System.Collections;
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
	public StareCamera inputHolder;
	public ClickableNews[] pool;

	// day manager
	[System.NonSerialized]
	public int curDay, curActivatedNews;
	[System.NonSerialized]
	public float evenementialScore, alarmistScore, nostalgiaScore, hipsterScore, alarmWeight, eventWeight, nostalWeight, hipsterWeight, invTotal;
	[System.NonSerialized]
	public List<Card> cardsOfTheDay, possibleCards;

	[Header("Gameplay")]
	public float startScore = 4;
	public float multiplierPerArticle = 1;
	public float fakeNewsMultiplier = 2;
	public BubbleDay[] days;
	public Card[] allRandomCards;

	private int state;
	private float cooldown;
	// used when moving day
	private int forced, randomNews, total, spawned;
	private Vector3[] spawnPositions;

	public void Awake()
	{
		cardsOfTheDay = new List<Card>();
		possibleCards = new List<Card>();
		instance = this;
		curDay = 0;
		curActivatedNews = 0;

		evenementialScore = alarmistScore = nostalgiaScore = hipsterScore = startScore;

		spawner.InitSpawn ();

		MoveOntoNextDay();
		//StartCoroutine(AdvanceToNextDay());
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
		// An animation has been launched in ClickableNews.cs. From this singleton, click input should be shutdown.
		inputHolder.PutOnCooldown(1.0f);

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
			if (curDay < days.Length-1)
				MoveOntoNextDay(); //StartCoroutine(AdvanceToNextDay());
		}
	}

	// Faked coroutine
	public void Update()
	{
		cooldown -= Time.deltaTime;

		if (state == 0)
		{
			if(cooldown < 0)
			{
				for (int i = 0; i < pool.Length; i++)
					pool[i].Kill();

				state++;
				cooldown = 0.2f;
				return;
			}
		}

		if (state == 1)
		{
			if (cooldown < 0)
			{
				// Determine how many news there are, and where they should spawn
				forced = days[curDay].forcedNews == null ? 0 : days[curDay].forcedNews.Count;
				randomNews = Mathf.Max(days[curDay].numberOfRandomNews, 0);
				total = forced + randomNews;
				spawnPositions = spawner.GetPositions(total);

				spawned = 0;

				state++;
				cooldown = 0.3f;
				return;
			}
		}

		if (state == 2)
		{
			if (forced == 0) { state++; return; }
			else if (cooldown < 0)
			{
				ClickableNews news = GetNews();
				news.Load(days[curDay].forcedNews[forced-1]);
				news.Spawn(spawnPositions[spawned]);
				spawned++;
				forced--;
				cooldown = 0.3f;
			}
		}

		if (state == 3)
		{
			if (randomNews > 0)
			{
				cardsOfTheDay.Clear();
				cardsOfTheDay.TrimExcess();
				//cardsOfTheDay = new List<Card>();
				for (int i = 0; i < allRandomCards.Length; i++)
				{
					if (curDay == 14) // TEMP
					{
						if (allRandomCards[i].firstDay == 15)
							cardsOfTheDay.Add(allRandomCards[i]);
					}
					else
					{
						if (allRandomCards[i].firstDay <= curDay && allRandomCards[i].lastDay >= curDay)
							cardsOfTheDay.Add(allRandomCards[i]);
					}
				}

				state++;
			}
			else { state = 5; return; }
		}

		if (state == 4)
		{
			if (randomNews == 0) { state++; return; }
			else if (cooldown < 0)
			{
				ClickableNews news = GetNews();
				news.Load(GetRandomCard());
				news.Spawn(spawnPositions[spawned]);
				spawned++;
				randomNews--;
				cooldown = 0.3f;
			}
		}

		if (state == 5)
		{
			state = 0;
			inputHolder.PutOnCooldown(1);
			enabled = false;
		}
	}

	void MoveOntoNextDay()
	{
		// First, freeze inputs.
		inputHolder.PutOnCooldown(100.0f);

		cooldown = 2.0f;
		state = 0;
		enabled = true;
	}

	// Called at the end of each day
	/**
	public IEnumerator AdvanceToNextDay()
	{
		// First, freeze inputs.
		inputHolder.PutOnCooldown(100.0f);

		// Then kill all old news.
		yield return new WaitForSeconds(2.0f);
		for (int i = 0; i < pool.Length; i++)
			pool[i].Kill();

		yield return new WaitForSeconds(0.2f);

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
				yield return new WaitForSeconds(0.3f);
			}

		// Spawning random news : also refreshing pool of today's cards
		if (randomNews > 0)
		{
			cardsOfTheDay.Clear();
			cardsOfTheDay.TrimExcess();
			//cardsOfTheDay = new List<Card>();
			for (int i = 0; i < allRandomCards.Length; i++)
				if (allRandomCards[i].firstDay <= curDay && allRandomCards[i].lastDay >= curDay)
					cardsOfTheDay.Add(allRandomCards[i]);

			for (int i = 0; i < randomNews; i++)
			{
				ClickableNews news = GetNews();
				news.Load(GetRandomCard());
				news.Spawn(spawnPositions[spawned]);
				spawned++;
				yield return new WaitForSeconds(0.3f);
			}
		}

		// Inputs are back.
		inputHolder.PutOnCooldown(1);

		yield return null;
	}
	//*/

	// Rolls a card for random news
	public Card GetRandomCard()
	{
		CardFamily family;
		float roll = Random.value;
		if (roll < alarmWeight) family = CardFamily.ALARMISTE;
		else if (roll < alarmWeight + hipsterWeight) family = CardFamily.HIPSTER;
		else if (roll < alarmWeight + hipsterWeight + nostalWeight) family = CardFamily.NOSTALGIE;
		else family = CardFamily.EVENEMENTIEL;

		float fakeNewsChance = -1.0f;
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
		possibleCards.Clear();

		for (int i = 0; i < cardsOfTheDay.Count; i++)
			if (cardsOfTheDay[i].family == fam && cardsOfTheDay[i].isFakeNews == mustBeFake)
				possibleCards.Add(cardsOfTheDay[i]);

		return possibleCards;
	}
}
