using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickableNews : MonoBehaviour {

	public Transform self;
	public Collider selfCollider;
	public SpriteRenderer selfRenderer;
	public UnityEvent OnSelected;

	[System.NonSerialized]
	public bool available;

	public void BillboardToZero() { self.LookAt(Vector3.zero); }

	public void Activate()
	{
		selfCollider.enabled = false;
		selfRenderer.color = Color.green;
		NewsPoolManager.instance.ClickSomeNews();
	}

	public void Kill()
	{
		available = true;
		selfCollider.enabled = false;
		selfRenderer.enabled = false;
	}

	public void Spawn(Vector3 pos)
	{
		selfRenderer.enabled = true;
		selfRenderer.color = Color.white;
		self.position = pos;
		BillboardToZero();
		selfCollider.enabled = true;
		available = false;
	}
}
