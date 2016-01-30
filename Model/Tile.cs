using UnityEngine;
using System.Collections;
using System;

public class Tile  {

	public enum TYPE { NONE, EMPTY, FLOOR };

	TYPE tileType;
	public TYPE type {
		get {
			return this.tileType;
		}
		set {
			TYPE old = tileType;
			tileType = value;
			if (old != tileType && cbTileChanged != null) {
				// type changed, call callback
				cbTileChanged(this);
			}
		}
	}

	public World world { get; protected set; }
	public int x { get; protected set; }
	public int y { get; protected set; }

	Action<Tile> cbJobComplete;
	Action<Tile> cbTileChanged;

	public Job job { get; protected set; }
	public Furniture installedFurniture {
		get;
		protected set;
	}

	// inventory

	public Tile (World world, int x, int y)
	{
		this.world = world;
		this.x = x;
		this.y = y;
		this.tileType = TYPE.EMPTY;
	}

	public void registerOnChangeCallback(Action<Tile> cb)
	{
		cbTileChanged += cb;
	}

	public void registerOnJobCompleteCallback(Action<Tile> cb)
	{
		cbJobComplete += cb;
	}

	public override string ToString ()
	{
		return string.Format ("[Tile: type={0}, x={1}, y={2}]", type, x, y);
	}

	public bool isInstalled ()
	{
		return installedFurniture != null;
	}

	public bool hasJob ()
	{
		return job != null;
	}

	public void setJob (Job job)
	{
		this.job = job;
		job.registerOnCompleteCallback (installFurniture);
	}

	void installFurniture(Job job)
	{
		Tile t = job.tile;
		installedFurniture = job.furniture;
		if (cbJobComplete != null)
			cbJobComplete (t);
		t.job = null;
	}
}
