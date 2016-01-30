﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseController : MonoBehaviour
{

	public GameObject cursor_prefab;
	public GameObject preview_parent;

	Camera cam;
	Vector3 lastFramePosition;
	Vector3 currFramePosition;
	Vector3 dragStartPosition;

	List<GameObject> dragPreviewObjects;

	void Start ()
	{
		cam = Camera.main;
		dragPreviewObjects = new List<GameObject> ();
	}

	void Update ()
	{
		currFramePosition = cam.ScreenToWorldPoint (Input.mousePosition);
		currFramePosition.z = 0;

		cameraMovement ();
		interaction ();

		lastFramePosition = cam.ScreenToWorldPoint (Input.mousePosition);
		lastFramePosition.z = 0;
	}

	void cameraMovement ()
	{
		if (Input.GetMouseButton (1) || Input.GetMouseButton (2)) { // right or middle button
			cam.transform.Translate (lastFramePosition - currFramePosition);
		}
		cam.orthographicSize -= cam.orthographicSize * Input.GetAxis ("Mouse ScrollWheel") * 0.8f;
		cam.orthographicSize = Mathf.Clamp (cam.orthographicSize, 3f, 30f);
	}

	Tile getTileAtWorldCoord (Vector3 coord)
	{
		int x = Mathf.FloorToInt (coord.x);
		int y = Mathf.FloorToInt (coord.y);
		return GetComponent<WorldController> ().getTileAt (x, y);
	}

	void interaction ()
	{
		
		// quick grab coordinate of a tile
		if (Input.GetMouseButtonDown (4)) {
			Debug.Log (string.Format ("Starts at {0},{1}", Mathf.FloorToInt (currFramePosition.x), Mathf.FloorToInt (currFramePosition.y)));
		}

		int start_x = Mathf.FloorToInt (dragStartPosition.x);
		int end_x = Mathf.FloorToInt (currFramePosition.x);
		if (end_x < start_x) {
			int tmp = end_x;
			end_x = start_x;
			start_x = tmp;
		}
		int start_y = Mathf.FloorToInt (dragStartPosition.y);
		int end_y = Mathf.FloorToInt (currFramePosition.y);
		if (end_y < start_y) {
			int tmp = end_y;
			end_y = start_y;
			start_y = tmp;
		}

		// Start drag
		if (Input.GetMouseButtonDown (0)) {
			dragStartPosition = currFramePosition;
		}

		// clean up preview
		while (dragPreviewObjects.Count > 0) {
			GameObject go = dragPreviewObjects [0];
			dragPreviewObjects.RemoveAt (0);
			SimplePool.Despawn (go);
		}

		for (int x = start_x; x <= end_x; x++) {
			for (int y = start_y; y <= end_y; y++) {
				Tile t = getTileAtWorldCoord (new Vector3 (x, y, 0));
				if (t == null)
					continue; // no tile here, out of map

				// End drag;
				if (Input.GetMouseButtonUp (0)) {
					// Build all
					t.type = Tile.TYPE.FLOOR;
				} else
					// Dragging
					if (Input.GetMouseButton (0) && !Input.GetMouseButtonDown (0)) {
					// Display a preview of drag area
					GameObject go = SimplePool.Spawn (cursor_prefab, new Vector3 (x, y, 0), Quaternion.identity);
					go.transform.SetParent (preview_parent.transform, true);
					dragPreviewObjects.Add (go);
				}
			}
		}
	}
}
