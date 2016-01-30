using UnityEngine;
using System.Collections;

public class Worker {

	public float X {
		get {
			return Mathf.Lerp (currentTile.X, destinationTile.X, movementPercentage);
		}
	}
	public float Y {
		get {
			return Mathf.Lerp (currentTile.Y, destinationTile.Y, movementPercentage);
		}
	}

	Tile currentTile;
	Tile destinationTile;
	float movementPercentage;

	float walk_speed = 5f;
	float work_speed = 5f;

	Job job;

	public Worker(Tile tile)
	{
		currentTile = destinationTile = tile;
	}

	public void tick(float deltaTime)
	{
		if (destinationTile == currentTile) {
			// Not moving, can do work or grab a new job
			if (job == null) {
				// doesnt have a job, look for one
				this.job = currentTile.world.task();
			} else {
				if (currentTile == job.tile) {
					if (job.doWork (deltaTime * work_speed)) {
						job = null;
					}
				} else {
					destinationTile = job.tile;
				}
			}
		} else {
			float distToTravel = Mathf.Sqrt(Mathf.Pow(currentTile.X - destinationTile.X, 2) + Mathf.Pow(currentTile.Y - destinationTile.Y, 2));
			float distThisFrame = walk_speed * deltaTime;
			float percThisFrame = distThisFrame / distToTravel;
			movementPercentage += percThisFrame;
			if (movementPercentage >= 1) {
				// reach destination
				currentTile = destinationTile;
				movementPercentage = 0;
			}
		}
	}

	public bool isMoving ()
	{
		return destinationTile != currentTile;
	}

	public void setDestination(Tile tile)
	{
		destinationTile = tile;
	}
}
