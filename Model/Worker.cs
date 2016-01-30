﻿using UnityEngine;
using System.Collections;
using System;

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

	Action<Worker> cbStateChange;

	public Worker(Tile tile)
	{
		currentTile = destinationTile = tile;
	}

	public void tick(float deltaTime)
	{
		if (isMoving()) {
			float distToTravel = Mathf.Sqrt(Mathf.Pow(currentTile.X - destinationTile.X, 2) + Mathf.Pow(currentTile.Y - destinationTile.Y, 2));
			float distThisFrame = walk_speed * deltaTime;
			float percThisFrame = distThisFrame / distToTravel;
			movementPercentage += percThisFrame;
			if (movementPercentage >= 1) {
				// reach destination
				currentTile = destinationTile;
				movementPercentage = 0;
				if (cbStateChange != null)
					cbStateChange (this);
			}
		} else {
			// Not moving, can do work or grab a new job
			if (job == null) {
				// doesnt have a job, look for one
				Job new_job = currentTile.world.task();
				if (new_job != null) {
					setJob (new_job);
				}
			} else {
				if (currentTile == job.tile) {
					if (job.doWork (deltaTime * work_speed)) {
						setJob (null);
					}
				} else {
					setDestination(job.tile);
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
		if (isMoving() && cbStateChange != null)
			cbStateChange (this);
	}

	public void registerOnStateChangeCallback(Action<Worker> cb)
	{
		cbStateChange += cb;
	}
}