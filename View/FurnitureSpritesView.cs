using UnityEngine;
using System.Collections.Generic;

public class FurnitureSpritesView : MonoBehaviour
{
	Dictionary<string, Sprite> spriteMap;
	Dictionary<Furniture, GameObject> furnitureToGameObjectMap;
	Dictionary<Job, GameObject> jobToGameObjectMap;

	public GameObject furnitures_parent;
	public Sprite job_placeholder_sprite;
	public Sprite wall_placeholder_sprite;

	void Start ()
	{
		spriteMap = new Dictionary<string, Sprite> ();
		furnitureToGameObjectMap = new Dictionary<Furniture, GameObject> ();
		jobToGameObjectMap = new Dictionary<Job, GameObject> ();

		loadFurnituresSprites ();
	}

	void loadFurnituresSprites ()
	{
		foreach (Sprite s in Resources.LoadAll<Sprite> ("Images/Furnitures")) {
			spriteMap [s.name] = s;
		}
	}

	public void jobCreated (Job job)
	{
		Tile tile = job.tile;
		int x = tile.x;
		int y = tile.y;

		GameObject job_go = new GameObject ();
		job_go.name = "Job_" + x + "_" + y;
		job_go.transform.position = new Vector3 (x, y, -1f);
		job_go.transform.SetParent (furnitures_parent.transform, true);
		job_go.AddComponent<SpriteRenderer> ().sprite = job_placeholder_sprite;
		AutoWorker aw = job_go.AddComponent<AutoWorker> ();
		aw.job = job;
		job.registerOnCompleteCallback(onJobComplete);
		jobToGameObjectMap [job] = job_go;
		tile.registerOnJobCompleteCallback ((t) => {
			Furniture furn = job.furniture;
			GameObject furn_go = new GameObject ();
			furn_go.name = "Furn_" + x + "_" + y;
			furn_go.transform.position = new Vector3 (x, y, -1f);
			furn_go.transform.SetParent (furnitures_parent.transform, true);
			furn_go.AddComponent<SpriteRenderer> ();
			furnitureToGameObjectMap [furn] = furn_go;

			furn.registerOnChangeCallback(onFurnitureChanged);
			onFurnitureChanged(furn); // call it once to do the first sprite set
			// trigger onFurnitureChanged for the 4 neighbors;

			if (furn.linkedObject) {
				// Linked object, we need to upgrade neighbours
				WorldController wc = GetComponent<WorldController> ();
				Tile tile_to_north = wc.getTileAt (x, y + 1);
				if (tile_to_north != null && tile_to_north.installedFurniture != null && tile_to_north.installedFurniture.type == furn.type) 
					tile_to_north.installedFurniture.neighbourChanged (tile);
				Tile tile_to_east = wc.getTileAt (x + 1, y);
				if (tile_to_east != null && tile_to_east.installedFurniture != null && tile_to_east.installedFurniture.type == furn.type) 
					tile_to_east.installedFurniture.neighbourChanged (tile);
				Tile tile_to_south = wc.getTileAt (x, y - 1);
				if (tile_to_south != null && tile_to_south.installedFurniture != null && tile_to_south.installedFurniture.type == furn.type) 
					tile_to_south.installedFurniture.neighbourChanged (tile);
				Tile tile_to_west = wc.getTileAt (x - 1, y);
				if (tile_to_west != null && tile_to_west.installedFurniture != null && tile_to_west.installedFurniture.type == furn.type) 
					tile_to_west.installedFurniture.neighbourChanged (tile);
			}
		});
	}

	public void onJobComplete(Job job)
	{
		GameObject job_go = jobToGameObjectMap [job];
		jobToGameObjectMap.Remove (job);
		Destroy (job_go);
	}

	// FIXME This can only set wall sprite
	void onFurnitureChanged (Furniture furn)
	{
		GameObject go = furnitureToGameObjectMap [furn];

		string sprite_name = "default";
		if (furn.linkedObject) {

			WorldController wc = GetComponent<WorldController> ();

			Tile tile = furn.tile;
			int x = tile.x;
			int y = tile.y;
			sprite_name = "walls_";

			Tile tile_to_north = wc.getTileAt (x, y + 1);
			if (tile_to_north != null && tile_to_north.installedFurniture != null && tile_to_north.installedFurniture.type == furn.type) {
				sprite_name += "N";
			}
			Tile tile_to_east = wc.getTileAt (x + 1, y);
			if (tile_to_east != null && tile_to_east.installedFurniture != null && tile_to_east.installedFurniture.type == furn.type) {
				sprite_name += "E";
			}
			Tile tile_to_south = wc.getTileAt (x, y - 1);
			if (tile_to_south != null && tile_to_south.installedFurniture != null && tile_to_south.installedFurniture.type == furn.type) {
				sprite_name += "S";
			}
			Tile tile_to_west = wc.getTileAt (x - 1, y);
			if (tile_to_west != null && tile_to_west.installedFurniture != null && tile_to_west.installedFurniture.type == furn.type) {
				sprite_name += "W";
			}
		}

		go.GetComponent<SpriteRenderer> ().sprite = spriteMap[sprite_name];
	}

}

