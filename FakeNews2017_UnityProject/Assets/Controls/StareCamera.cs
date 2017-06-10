using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StareCamera : MonoBehaviour
{
	public float range;
	public LayerMask layerMask;

	public Transform self;
	private RaycastHit hit;

	void Update ()
	{
		if (Input.GetMouseButtonDown(0)) Click();
	}

	void Click()
	{
		Ray ray = new Ray(self.position, self.forward);

		if (!Physics.Raycast(ray, out hit, range, layerMask)) return;
		
		Collider col = hit.collider;
		ClickableNews cn = col.GetComponent<ClickableNews>();
		if (!cn) return;
		if (cn.OnSelected != null) cn.OnSelected.Invoke();
	}
}
