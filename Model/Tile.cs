﻿using UnityEngine;
using System.Collections.Generic;
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

	Action<Tile> cbFurnitureInstalled;
	Action<Tile> cbTileChanged;

	public Job job { get; protected set; }
	public Furniture Furniture  {get; private set;}

	// inventory

	public Tile (World world, int x, int y)
	{
		this.world = world;
		this.X = x;
		this.Y = y;
		this.tileType = TYPE.EMPTY;
	}

	public bool isValidInstallation (Furniture.TYPE furniture_type)
	{
		if (hasJob () || isInstalled() || this.type == Tile.TYPE.EMPTY) {
			return false;
		}
		if (furniture_type == Furniture.TYPE.DOOR) {
			Tile tile_to_north = world.getTileAt (X, Y + 1);
			Tile tile_to_east = world.getTileAt (X + 1, Y);
			Tile tile_to_south = world.getTileAt (X, Y - 1);
			Tile tile_to_west = world.getTileAt (X - 1, Y);
			if (tile_to_north != null && tile_to_north.hasWall () && tile_to_south.hasWall ()) {
				// has walls north/south
			} else if (tile_to_east != null && tile_to_east.hasWall () && tile_to_west.hasWall ()) {
				// has walls east/west
			} else {
				return false;
			}
		}

		return true;
	}

	public bool hasWall()
	{
		return this.Furniture != null && this.Furniture.type == Furniture.TYPE.WALL;
	}

	public bool hasLinkedInstallation()
	{
		return this.Furniture != null && this.Furniture.linkedObject;
	}

	public void installFurniture (Furniture furn)
	{
		if (this.Furniture != null) {
			throw new Exception ("Cant install where already there is one");
		}
		this.Furniture = furn;

		if (this.Furniture.linkedObject) {
			UpdateLinkedFurnituredTiles ();
		}

		if (cbFurnitureInstalled != null)
			cbFurnitureInstalled (this);
	}

	// trigger onFurnitureChanged for the 4 neighbors if linked
	void UpdateLinkedFurnituredTiles ()
	{
		// Linked object, we need to upgrade neighbours
		Tile tile_to_north = world.getTileAt (X, Y + 1);
		if (tile_to_north != null && tile_to_north.hasLinkedInstallation ())
			tile_to_north.Furniture.neighbourChanged (this);
		Tile tile_to_east = world.getTileAt (X + 1, Y);
		if (tile_to_east != null && tile_to_east.hasLinkedInstallation ())
			tile_to_east.Furniture.neighbourChanged (this);
		Tile tile_to_south = world.getTileAt (X, Y - 1);
		if (tile_to_south != null && tile_to_south.hasLinkedInstallation ())
			tile_to_south.Furniture.neighbourChanged (this);
		Tile tile_to_west = world.getTileAt (X - 1, Y);
		if (tile_to_west != null && tile_to_west.hasLinkedInstallation ())
			tile_to_west.Furniture.neighbourChanged (this);
	}

	public void uninstallFurniture ()
	{
		this.Furniture.uninstall ();
		bool linked = this.Furniture.linkedObject;
		this.Furniture = null;
		if (linked) {
			UpdateLinkedFurnituredTiles ();
		}

	}

	public List<Tile> getConnected ()
	{
		List<Tile> connectedTiles = new List<Tile>();

		if (X > 0 && Y > 0)
			connectedTiles.Add(world.getTileAt(X-1, Y-1));
		if (Y > 0)
			connectedTiles.Add(world.getTileAt(X,   Y-1));
		if (X < world.Width-1 && Y > 0)
			connectedTiles.Add(world.getTileAt(X+1, Y-1));

		if (X > 0)
			connectedTiles.Add(world.getTileAt(X-1, Y));
		if (X < world.Width-1)
			connectedTiles.Add(world.getTileAt(X+1, Y));

		if (X > 0 && Y < world.Height-1)
			connectedTiles.Add(world.getTileAt(X-1, Y+1));
		if (Y < world.Height-1)
			connectedTiles.Add(world.getTileAt(X,   Y+1));
		if (X < world.Width-1 && Y < world.Height-1)
			connectedTiles.Add(world.getTileAt(X+1, Y+1));

		return connectedTiles;
	}

	public bool isPassable()
	{
		if (this.Furniture != null && this.Furniture.type == Furniture.TYPE.WALL)
			return false;
		return true;
	}

	public bool isWalkable ()
	{
		return this.type == TYPE.FLOOR && this.isPassable ();
	}

	public int heuristic_cost_estimate (Tile other)
	{
		return Mathf.Abs(this.X - other.X) + Mathf.Abs(this.Y - other.Y);
	}

	public float costToEnterFrom (int x, int y)
	{
		if (x != X && y != Y) // diagonally
			return 1.001f;
		else
			return 1f; // TODO: better movement cost
	}

	public void registerOnChangeCallback(Action<Tile> cb)
	{
		cbTileChanged += cb;
	}

	public void registerOnFurnitureInstalled (Action<Tile> cb)
	{
		cbFurnitureInstalled += cb;
	}

	public void UnregisterOnFurnitureInstalled (Action<Tile> cb)
	{
		cbFurnitureInstalled -= cb;
	}

	public override string ToString ()
	{
		return string.Format ("[Tile: type={0}, x={1}, y={2}]  [{3}]", type, X, Y, this.GetHashCode());
	}

	public bool isInstalled ()
	{
		return this.Furniture != null;
	}

	public bool hasJob ()
	{
		return job != null;
	}

	public void setJob (Job job)
	{
		this.job = job;
	}

	public bool isNeighbour(Tile tile)
	{
		return (tile.X == this.X && (tile.Y + 1 == this.Y || tile.Y - 1 == this.Y)) ||
		(tile.Y == this.Y && (tile.X + 1 == this.X || tile.X - 1 == this.X));
	}
}
