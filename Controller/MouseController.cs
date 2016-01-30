using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{

	public GameObject cursor_prefab;
	public GameObject preview_parent;

	Camera cam;
	Vector3 lastFramePosition;
	Vector3 currFramePosition;
	Vector3 dragStartPosition;

	List<GameObject> dragPreviewObjects;

	Tile.TYPE buildMode = Tile.TYPE.FLOOR;

	Furniture.TYPE creatingFurniture;

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
		// if over ui, dont do this thing
		if (EventSystem.current.IsPointerOverGameObject ()) {
			return;
		}


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
				if (World.isCoordinatesWithinBuildWorld(x, y) == false)
					continue; // no tile here, out of map

				// End drag;
				if (Input.GetMouseButtonUp (0)) {
					if (buildMode != Tile.TYPE.NONE) {
						GetComponent<WorldController> ().buildTile (buildMode, x, y);
					}
					if (creatingFurniture != Furniture.TYPE.NONE) {
						Job job = GetComponent<WorldController> ().createInstallJobAt (Furniture.TYPE.WALL, x, y);
						if (job != null) {
							FurnitureSpritesController fsc = GetComponent<FurnitureSpritesController> ();
							fsc.jobCreated (job);
						}
					}
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

	public void SetBuildMode(string type)
	{
		creatingFurniture = Furniture.TYPE.NONE;
		switch (type) {
		case "floor":
			buildMode = Tile.TYPE.FLOOR;
			break;
		case "bulldoze":
			buildMode = Tile.TYPE.EMPTY;
			break;

		default:
			break;
		}
	}

	public void SetCreateMode(string type)
	{
		buildMode = Tile.TYPE.NONE;
		switch (type) {
		case "wall":
			creatingFurniture = Furniture.TYPE.WALL;
			break;
		default:
			break;
		}
	}

}
