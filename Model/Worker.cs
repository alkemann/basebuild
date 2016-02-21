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
			currentTile = currentlyMovingTo;
			movementPercentage = 0;
			if (currentTile == destinationTile) {
				// reach destination
				if (cbStateChange != null)
					cbStateChange (this);
			} else {
				// Move to next tile in path
				currentlyMovingTo = travelPath.Pop();


				// FIXME: how can we look for new path if the path
				// we originally planned is no longer valid?
				/*
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
				*/
			}
		}
	}

	void lookForJob ()
	{
		// doesnt have a job, look for one
		Job new_job = currentTile.world.getNearestJob(currentTile.X, currentTile.Y);
		if (new_job == null) {
			// if no job found, idle a bit before looking again for something to do
			job = new Job(null, 2f, Job.TYPE.NONE); // TODO: Make an IDLE job type? would help with animation
		} else {
			setJob (new_job);
		}
	}

	void workForSomeTime (float deltaTime)
	{
		if (job.tile == null || currentTile == job.tile) {
			bool work_done = job.doWork (deltaTime * work_speed);
			if (work_done) {
				workCompleted ();
			}
		} else {
			setDestination (job);
		}
	}

	private void workCompleted ()
	{
		if (job.tile != null)
			job.tile.setJob (null);
		setJob (null);
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

	public void setDestination(Job job)
	{
		setDestination (job.tile);
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
