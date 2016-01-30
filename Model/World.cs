﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class World {

	Tile[,] tiles;
	public const int WIDTH = 100;
	public const int HEIGHT = 100;

	JobQueue tasks;

	public Job task ()
	{
		return tasks.Dequeue ();
	}

	List<Worker> workers;

	public World()
	{
		tiles = new Tile[WIDTH, HEIGHT];
		createTiles ();
		tasks = new JobQueue ();
		workers = new List<Worker> ();
		Worker w = new Worker (tiles [WIDTH / 2, HEIGHT / 2]);
		workers.Add (w);
	}

	public void tick (float deltaTime)
	{
		foreach(Worker w in getWorkers()) {
			w.tick (Time.deltaTime);
		}
	}

	public List<Worker> getWorkers ()
	{
		return workers;
	}

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
		tasks.Enqueue (job);
		return job;
	}
}
