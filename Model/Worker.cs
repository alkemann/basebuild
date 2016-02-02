using UnityEngine;
using System.Collections.Generic;
using System;

public class Worker {

	public float X {
		get {
			return Mathf.Lerp (currentTile.X, currentlyMovingTo.X, movementPercentage);
		}
	}
	public float Y {
		get {
			return Mathf.Lerp (currentTile.Y, currentlyMovingTo.Y, movementPercentage);
		}
	}

	Tile currentTile;
	Tile destinationTile;
	float movementPercentage;

	List<Tile> travelPath;
	Tile currentlyMovingTo;

	float walk_speed;
	float work_speed;

	Job job;

	Action<Worker> cbStateChange;

	public Worker(Tile tile, float walk = 5f, float work = 5f)
	{
		currentTile = destinationTile = currentlyMovingTo = tile;
		this.walk_speed = walk;
		this.work_speed = work;
	}

	public void tick(float deltaTime)
	{
		if (isMoving()) {
			moveWorker (deltaTime);
		} else {
			// Not moving, can do work or grab a new job
			if (job == null) {
				lookForJob ();
			} else {
				workForSomeTime (deltaTime);
			}
		}
	}

	void moveWorker (float deltaTime)
	{
		float distToTravel = Mathf.Sqrt (Mathf.Pow (currentTile.X - currentlyMovingTo.X, 2) + Mathf.Pow (currentTile.Y - currentlyMovingTo.Y, 2));
		float distThisFrame = walk_speed * deltaTime;
		float percThisFrame = distThisFrame / distToTravel;
		movementPercentage += percThisFrame;
		if (movementPercentage >= 1) {
			currentTile = currentlyMovingTo;
			movementPercentage = 0;
			if (currentTile == destinationTile) {
				// reach destination
				if (cbStateChange != null)
					cbStateChange (this);
			} else {
				// Move to next tile in path
				int i = travelPath.Count - 1;
				currentlyMovingTo = travelPath [i];
				travelPath.RemoveAt (i);
			}
		}
	}

	void lookForJob ()
	{
		// doesnt have a job, look for one
		Job new_job = currentTile.world.getNearestJob(currentTile.X, currentTile.Y);
		if (new_job != null) {
			setJob (new_job);
		}
	}

	void workForSomeTime (float deltaTime)
	{
		if (currentTile == job.tile) {
			if (job.doWork (deltaTime * work_speed)) {
				setJob (null);
			}
		}
		else {
			setDestination (job.tile);
		}
	}

	public bool isMoving ()
	{
		return destinationTile != currentTile;
	}

	public bool isWorking ()
	{
		return job != null && !isMoving ();
	}

	public void setJob(Job job)
	{
		this.job = job;
		if (cbStateChange != null)
			cbStateChange (this);
	}

	public void setDestination(Tile tile)
	{
		destinationTile = tile;
		if (destinationTile != this.currentTile) {
			travelPath = findPathTo (destinationTile.X, destinationTile.Y);
			if (travelPath == null) {
				destinationTile = currentlyMovingTo = currentTile;
				// no path found, cant move
			} else {
				int i = travelPath.Count - 1;
				currentlyMovingTo = travelPath [i];
				travelPath.RemoveAt (i);
				if (cbStateChange != null)
					cbStateChange (this);
			}
		}
	}

	public void registerOnStateChangeCallback(Action<Worker> cb)
	{
		cbStateChange += cb;
	}

	public List<Tile> findPathTo(int x, int y)
	{
		return dijikstra_search(currentTile, currentTile.world.getTileAt(x, y));
	}

	protected List<Tile> dijikstra_search (Tile source, Tile target)
	{
		int predicted_length = 15;

		Dictionary<Tile, float> dist = new Dictionary<Tile, float> ();
		Dictionary<Tile, Tile> prev = new Dictionary<Tile, Tile> ();

		// unvisited
		List<Tile> unvisited = new List<Tile>(predicted_length);

		// Initialization
		foreach (Tile v in WorldController.Instance.world.tiles) {
			dist[v] = Mathf.Infinity; 	// Unknown distance from source to v
			prev[v] = null; 			// Previous node in optimal path from source
			unvisited.Add(v); 					// All nodes initially in unvisited nodes
		}

		dist[source] = 0;
		while (unvisited.Count > 0) {
			Tile u = null;

			// Source node will be selected first
			// put the smallest distance in u
			// u ← vertex in unvisted with min dist[u]
			foreach (Tile possibleU in unvisited) {
				if (u == null || dist [possibleU] < dist [u])
					u = possibleU;
			}
			if (u == target) {
				break; // we found the path
			}

			foreach (Tile v in u.getConnected()) { // where v is still in unvisited.
				if (u.isPassable() == false && u != source) continue;
				float alt = dist[u] + v.costToEnterFrom(u.X, u.Y);
				if (alt < dist [v]) { // A shorter path to v has been found
					dist[v] = alt;
					prev[v] = u;
				}
			}

			unvisited.Remove (u);
		}

		if (prev [target] == null) {
			Debug.Log ("No route found");
			return null; // No path to target could be found
		}
		List<Tile> path = new List<Tile> ();
		Tile curr = target;
		while (curr != null) {  // Construct the shortest path with a stack S
			path.Add (curr); 	// Push the vertex onto the stack
			curr = prev [curr];	// Traverse from target to source
		} // Push the source onto the stack

		return path;
	}

}
