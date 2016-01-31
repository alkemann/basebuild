using UnityEngine;
using System.Collections.Generic;

public class WorkerSpritesView : MonoBehaviour {

	Dictionary<Worker, GameObject> workerToGameObjectMap;

	public GameObject char_prefab;
	public Sprite moving_sprite;
	public Sprite resting_sprite;
	public Sprite working_sprite;
	public GameObject character_parent;

	public void Start()
	{
		workerToGameObjectMap = new Dictionary<Worker, GameObject> ();
		GetComponent<WorldController> ().world.registerOnWorkerCreated (onWorkerCreated);

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
		go.transform.SetParent (character_parent.transform);
		workerToGameObjectMap [worker] = go;
		worker.registerOnStateChangeCallback (onWorkerStateChanged);
	}

	void onWorkerStateChanged(Worker worker)
	{
		GameObject go = workerToGameObjectMap [worker];
		if (worker.isMoving ()) {
			go.transform.position = new Vector3 (worker.X, worker.Y, -1);
			go.GetComponent<SpriteRenderer> ().sprite = moving_sprite;
		} else if (worker.isWorking ()) {
			go.GetComponent<SpriteRenderer> ().sprite = working_sprite;
		} else {
			go.GetComponent<SpriteRenderer> ().sprite = resting_sprite;
		}
	}
}
