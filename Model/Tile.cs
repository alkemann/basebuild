using UnityEngine;
using System.Collections;
using System;

public class Tile  {

	public enum TYPE { EMPTY, FLOOR };

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

	Action<Tile> cbTileChanged;

	public Tile (World world, int x, int y)
	{
		this.world = world;
		this.x = x;
		this.y = y;

		if (UnityEngine.Random.Range (0, 2) == 0)
			this.type = Tile.TYPE.EMPTY;
		else
			this.type = Tile.TYPE.FLOOR;
	}

	public void registerOnChangeCallback(Action<Tile> cb)
	{
		cbTileChanged += cb;
	}

	public void switchType ()
	{

		if (this.type == Tile.TYPE.FLOOR) 
			this.type = Tile.TYPE.EMPTY;
		else
			this.type = Tile.TYPE.FLOOR;

	}
	public override string ToString ()
	{
		return string.Format ("[Tile: type={0}, x={1}, y={2}]", type, x, y);
	}
	
}
