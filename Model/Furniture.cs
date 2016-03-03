using UnityEngine;
using System.Collections.Generic;
using System;

public class Furniture
{
	public enum TYPE { NONE, WALL, DOOR, TERMINAL, SPAWNER, MINER, HOPPER, DEPOT, PROCESSOR }

	public TYPE type { get; protected set; }
	public Tile tile { get; protected set; }
	public bool linkedObject { get; protected set; }

	Action<Furniture> cbFurnitureChanged;
	Action<Furniture> cbFurnitureUninstalled;


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
		}
		if (type == TYPE.MINER) {
			// TODO call register tick if u need
		}
	}

	public static float GetCost(Furniture.TYPE type)
	{
		return Behaviours.GetCostByType(type);
	}

	public float costToBuild ()
	{
		return Behaviours.GetCostByType(this.type);
	}

	public void uninstall ()
	{
		if (cbFurnitureUninstalled != null) {
			cbFurnitureUninstalled(this);
		}
		cbFurnitureUninstalled = null;
		cbFurnitureChanged = null;
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
