using UnityEngine;
using System.Collections.Generic;

public class WorkerSpritesView : MonoBehaviour {

	Dictionary<Worker, GameObject> workerToGameObjectMap;

	public GameObject char_prefab;
	public GameObject character_parent;

	// FIXME use callback system to react to worker being created
	public void renderWorkers(World world)
	{
		workerToGameObjectMap = new Dictionary<Worker, GameObject> ();
		foreach (Worker worker in world.getWorkers()) {
			GameObject go = Instantiate (char_prefab);
			go.transform.SetParent (character_parent.transform);
			workerToGameObjectMap [worker] = go;
		}

	}

	public void Update()
	{
		foreach(var item in workerToGameObjectMap ) {
			Worker worker = item.Key;
			if (worker.isMoving ()) {
				GameObject go = item.Value;
				go.transform.position = new Vector3 (worker.X, worker.Y, -1);
			}
		}
			
	}
}
