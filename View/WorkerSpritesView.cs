using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorkerSpritesView : MonoBehaviour {

	Dictionary<Worker, GameObject> workerToGameObjectMap;

	List<Sprite> spriteMap;
	public GameObject char_prefab;
	public GameObject character_parent;

	public void Start()
	{
		workerToGameObjectMap = new Dictionary<Worker, GameObject> ();
		GetComponent<WorldController> ().world.registerOnWorkerCreated (onWorkerCreated);

		spriteMap = new List<Sprite> ();
		foreach (Sprite s in Resources.LoadAll<Sprite> ("Images/Characters")) {
			spriteMap.Add(s);
		}
	}

	public void Update()
	{
		foreach(var item in workerToGameObjectMap ) {
			Worker worker = item.Key;
			GameObject go = item.Value;
			if (worker.isMoving ()) {
				go.transform.position = new Vector3 (worker.X, worker.Y, -1);
			}
		}
	}

	void onWorkerCreated(Worker worker)
	{
		GameObject go = Instantiate (char_prefab);
		go.transform.position =  new Vector3 (worker.X, worker.Y, -1);
		go.transform.SetParent (character_parent.transform);
		workerToGameObjectMap [worker] = go;

		int sprite_id = (int) Mathf.Round (Random.Range (0, spriteMap.Count));
		go.GetComponent<SpriteRenderer> ().sprite = spriteMap [ sprite_id ];
//		worker.registerOnStateChangeCallback (onWorkerStateChanged);
	}

	void onWorkerStateChanged(Worker worker)
	{
//		GameObject go = workerToGameObjectMap [worker];
		if (worker.isMoving ()) {
			//moving
		} else if (worker.isWorking ()) {
			// working
		} else {
			// resting
		}
	}
}
