using UnityEngine;
using System.Collections.Generic;

public class FurnitureSpritesController : MonoBehaviour
{
	Dictionary<Furniture.TYPE, Sprite> spriteMap;
	Dictionary<Furniture, GameObject> furnitureToGameObjectMap;
	Dictionary<Job, GameObject> jobToGameObjectMap;

	public GameObject furnitures_parent;
	public Sprite job_placeholder_sprite;
	public Sprite wall_placeholder_sprite;

	void Start ()
	{
		spriteMap = new Dictionary<Furniture.TYPE, Sprite> ();
		// FIXME: load all the wall sprites
		furnitureToGameObjectMap = new Dictionary<Furniture, GameObject> ();
		jobToGameObjectMap = new Dictionary<Job, GameObject> ();
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
		});
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
		go.GetComponent<SpriteRenderer> ().sprite = wall_placeholder_sprite;
		// FIXME: look at neighbours to figure out what sprite to use
	}

}

