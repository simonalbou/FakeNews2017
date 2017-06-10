using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsPoolManager : MonoBehaviour {

	// pooling
	public static NewsPoolManager instance;
	public Spawner spawner;
	public ClickableNews[] pool;

	// day manager
	[System.NonSerialized]
	public int curDay, curActivatedNews;
	[System.NonSerialized]
	public float evenementialScore, alarmistScore, nostalgiaScore, hipsterScore;

	[Header("Gameplay")]
	public float startScore = 4;

	public void Awake()
	{
		instance = this;
		curDay = 1;
		curActivatedNews = 0;

		for (int i = 0; i < pool.Length; i++)
			pool[i].Kill();

		evenementialScore = alarmistScore = nostalgiaScore = hipsterScore = startScore;

		spawner.InitSpawn ();

		SpawnNews(1);
	}

	#region pooling

	public ClickableNews GetNews()
	{
		for (int i = 0; i < pool.Length; i++)
			if (pool[i].available) return pool[i];

		return null;
	}

	public void SpawnNews(int newsNumber)
	{
		Vector3[] spawnPositions = spawner.GetPositions(newsNumber);

		Vector3 pos = GetSpawnPos();
		for (int i = 0; i < newsNumber; i++)
			GetNews().Spawn(spawnPositions[i]);
	}

	Vector3 GetSpawnPos()
	{
		float x = Random.value * 10 + 10;
		float y = Random.value * 10 + 10;
		float z = Random.value * 10 + 10;
		if (Random.value < 0.5f) x *= -1;
		if (Random.value < 0.5f) y *= -1;
		if (Random.value < 0.5f) z *= -1;
		return new Vector3(x, y, z);
	}

	#endregion

	#region day manager

	public void ClickSomeNews(NewsType nt)
	{
		curActivatedNews++;

		if (nt == NewsType.Alarmist) alarmistScore += curDay;
		if (nt == NewsType.Evenemential) evenementialScore += curDay;
		if (nt == NewsType.Hipster) hipsterScore += curDay;
		if (nt == NewsType.Nostalgia) nostalgiaScore += curDay;

		if (curActivatedNews == curDay && curDay < 10)
		{
			curDay++;
			curActivatedNews = 0;

			for (int i = 0; i < pool.Length; i++)
				pool[i].Kill();

			SpawnNews(curDay*3);
		}
	}

	#endregion
}
