using UnityEngine;
using System.Collections.Generic;
using System;

public class Furniture {
	public enum TYPE { NONE, WALL, DOOR, TERMINAL, SPAWNER };
	public static float[] costs = {0, 2f, 10f, 50f, 100f};

	public TYPE type { get; protected set; }

	public Tile tile { get; protected set; }

	public bool linkedObject { get; protected set; }

	Action<Furniture> cbFurnitureChanged;


	public Furniture (Tile tile, TYPE type)
	{
		this.tile = tile;
		this.type = type;
		if (type == TYPE.WALL || type == TYPE.DOOR) {
			this.linkedObject = true;
		} else {
			this.linkedObject = false;
		}
	}

	public float costToBuild()
	{
		return Furniture.costs [(int) this.type];
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
