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

	Stack<Tile> travelPath;
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
		float distToTravel = currentlyMovingTo.costToEnterFrom (currentTile.X, currentTile.Y); // actually pay the movement cost of a tile
		// TODO pay half costToEnterFrom going in and other half going out.
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
				currentlyMovingTo = travelPath.Pop();
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
			job = new Job(currentTile, 2f, Job.TYPE.NONE); // TODO: Make an IDLE job type? would help with animation
		} else {
			setJob (new_job);
		}
	}

	void workForSomeTime (float deltaTime)
	{
		if (currentTile == job.tile) {
			bool work_done = job.doWork (deltaTime * work_speed);
			if (work_done) {
				workCompleted ();
			}
		} else {
			setDestination (job.tile);
		}
	}

	private void workCompleted ()
	{
		job.tile.setJob (null);
		setJob (null);
		// At this point, I want to make sure no references exist to
		// the Job, and have it garbage collected.
		if (currentTile.isWalkable () == false) {
			// The tile the worker completed work in
			// is nolonger safe, is the one they came from
			// safe?
			moveToSaferTile ();
		}
	}

	void moveToSaferTile ()
	{
		if (cameFrom.isPassable ()) {
			currentTile = destinationTile = currentlyMovingTo = cameFrom;
		}
		else {
			// TODO look for another safe til to move to
			// try to go in the same general diretion?
			List<Tile> neighbors = currentTile.getConnected ();
			foreach (Tile t in neighbors) {
				if (t.isWalkable () && t.hasJob () == false) {
					//setDestination (t); // TODO should i teleport out?
					currentTile = destinationTile = currentlyMovingTo = t;
					return;
					// hmm assuming nothing below needs to work
				}
			}
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
				currentlyMovingTo = travelPath.Pop();
				if (cbStateChange != null)
					cbStateChange (this);
			}
		}
	}

	public void registerOnStateChangeCallback(Action<Worker> cb)
	{
		cbStateChange += cb;
	}

	public Stack<Tile> findPathTo(int x, int y)
	{
		return currentTile.world.findPath(currentTile, currentTile.world.getTileAt(x, y));
	}
}
