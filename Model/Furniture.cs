using UnityEngine;
using System.Collections.Generic;
using System;

public class Furniture {
	public enum TYPE { NONE, WALL, DOOR, TERMINAL, SPAWNER, MINER };
	public static float[] costs = {0, 2f, 10f, 50f, 100f, 20f};

	public TYPE type { get; protected set; }

	public Tile tile { get; protected set; }

	public bool linkedObject { get; protected set; }

	Action<Furniture> cbFurnitureChanged;
	Action<Furniture> cbFurnitureUninstalled;

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
			coolDown = UnityEngine.Random.Range(2f, 3f); // set first cooldown
			tile.world.registerOnTick(terminalTrigger);
		}
		if (type == TYPE.MINER) {
			// TODO how to cleanly place furniture code like this
			coolDown = UnityEngine.Random.Range(10f, 15f); // set first cooldown
			tile.world.registerOnTick(minerTrigger);
		}
	}

	public static float GetCost(Furniture.TYPE type)
	{
		return Furniture.costs [(int) type];
	}

	void terminalTrigger (float time)
	{
		coolDown -= time;
		if (coolDown <= 0) {
			// Terminal has triggered, lets create a job to force a worker to move here
			tile.world.createCustomJobAt(tile.X, tile.Y, 5f, Job.TYPE.TERMINAL_WORK);
			coolDown = UnityEngine.Random.Range(5f, 6f); // reset cooldown
		}
	}

	void minerTrigger (float time)
	{
		coolDown -= time;
		if (coolDown <= 0) {
			// Terminal has triggered, lets create a job to force a worker to move here
			tile.world.createCustomJobAt(tile.X, tile.Y, 5f, Job.TYPE.MINER_WORK);
			coolDown = UnityEngine.Random.Range(10f, 15f); // reset cooldown
		}
	}

	public void uninstall ()
	{
		if (cbFurnitureUninstalled != null)
			cbFurnitureUninstalled (this);
		cbFurnitureUninstalled = null;
		cbFurnitureChanged = null;
	}

	public float costToBuild()
	{
		return Furniture.costs [(int) this.type];
	}

	public void RegisterOnChangeCallback (Action<Furniture> cb)
	{
		cbFurnitureChanged += cb;
	}

	public void UnRegisterOnChangeCallback (Action<Furniture> cb)
	{
		cbFurnitureChanged -= cb;
	}

	public void RegisterOnUnInstallCallback (Action<Furniture> cb)
	{
		cbFurnitureUninstalled += cb;
	}

	public void UnRegisterOnUnInstallCallback (Action<Furniture> cb)
	{
		cbFurnitureUninstalled -= cb;
	}

	public void neighbourChanged(Tile neighbour)
	{
		cbFurnitureChanged (this);
	}
}
