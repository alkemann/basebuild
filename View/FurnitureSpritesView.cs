using UnityEngine;
using System.Collections.Generic;
using System;

public class FurnitureSpritesView : MonoBehaviour
{
	Dictionary<string, Sprite> spriteMap;
	Dictionary<Furniture, GameObject> furnitureToGameObjectMap; // TODO pooling?
	Dictionary<Job, GameObject> jobToGameObjectMap; // TODO pooling?

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

		// Create game object and visualization for a Job
		GameObject job_go = new GameObject ();
		jobToGameObjectMap [job] = job_go;

		job_go.name = "Job_" + x + "_" + y;
		job_go.transform.position = new Vector3 (x, y, -1f);
		job_go.transform.SetParent (furnitures_parent.transform, true);
		job_go.AddComponent<SpriteRenderer> ().sprite = job_placeholder_sprite;

		// Make sure we remove game objects for jobs when they are complete
		// Or canceled
		Action<Job> onJobComplete = (j) => {
			jobToGameObjectMap.Remove (job);
			Destroy (job_go);
		};
		job.registerOnCompleteCallback(onJobComplete);
		job.registerOnCancelCallback (onJobComplete);

		// If the job is an install type, get ready to create visualization for
		// the furniture when it is installed
		if (job.type == Job.TYPE.INSTALL) {
			tile.registerOnFurnitureInstalled ((Tile t) => {
				Furniture furn = t.Furniture;
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
			if (tile_to_north != null && tile_to_north.Furniture != null && tile_to_north.Furniture.type == furn.type) {
				sprite_name += "N";
			}
			Tile tile_to_east = wc.getTileAt (x + 1, y);
			if (tile_to_east != null && tile_to_east.Furniture != null && tile_to_east.Furniture.type == furn.type) {
				sprite_name += "E";
			}
			Tile tile_to_south = wc.getTileAt (x, y - 1);
			if (tile_to_south != null && tile_to_south.Furniture != null && tile_to_south.Furniture.type == furn.type) {
				sprite_name += "S";
			}
			Tile tile_to_west = wc.getTileAt (x - 1, y);
			if (tile_to_west != null && tile_to_west.Furniture != null && tile_to_west.Furniture.type == furn.type) {
				sprite_name += "W";
			}
		}
		return spriteMap[sprite_name];
	}
}

