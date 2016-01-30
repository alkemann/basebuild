using UnityEngine;
using System.Collections.Generic;
using System;

public class World {

	Tile[,] tiles;
	public const int WIDTH = 100;
	public const int HEIGHT = 100;

	List<Job> jobs;

	public World()
	{
		tiles = new Tile[WIDTH, HEIGHT];
		createTiles ();
		jobs = new List<Job> ();

	public static bool isCoordinatesWithinBuildWorld (int x, int y)
	{
		return (x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT);
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
		if (x < 0 || x >= WIDTH) {
			return null;
			// throw new Exception ("x is out of map scope");
		} else if (y < 0 || y >= HEIGHT) {
			return null;
			// throw new Exception ("y is out of map scope");
		}
		try {
			return tiles[x,y];
		} catch (IndexOutOfRangeException e) {
			Debug.LogError( string.Format( " Trying to access {0},{1}", x, y ));
			Debug.Log (e.StackTrace);
			return null;
		}
	}

	public void buildTileAt (Tile.TYPE buildMode, int x, int y)
	{
		Tile tile = tiles [x, y];
		tile.type = buildMode;
	}

	public Job createInstallJobAt (Furniture.TYPE type, int x, int y)
	{
		Tile tile = tiles [x, y];
		if (tile.hasJob () || tile.isInstalled() || tile.type == Tile.TYPE.EMPTY)
			return null;

		Job job = new Job(tile, new Furniture (tile, type));

//		jobs.Add (job);
		return job;
	}
}
