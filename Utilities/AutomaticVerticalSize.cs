using UnityEngine;
using System.Collections;

public class AutomaticVerticalSize : MonoBehaviour {

	public float childHeight = 35f;
	public float padding = 2f;

	void Start() {
		adjustSize ();
	}

	public void adjustSize ()
	{
		RectTransform tran = this.GetComponent<RectTransform> ();
		Vector2 size = tran.sizeDelta;
		int children = this.transform.childCount;
		size.y = children * childHeight + ((children - 1) * padding);
		tran.sizeDelta = size;
	}
}
