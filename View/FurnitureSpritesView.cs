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
		WorldController.Instance.world.registOnJobCreated (jobCreated);
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
		int x = tile.X;
		int y = tile.Y;

		GameObject job_go = new GameObject ();
		job_go.name = "Job_" + x + "_" + y;
		job_go.transform.position = new Vector3 (x, y, -1f);
		job_go.transform.SetParent (furnitures_parent.transform, true);
		job_go.AddComponent<SpriteRenderer> ().sprite = job_placeholder_sprite;
		job.registerOnCompleteCallback(onJobComplete);
		jobToGameObjectMap [job] = job_go;
		if (job.furniture != null) {
			tile.registerOnJobCompleteCallback ((t) => {
				Furniture furn = job.furniture;
				GameObject furn_go = new GameObject ();
				furn_go.name = "Furn_" + x + "_" + y;
				furn_go.transform.position = new Vector3 (x, y, -1f);
				furn_go.transform.SetParent (furnitures_parent.transform, true);
				furn_go.AddComponent<SpriteRenderer> ();
				furnitureToGameObjectMap [furn] = furn_go;

				furn.registerOnChangeCallback (onFurnitureChanged);
				onFurnitureChanged (furn); // call it once to do the first sprite set

			});
		}
	}

	public void onJobComplete(Job job)
	{
		GameObject job_go = jobToGameObjectMap [job];
		jobToGameObjectMap.Remove (job);
		Destroy (job_go);
	}

	void onFurnitureChanged (Furniture furn)
	{
		GameObject go = furnitureToGameObjectMap [furn];
		go.GetComponent<SpriteRenderer> ().sprite = getSpriteForFurniture (furn);
	}

	Sprite getSpriteForFurniture (Furniture furn)
	{
		// FIXME This can only set wall sprite
		string sprite_name = "default";
		if (furn.linkedObject) {
			WorldController wc = GetComponent<WorldController> ();
			Tile tile = furn.tile;
			int x = tile.X;
			int y = tile.Y;
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
		return spriteMap[sprite_name];
	}
}

