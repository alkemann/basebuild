﻿using UnityEngine;
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

	Action<Tile> cbFurnitureInstalled;
	Action<Tile> cbTileChanged;

	public Job job { get; protected set; }
	public Furniture Furniture  {get; private set;}

	public void installFurniture (Furniture furn)
	{
		if (this.Furniture != null) {
			throw new Exception ("Cant install where already there is one");
		}
		this.Furniture = furn;
		// trigger onFurnitureChanged for the 4 neighbors if linked
		if (furn.linkedObject) {
			// Linked object, we need to upgrade neighbours
			Tile tile_to_north = world.getTileAt (X, Y + 1);
			if (tile_to_north != null && tile_to_north.Furniture != null && tile_to_north.Furniture.linkedObject)
				tile_to_north.Furniture.neighbourChanged (this);
			Tile tile_to_east = world.getTileAt (X + 1, Y);
			if (tile_to_east != null && tile_to_east.Furniture != null && tile_to_east.Furniture.linkedObject)
				tile_to_east.Furniture.neighbourChanged (this);
			Tile tile_to_south = world.getTileAt (X, Y - 1);
			if (tile_to_south != null && tile_to_south.Furniture != null && tile_to_south.Furniture.linkedObject)
				tile_to_south.Furniture.neighbourChanged (this);
			Tile tile_to_west = world.getTileAt (X - 1, Y);
			if (tile_to_west != null && tile_to_west.Furniture != null && tile_to_west.Furniture.linkedObject)
				tile_to_west.Furniture.neighbourChanged (this);
		}

		if (cbFurnitureInstalled != null)
			cbFurnitureInstalled (this);
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

	public void registerOnFurnitureInstalled (Action<Tile> cb)
	{
		cbFurnitureInstalled += cb;
	}

	public override string ToString ()
	{
		return string.Format ("[Tile: type={0}, x={1}, y={2}]", type, X, Y);
	}

	public bool isInstalled ()
	{
		return Furniture != null;
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
