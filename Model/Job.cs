﻿using UnityEngine;
using System.Collections;
using System;

public class Job {

	public enum TYPE { NONE, MOVE, CONSTRUCT, INSTALL, TERMINAL_WORK }

	public TYPE type { get; protected set; }
	public Tile tile { get; protected set; }

	Action<Job> cbOnComplete;
	Action<Job> cbOnCancel;
	float work = 2f;

	public Job (Tile tile, float work = 2f, TYPE type = TYPE.INSTALL)
	{
		this.tile = tile;
		this.work = work;
		this.type = type;
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

	public void registerOnCancelCallback(Action<Job> cb)
	{
		cbOnCancel += cb;
	}

	public void unregisterOnCancelCallback(Action<Job> cb)
	{
		cbOnCancel -= cb;
	}
}
