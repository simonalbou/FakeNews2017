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

	Transform _spawnroot = null;
	Vector3[] _spawnPositions = null;

	void Awake ()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		//StartCoroutine (SpawnAfterSeconds ());
	}

	IEnumerator SpawnAfterSeconds()
	{
		Spawn (Random.Range (5, 10));
		yield return new WaitForSeconds (0.2f);
		StartCoroutine (SpawnAfterSeconds ());
	}

	public void InitSpawn ()
	{
		_spawnroot = transform;



		int numberOfAngle = (int) (360f / angle);

		_spawnPositions = new Vector3[numberOfStair * numberOfAngle];
		int positionCount = 0;

		for (int h = 0; h < numberOfStair; h++)
		{
			float correctRadius = radius - 0.25f * h;
			for (int i = 0; i < numberOfAngle; i++)
			{
				Vector3 position = new Vector3 ();
				position.x = _spawnroot.position.x + correctRadius * Mathf.Sin (i * angle * Mathf.Deg2Rad);
				position.y = _spawnroot.position.y + heightSpace * h;
				position.z = _spawnroot.position.z + correctRadius * Mathf.Cos (i * angle * Mathf.Deg2Rad);

				_spawnPositions [positionCount] = position;
				positionCount++;
			}
		}
	}

	public void Spawn (int numberOfScreen)
	{
		// Reset previous screen
		for (int i = 0; i < _spawnroot.childCount; i++)
		{
			GameObject.Destroy (_spawnroot.GetChild (i).gameObject);
		}

		// Setup all posible position
		List<Vector3> pool = new List<Vector3> (_spawnPositions);
		numberOfScreen = pool.Count-1;
		for (int i = 0; i < numberOfScreen; i++)
		{
			//TODO use a pool
			GameObject screen = GameObject.Instantiate (screenPrefab, _spawnroot);

			int index = Random.Range (0, pool.Count);

			screen.transform.position = pool [index];

			//TODO use a 'Screen' object
			screen.GetComponent<Renderer> ().sharedMaterial = materials [Random.Range (0, materials.Count)];

			//Remove position form the possible list
			pool.RemoveAt (index);

			if (pool.Count < i)
				break;
		}
	}

	public Vector3[] GetPositions (int numberOfPosition)
	{
		Vector3[] positions = new Vector3[numberOfPosition];
		List<Vector3> pool = new List<Vector3> (_spawnPositions);

		numberOfPosition = Mathf.Min (numberOfPosition, pool.Count);

		for (int i = 0; i < numberOfPosition; i++)
		{
			int index = Random.Range (0, pool.Count);

			positions [i] = pool [index];

			pool.RemoveAt (index);
		}

		return positions;
	}
}
