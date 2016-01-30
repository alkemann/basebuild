using UnityEngine;
using System.Collections;
using System;

public class World {

	Tile[,] tiles;
	public const int WIDTH = 100;
	public const int HEIGHT = 100;

	public World()
	{
		tiles = new Tile[WIDTH, HEIGHT];
		createTiles ();
	}

	void createTiles ()
	{
		for (int x = 0; x < WIDTH; x++) {
			for (int y = 0; y < HEIGHT; y++) {
				tiles [x, y] = new Tile (this, x, y);
			}
		}
	}

	public Tile getTileAt(int x, int y)
	{
		if (x<0 || x > WIDTH) {
			return null;
			// throw new Exception ("x is out of map scope");
		} else if (y<0 || y>HEIGHT) {
			return null;
			// throw new Exception ("y is out of map scope");
		}
		return tiles[x,y];
	}
}
