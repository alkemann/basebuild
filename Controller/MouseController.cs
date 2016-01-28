using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour {

	public GameObject cursor_go;

	Camera cam;
	Vector3 lastFramePosition;
	Vector3 currFramePosition;

	void Start () {
		cam = Camera.main;
	}
	
	void Update () {
		currFramePosition = cam.ScreenToWorldPoint (Input.mousePosition);
		currFramePosition.z = 0;

		cameraMovement ();
		cursor ();

		lastFramePosition =  cam.ScreenToWorldPoint (Input.mousePosition);
		lastFramePosition.z = 0;
	}

	void cameraMovement ()
	{
		if ( Input.GetMouseButton(1) || Input.GetMouseButton(2) ) { // right or middle button
			cam.transform.Translate (lastFramePosition - currFramePosition);
		}
	}

	void cursor ()
	{
		// cursor_go.transform.position = currFramePosition; // moves  cursor to freeform cursor
		Tile t = getTileAtWorldCoord(currFramePosition);
		if (t == null) {
			cursor_go.SetActive (false);
		} else {
			cursor_go.SetActive (true);
			cursor_go.transform.position = new Vector3 (t.x, t.y, 0);
		}
	}


	Tile getTileAtWorldCoord(Vector3 coord)
	{
		int x = Mathf.FloorToInt (currFramePosition.x);
		int y = Mathf.FloorToInt (currFramePosition.y);
		return GetComponent<WorldController> ().getTileAt (x, y);
	}
}
