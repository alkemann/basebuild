using UnityEngine;
using System.Collections.Generic;
using System;
using Path;

public class World {

	public int Width { get; protected set; }
	public int Height { get; protected set; }

	Tile[,] tiles;

	public JobQueue jobs;
	public bool pause;

	Finder Pathfinder;

	public Job getNearestJob (int x, int y)
	{
		return jobs.getNearestJob (x, y);
	}

	public Job getFirstJob ()
	{
		return jobs.getFirstJob ();
	}

	public void putJobBack (Job job)
	{
		this.jobs.Add (job);
	}

	Action<Worker> cbWorkerCreated;
	Action<Tile> cbTileChanged;
	Action<Job> cbJobCreated;
	Action<float> cbTick;

	public World (int width = 100, int height = 100)
	{
		this.Width = width;
		this.Height = height;

		Pathfinder = new Finder(this);

		tiles = new Tile[width, height];
		createTiles (width, height);
		jobs = new JobQueue ();
	}

	public Stack<Tile> findPath (Tile from, Tile to)
	{
		return Pathfinder.findPath (from, to);
	}

	public void createWorkerAt(int x, int y)
	{
		Tile t = tiles [x, y];
		if (t.type == Tile.TYPE.EMPTY)
			return; // Cant create workers on empty
		Worker w = new Worker (t, UnityEngine.Random.Range(4.5f, 7.5f), UnityEngine.Random.Range(0.5f, 5f));

		registerOnTick (w.tick); // make sure workers can react to tick

		if (cbWorkerCreated != null)
			cbWorkerCreated (w);
	}

	public void tick (float deltaTime)
	{
		if (pause)
			return;
		if (cbTick != null) {
			cbTick (deltaTime);
		}
	}

	public bool isCoordinatesWithinBuildWorld (int x, int y)
	{
		return (x >= 0 && x < Width && y >= 0 && y < Height);
	}

	void createTiles (int width, int height)
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles [x, y] = new Tile (this, x, y);
			}
		}
	}

	public Tile getTileAt(int x, int y)
	{
		if (x < 0 || x >= Width) {
			return null;
			// throw new Exception ("x is out of map scope");
		} else if (y < 0 || y >= Height) {
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
		if (cbTileChanged != null)
			cbTileChanged (tile);
	}

	public Job createInstallJobAt (Furniture.TYPE type, int x, int y)
	{
		Tile tile = tiles [x, y];
		if (tile.isValidInstallation(type) == false || tile.isWalkable() == false)
			return null;

		Job job = new Job(tile, Furniture.costs[(int) type], Job.TYPE.INSTALL);
		jobs.Add (job);
		tile.setJob (job);
		job.registerOnCompleteCallback ((j) => {
			Tile job_tile = j.tile;
			job_tile.installFurniture(new Furniture (job_tile, type));
			job_tile.setJob(null); // remove the job from tile
			if (cbTileChanged != null)
				cbTileChanged(job_tile);
		});
		if (cbJobCreated != null)
			cbJobCreated (job);
		return job;
	}

	public Job createMoveJobAt (int x, int y)
	{
		Tile tile = tiles [x, y];
		if (tile.hasJob () || tile.isWalkable () == false)
			return null;
		Job job = new Job (tile, 0.01f, Job.TYPE.MOVE);
		jobs.Add (job);
		tile.setJob (job);
		if (cbJobCreated != null)
			cbJobCreated (job);

		// TODO was this needed?
//		job.registerOnCompleteCallback ((j) => {
//			tile.setJob(null); // remove the job from tile
//		});
		return job;
	}

	public Job createCustomJobAt(int x, int y, float workload, Job.TYPE type) {

		Tile tile = tiles [x, y];
		if (tile.hasJob () || tile.isWalkable() == false)
			return null;
		Job job = new Job (tile, workload, type);
		jobs.Add (job);
		tile.setJob (job);
		if (cbJobCreated != null)
			cbJobCreated (job);
		return job;
	}

	public void registOnJobCreated(Action<Job> cb)
	{
		cbJobCreated += cb;
	}

	public void unregistOnJobCreated (Action<Job> cb)
	{
		cbJobCreated -= cb;
	}

	public void registerOnWorkerCreated(Action<Worker> cb)
	{
		cbWorkerCreated += cb;
	}

	public void unregistOnWorkerCreated (Action<Worker> cb)
	{
		cbWorkerCreated -= cb;
	}

	public void registerOnTileChanged (Action<Tile> cb)
	{
		cbTileChanged += cb;
	}

	public void unregistOnTileCreated (Action<Tile> cb)
	{
		cbTileChanged -= cb;
	}

	public void registerOnTick (Action<float> cb)
	{
		cbTick += cb;
	}

	public void unregistOnTick (Action<float> cb)
	{
		cbTick -= cb;
	}
}
