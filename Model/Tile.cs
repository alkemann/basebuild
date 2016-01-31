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
	public int X { get; protected set; }
	public int Y { get; protected set; }

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
		this.X = x;
		this.Y = y;
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
		return string.Format ("[Tile: type={0}, x={1}, y={2}]", type, X, Y);
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
		if (job.furniture != null)
			job.registerOnCompleteCallback (installFurniture);
	}

	void installFurniture(Job job)
	{
		Tile tile = job.tile;
		Furniture furn = job.furniture;
		installedFurniture = job.furniture;
		if (cbJobComplete != null)
			cbJobComplete (tile);

		// trigger onFurnitureChanged for the 4 neighbors if linked
		if (job.furniture.linkedObject) {
			// Linked object, we need to upgrade neighbours
			Tile tile_to_north = world.getTileAt (X, Y + 1);
			if (tile_to_north != null && tile_to_north.installedFurniture != null && tile_to_north.installedFurniture.type == furn.type) 
				tile_to_north.installedFurniture.neighbourChanged (tile);
			Tile tile_to_east = world.getTileAt (X + 1, Y);
			if (tile_to_east != null && tile_to_east.installedFurniture != null && tile_to_east.installedFurniture.type == furn.type) 
				tile_to_east.installedFurniture.neighbourChanged (tile);
			Tile tile_to_south = world.getTileAt (X, Y - 1);
			if (tile_to_south != null && tile_to_south.installedFurniture != null && tile_to_south.installedFurniture.type == furn.type) 
				tile_to_south.installedFurniture.neighbourChanged (tile);
			Tile tile_to_west = world.getTileAt (X - 1, Y);
			if (tile_to_west != null && tile_to_west.installedFurniture != null && tile_to_west.installedFurniture.type == furn.type) 
				tile_to_west.installedFurniture.neighbourChanged (tile);
		}

		tile.job = null; // remove the job
	}

	public bool isNeighbour(Tile tile)
	{
		return (tile.X == this.X && (tile.Y + 1 == this.Y || tile.Y - 1 == this.Y)) ||
		(tile.Y == this.Y && (tile.X + 1 == this.X || tile.X - 1 == this.X));
	}
}
