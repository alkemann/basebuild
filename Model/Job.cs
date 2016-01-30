using UnityEngine;
using System.Collections;
using System;

public class Job {

	float work = 2f;

	public Tile tile { get; protected set; }
	public Furniture furniture { get; protected set; }

	Action<Job> cbOnComplete;

	public Job (Tile tile, Furniture furn)
	{
		this.tile = tile;
		this.furniture = furn;
		tile.setJob (this);
	}

	public bool doWork(float work)
	{
		if (this.work <= 0) {
			Debug.LogError ("Work already compelted!");
			return true;
		}
		this.work -= work;
		if (this.work <= 0 && cbOnComplete != null) {
			cbOnComplete (this);
		}
		return this.work <= 0;
	}

	public void registerOnCompleteCallback(Action<Job> cb)
	{
		cbOnComplete += cb;
	}

	public void unregisterOnCompleteCallback(Action<Job> cb)
	{
		cbOnComplete -= cb;
	}
}
