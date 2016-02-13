using UnityEngine;
using System.Collections.Generic;

public class TileClickable : MonoBehaviour {

	public Tile tile;
	Stack<Tile> path;
	Worker activeWorker;

	public void OnMouseUp()
	{
		path = null;
		activeWorker = null;
		Debug.Log (string.Format("Clicked {0} {1},{2}", this.name, tile.X, tile.Y));


		List<Worker> workers = WorldController.Instance.world.getWorkers ();
		foreach (Worker w in workers) {
			sendWorkerToThisTile (w);
			break;
		}
	}

	void sendWorkerToThisTile (Worker w)
	{
		w.setDestination(WorldController.Instance.world.getTileAt(tile.X, tile.Y));
		this.path = w.findPathTo (tile.X, tile.Y);
//		if (this.path != null) {
//			activeWorker = w;
//		}
	}

	public void Update()
	{
		if (path!= null) {
			int currTile = 0;
			Tile[] pathArr = this.path.ToArray ();
			while (currTile < pathArr.Length - 1) {
				Vector3 start = new Vector3 (pathArr[currTile].X+0.5f, pathArr[currTile].Y+0.5f, -3);
				Vector3 end = new Vector3 (pathArr[currTile+1].X+0.5f, pathArr[currTile+1].Y+0.5f, -3);
				Debug.DrawLine (start, end, Color.red);


				currTile++;
			}
		}
		if (path != null && activeWorker != null) {
			// move the worker
		}
	}
}
