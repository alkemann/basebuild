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

	Tile cameFrom;
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
			cameFrom = currentTile;
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
				if (currentlyMovingTo.isWalkable() == false) {
					// changes since path was built means the
					// path must be abandoned, if new one cant be build
					// job and destination must be abandoned
					travelPath = findPathTo (destinationTile.X, destinationTile.Y);
					if (travelPath == null) {
						destinationTile = currentlyMovingTo = currentTile;
						if (job != null) {
							currentTile.world.putJobBack (job);
							setJob (null);
						}
					}
				}
			}
		}
	}

	void lookForJob ()
	{
		// doesnt have a job, look for one
		Job new_job = currentTile.world.getNearestJob(currentTile.X, currentTile.Y);
		if (new_job == null) {
			// if no job found, idle a bit before looking again for something to do
			job = new Job(currentTile, 10, Job.TYPE.NONE); // TODO: Make an IDLE job type? would help with animation
		} else {
			setJob (new_job);
		}
	}

	void workForSomeTime (float deltaTime)
	{
		if (currentTile == job.tile) {
			if (job.doWork (deltaTime * work_speed)) {
				Job j = job;
				setJob (null);
				if (j.tile.isPassable () == false) {
					// The tile the worker completed work in
					// is nolonger safe, is the one they came from
					// safe?
					if (cameFrom.isPassable ())
						setDestination (cameFrom);
					else {
						// TODO look for another safe til to move to
						// try to go in the same general diretion?
					}
				}
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
		return currentTile.world.findPath(currentTile, currentTile.world.getTileAt(x, y));
	}
}
