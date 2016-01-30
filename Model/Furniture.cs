using UnityEngine;
using System.Collections;
using System;

public class Furniture {
	public enum TYPE { NONE, WALL };

	public TYPE type { get; protected set; }

	public Tile tile { get; protected set; }

	public bool linkedObject { get; protected set; }

	Action<Furniture> cbFurnitureChanged;


	public Furniture (Tile tile, TYPE type)
	{
		this.tile = tile;
		this.type = type;
		this.linkedObject = true;
	}

	public void registerOnChangeCallback (Action<Furniture> cb)
	{
		cbFurnitureChanged += cb;
	}

	public void neighbourChanged(Tile neighbour)
	{
		cbFurnitureChanged (this);
	}
}
