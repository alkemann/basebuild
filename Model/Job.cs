using UnityEngine;
using System.Collections.Generic;
using System;

public class Job {

	public enum TYPE { NONE, MOVE, CONSTRUCT, INSTALL, UNINSTALL, TERMINAL_WORK, MINER_WORK
	}

	public TYPE type { get; protected set; }
	public Tile tile { get; protected set; }
	public Dictionary<string, object> meta;

	Action<Job> cbOnComplete;
	Action<Job> cbOnCancel;
	float work = 2f;

	public Job (Tile tile, float work = 2f, TYPE type = TYPE.INSTALL, Dictionary<string, object> meta = null)
	{
		if (tile != null) {
			this.tile = tile;
			tile.setJob (this);
		}
		this.work = work;
		this.type = type;
		this.meta = meta;
	}

	public bool ShouldWorkerNextToTile()
	{
		return this.type == TYPE.CONSTRUCT || this.type == TYPE.INSTALL || this.type == TYPE.MINER_WORK; //  || this.type == TYPE.UNINSTALL
	}

	public void cancel()
	{
		if (cbOnCancel != null) {
			cbOnCancel (this);
		}
	}

	public float WorkLeft()
	{
		return work;
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
