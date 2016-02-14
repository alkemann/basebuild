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

	float coolDown = 0f;


	public Furniture (Tile tile, TYPE type)
	{
		this.tile = tile;
		this.type = type;
		if (type == TYPE.WALL || type == TYPE.DOOR) {
			this.linkedObject = true;
		} else {
			this.linkedObject = false;
		}
		if (type == TYPE.TERMINAL) {
			// TODO how to cleanly place furniture code like this
			coolDown = UnityEngine.Random.Range (2f, 3f); // set first cooldown
			// FIXME: there is no tick
		}
	}

	void terminalTrigger(float time)
	{
		coolDown -= time;
		if (coolDown <= 0) {
			// Terminal has triggered, lets create a job to force a worker to move here
			tile.world.createCustomJobAt(tile.X, tile.Y, 5f, Job.TYPE.TERMINAL_WORK);
			coolDown = UnityEngine.Random.Range (5f, 6f); // reset cooldown
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
