using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum NewsType { Evenemential, Alarmist, Nostalgia, Hipster, Meta }

public class ClickableNews : MonoBehaviour {

	public Transform self;
	public Collider selfCollider;
	public GameObject content;
	public Image selfRenderer;
	public Text text;
	public UnityEvent OnSelected;
	public NewsType newsType;

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
		NewsPoolManager.instance.ClickSomeNews(newsType);
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
}
