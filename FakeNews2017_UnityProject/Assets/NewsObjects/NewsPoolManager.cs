using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsPoolManager : MonoBehaviour {

	// pooling
	public static NewsPoolManager instance;
	public ClickableNews[] pool;

	// day manager
	[System.NonSerialized]
	public int curDay, curActivatedNews;

	public void Awake()
	{
		instance = this;
		curDay = 1;
		curActivatedNews = 0;

		for (int i = 0; i < pool.Length; i++)
			pool[i].Kill();

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
		Vector3[] spawnPositions = new Vector3[newsNumber];

		// get la valeur des spawn positions

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

	public void ClickSomeNews()
	{
		curActivatedNews++;
		if(curActivatedNews == curDay && curDay < 10)
		{
			curDay++;
			curActivatedNews = 0;

			for (int i = 0; i < pool.Length; i++)
				pool[i].Kill();

			for (int i = 0; i < curDay*3; i++)
				SpawnNews(curDay*3);
		}
	}

	#endregion
}
