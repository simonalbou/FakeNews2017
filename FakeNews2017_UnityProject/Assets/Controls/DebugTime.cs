using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTime : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Time.timeScale = Input.GetMouseButton(1) ? 10 : 1;
	}
}
