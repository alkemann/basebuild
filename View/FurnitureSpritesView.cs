using UnityEngine;
using System.Collections.Generic;
using System;

public class FurnitureSpritesView : MonoBehaviour
{
	Dictionary<string, Sprite> spriteMap;
	Dictionary<Furniture, GameObject> furnitureToGameObjectMap;
	Dictionary<Job, GameObject> jobToGameObjectMap;

	public GameObject job_prefab;
	public GameObject furnitures_parent;
	public GameObject jobs_parent;

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

		GameObject job_go = SimplePool.Spawn (job_prefab, new Vector3 (x, y, 0), Quaternion.identity);
		jobToGameObjectMap [job] = job_go;

		job_go.name = "Job_" + x + "_" + y;
		job_go.transform.position = new Vector3 (x, y, -1f);
		job_go.transform.SetParent (jobs_parent.transform, true);

		// Make sure we remove game objects for jobs when they are complete
		// Or canceled
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

				furn.RegisterOnChangeCallback (onFurnitureChanged);
				onFurnitureChanged (furn); // call it once to do the first sprite set

				furn.RegisterOnUnInstallCallback (onFurnitureUninstall);
			});
		}
	}

	void onJobComplete(Job job)
	{
		job.unregisterOnCompleteCallback(onJobComplete);
		job.unregisterOnCancelCallback(onJobComplete);
		GameObject job_go = jobToGameObjectMap [job];
		jobToGameObjectMap.Remove (job);
		SimplePool.Despawn (job_go);
	}

	void onFurnitureChanged (Furniture furn)
	{
		GameObject go = furnitureToGameObjectMap [furn];
		go.GetComponent<SpriteRenderer> ().sprite = getSpriteForFurniture (furn);
	}

	void onFurnitureUninstall (Furniture furn)
	{
		if (furnitureToGameObjectMap.ContainsKey (furn) == false) {
//			Debug.Log ("Furn removal called an extra time!");
			// FIXME: Figure out why i am called twice for each furniture being removed
			return;
		}
		GameObject go = furnitureToGameObjectMap [furn];
		furn.UnRegisterOnChangeCallback (onFurnitureChanged);
		furn.UnRegisterOnUnInstallCallback (onFurnitureUninstall);
		furnitureToGameObjectMap.Remove (furn);
		Destroy (go);
	}

	Sprite getSpriteForFurniture (Furniture furn)
	{
		WorldController wc = WorldController.Instance;
		Tile tile = furn.tile;
		int x = tile.X;
		int y = tile.Y;
		string sprite_name = "default";
		switch (furn.type) {
		case Furniture.TYPE.WALL:
			sprite_name = "WALL_";
			Tile tile_to_north = wc.getTileAt (x, y + 1);
			if (tile_to_north != null && tile_to_north.hasLinkedInstallation()) {
				sprite_name += "N";
			}
			Tile tile_to_east = wc.getTileAt (x + 1, y);
			if (tile_to_east != null && tile_to_east.hasLinkedInstallation()) {
				sprite_name += "E";
			}
			Tile tile_to_south = wc.getTileAt (x, y - 1);
			if (tile_to_south != null && tile_to_south.hasLinkedInstallation()) {
				sprite_name += "S";
			}
			Tile tile_to_west = wc.getTileAt (x - 1, y);
			if (tile_to_west != null && tile_to_west.hasLinkedInstallation()) {
				sprite_name += "W";
			}
			break;
		case Furniture.TYPE.DOOR:
			sprite_name = "DOOR_";
			tile_to_north = wc.getTileAt (x, y + 1);
			if (tile_to_north != null && tile_to_north.hasLinkedInstallation()) {
				sprite_name += "V";
			} else {
				sprite_name += "H";
			}
			sprite_name += "_CLOSED";
			break;
		case Furniture.TYPE.TERMINAL:
			sprite_name = "TERMINAL";
			break;
		case Furniture.TYPE.SPAWNER:
			sprite_name = "SPAWNER";
			break;
		}
		if (spriteMap.ContainsKey(sprite_name)) {
			return spriteMap[sprite_name];
		}
		throw new Exception ("Unknown sprite: " + sprite_name);
	}
}

