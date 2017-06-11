using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour 
{
	public static Spawner instance = null;

	public GameObject screenPrefab = null;

	public float angle = 20f;
	public float radius = 1f;
	public float heightSpace = 1.5f;
	public int numberOfStair = 4;
	public List<Material> materials;
	public int numberOfPart = 2;

	Transform _spawnroot = null;
	SpawnPoint[] _spawnPositions = null;

	List<List<List<Vector3>>> _stairToSpawns = new List<List<List<Vector3>>>();
	List<List<List<Vector3>>> _temp = new List<List<List<Vector3>>>();

	public class SpawnPoint
	{
		public int index;
		public Vector3 position;
	}

	void Awake ()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		//InitSpawn ();
		//StartCoroutine (SpawnAfterSeconds ());
	}

	/*
	IEnumerator SpawnAfterSeconds()
	{

		Spawn (Random.Range(2, 12));
		yield return new WaitForSeconds (0.5f);
		StartCoroutine (SpawnAfterSeconds ());

	}
	*/

	public void InitSpawn ()
	{
		_spawnroot = transform;

		int numberOfAngle = (int) (360f / angle);

		_spawnPositions = new SpawnPoint[numberOfStair * numberOfAngle];
		int positionCount = 0;

		for (int h = 0; h < numberOfStair; h++)
		{
			List<Vector3> stairSpawn = new List<Vector3> ();

			float correctRadius = radius - 0.25f * h;
			for (int i = 0; i < numberOfAngle; i++)
			{
				Vector3 position = new Vector3 ();
				position.x = _spawnroot.position.x + correctRadius * Mathf.Sin (i * angle * Mathf.Deg2Rad);
				position.y = _spawnroot.position.y + heightSpace * h;
				position.z = _spawnroot.position.z + correctRadius * Mathf.Cos (i * angle * Mathf.Deg2Rad);

				stairSpawn.Add (position);

				positionCount++;
			}

			_stairToSpawns.Add (SplitListToNVector3Array<Vector3> (stairSpawn, numberOfPart));
		}
		/*
		for (int i = 0; i < _stairToSpawns.Count; i ++)
			for (int j = 0; j < _stairToSpawns[i].Count; j++)
				for (int k = 0; k < _stairToSpawns[i][j].Count; k++)
				{
					GameObject go = GameObject.Instantiate (screenPrefab);
					go.transform.position = _stairToSpawns [i] [j] [k];
					go.GetComponent<Renderer> ().sharedMaterial = materials [j];
					go.name = "stair_" + i + "part_" + j + "index_" + k;
					go.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
				}
		*/
	}

	private void CopyPosition()
	{
		_temp = new List<List<List<Vector3>>> ();

		for (int stairIndex = 0; stairIndex < _stairToSpawns.Count; stairIndex++)
		{
			//Add a stair (contains parts, contains poistions)
			_temp.Add(new List<List<Vector3>>());


			for (int partIndex = 0; partIndex < _stairToSpawns[stairIndex].Count; partIndex++)
			{
				// Add a part to the stairs.
				_temp [stairIndex].Add (new List<Vector3> ());

				for (int positionIndex = 0; positionIndex < _stairToSpawns[stairIndex][partIndex].Count; positionIndex++)
				{
					_temp [stairIndex] [partIndex].Add(_stairToSpawns[stairIndex][partIndex][positionIndex]);
				}
			}
		}
	}

	private List<List<T>> SplitListToNVector3Array<T> ( List<T> list, int part)
	{
		int itemCountPerPart = (int)(list.Count / part);
		int currentPart = 0;
		int count = 0;

		List<List<T>> parts = new List<List<T>>();

		parts.Add (new List<T> ());

		for (int i = 0; i < list.Count; i++)
		{
			parts [currentPart].Add (list [i]);
			count++;

			if (count == itemCountPerPart && currentPart < part-1)
			{
				parts.Add (new List<T> ());
				currentPart++;
				count = 0;
			}
		}

		return parts;
	}

	public void Spawn (int numberOfScreen)
	{
		// Reset previous screen
		for (int i = 0; i < _spawnroot.childCount; i++)
		{
			GameObject.Destroy (_spawnroot.GetChild (i).gameObject);
		}

		// Setup all posible position

		Vector3[] positions = GetPositions (numberOfScreen);

		for (int i = 0; i < numberOfScreen; i++)
		{
			//TODO use a pool
			GameObject screen = GameObject.Instantiate (screenPrefab, _spawnroot);

			screen.transform.position = positions [i];
			screen.name = "screen_" + i;
			//TODO use a 'Screen' object
			screen.GetComponent<Renderer> ().sharedMaterial = materials [materials.Count-1];
		}
	}

	public Vector3[] GetPositions (int numberOfPosition)
	{
		Vector3[] positions = new Vector3[numberOfPosition];

		CopyPosition ();

		int currentPart = Random.Range(0, numberOfPart);

		for (int i = 0; i < numberOfPosition; i++)
		{
			int randomStair = Random.Range (0, _temp.Count);

			if (currentPart >= _temp [randomStair].Count)
			{
				currentPart = 0;			
			}
			//int randomPart = Random.Range (currentPart, _temp [randomStair].Count);
			int randomPart = currentPart;
			currentPart += 1;

			int randomIndex = Random.Range (0, _temp [randomStair] [randomPart].Count);

			positions [i] = _temp [randomStair] [randomPart] [randomIndex];

			_temp [randomStair] [randomPart].RemoveAt (randomIndex);

			if (_temp [randomStair] [randomPart].Count == 0)
			{
				_temp [randomStair].RemoveAt (randomPart);

				if (_temp [randomStair].Count == 0)
				{
					_temp.RemoveAt (randomStair);
				}
			}			
		}

		return positions;
	}
}
