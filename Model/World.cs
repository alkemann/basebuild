using UnityEngine;
using System.Collections.Generic;
using System;
using Path;

public class World
{

	public int Width { get; protected set; }
	public int Height { get; protected set; }

	public bool pause;

	// FIXME: how can we have this be not public?
	public JobQueue jobs;
	public List<Worker> workers;
	public Tile[,] tiles;

	Finder Pathfinder;

	Action<Worker> cbWorkerCreated;
	Action<Tile> cbTileChanged;
	Action<Job> cbJobCreated;
	Action<Astroid> cbAstroidCreated;
	Action<float> cbTick;

	public World (int width = 100, int height = 100)
	{
		this.Width = width;
		this.Height = height;
		Pathfinder = new Finder(this);
		workers = new List<Worker>();
		jobs = new JobQueue();
		tiles = new Tile[Width, Height];
		createTiles(Width, Height);
	}

	public void SetupBlankWorld ()
	{
		int x = Height / 2;
		int y = Width / 2;
		Tile center_tile = buildTileAt(Tile.TYPE.FLOOR, x, y);
		center_tile.installFurniture(new Furniture(center_tile, Furniture.TYPE.SPAWNER));
		if (cbTileChanged != null)
			cbTileChanged(center_tile);
		buildTileAt(Tile.TYPE.FLOOR, x + 1, y);
		buildTileAt(Tile.TYPE.FLOOR, x - 1, y);
		buildTileAt(Tile.TYPE.FLOOR, x + 1, y + 1);
		buildTileAt(Tile.TYPE.FLOOR, x, y + 1);
		buildTileAt(Tile.TYPE.FLOOR, x - 1, y + 1);
		buildTileAt(Tile.TYPE.FLOOR, x + 1, y - 1);
		buildTileAt(Tile.TYPE.FLOOR, x, y - 1);
		buildTileAt(Tile.TYPE.FLOOR, x - 1, y - 1);
		createAstroidAt(55, 55);
	}

	public void SetupWorldFromData (SaveGame data)
	{
		foreach (TileData tile_data in data.tiles) {
			Tile tile = tiles[tile_data.x, tile_data.y];
			tile.ApplyData(tile_data);
			if (cbTileChanged != null)
				cbTileChanged(tile);
			if (tile.astroid != null && cbAstroidCreated != null)
				cbAstroidCreated(tile.astroid);
			if (tile.furniture != null && tile.furniture.type == Furniture.TYPE.MINER)
				tile.TriggerFurnitureInstalled();
		}

		foreach (WorkerData worker_data in data.workers) {
			Tile tile = tiles[worker_data.x, worker_data.y];
			Worker worker = new Worker(tile, worker_data.walk_speed, worker_data.work_speed);
			workers.Add(worker);
			if (cbWorkerCreated != null)
				cbWorkerCreated(worker);
			RegisterOnTick(worker.tick); // make sure workers can react to tick
		}

		foreach (JobData job_data in data.jobs) {
			switch ((Job.TYPE)job_data.type) {
				case Job.TYPE.INSTALL:
					int t;
					if (Int32.TryParse((string) job_data.GetMeta("ftype"), out t))
						createInstallJobAt((Furniture.TYPE) t, job_data.x, job_data.y);
					break;
				case Job.TYPE.UNINSTALL:
					createUninstallJobAt(job_data.x, job_data.y);
					break;
				case Job.TYPE.MOVE:
					createMoveJobAt(job_data.x, job_data.y);
					break;
				case Job.TYPE.MINER_WORK:
				case Job.TYPE.TERMINAL_WORK:
					createCustomJobAt(job_data.x, job_data.y, (float) job_data.work, (Job.TYPE) job_data.type);
					break;
				//case Job.TYPE.CONSTRUCT:
				default:
					Debug.LogError("Unspecified job type");
					break;
			}
		}
	}

	public Stack<Tile> findPath (Tile from, Tile to)
	{
		return Pathfinder.findPath(from, to);
	}

	public void createWorkerAt (int x, int y)
	{
		Tile t = tiles[x, y];
		if (t.type == Tile.TYPE.EMPTY)
			return; // Cant create workers on empty
		Worker w = new Worker(t, UnityEngine.Random.Range(4.5f, 7.5f), UnityEngine.Random.Range(0.5f, 5f));
		workers.Add(w);
		RegisterOnTick(w.tick); // make sure workers can react to tick

		if (cbWorkerCreated != null)
			cbWorkerCreated(w);
	}

	public Astroid createAstroidAt (int x, int y)
	{
		Tile tile = getTileAt(x, y);
		Astroid astroid = new Astroid(tile);
		tile.astroid = astroid;

		if (cbAstroidCreated != null)
			cbAstroidCreated(astroid);
		return astroid;
	}

	public void tick (float deltaTime)
	{
		if (pause)
			return;
		if (cbTick != null) {
			cbTick(deltaTime);
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
				tiles[x, y] = new Tile(this, x, y);
			}
		}
	}

	public Tile getTileAt (int x, int y)
	{
		if (x < 0 || x >= Width) {
			return null;
			// throw new Exception ("x is out of map scope");
		} else if (y < 0 || y >= Height) {
			return null;
			// throw new Exception ("y is out of map scope");
		}
		try {
			return tiles[x, y];
		} catch (IndexOutOfRangeException e) {
			Debug.LogError(string.Format(" Trying to access {0},{1}", x, y));
			Debug.Log(e.StackTrace);
			return null;
		}
	}

	public Tile buildTileAt (Tile.TYPE buildMode, int x, int y)
	{
		Tile tile = tiles[x, y];
		tile.type = buildMode;
		if (cbTileChanged != null)
			cbTileChanged(tile);
		return tile;
	}

	public Job createInstallJobAt (Furniture.TYPE type, int x, int y)
	{
		if (this.jobs.Count() >= (this.workers.Count * 10) + 1) {
			return null;
		}
		Tile tile = tiles[x, y];
		if (tile.isValidInstallation(type) == false || tile.isWalkable() == false)
			return null;

		Dictionary<string, object> meta = new Dictionary<string, object>();
		meta.Add("ftype", (int) type);
		meta.Add("fcost", Furniture.costs[(int)type]);
		Job job = new Job(tile, Furniture.costs[(int)type], Job.TYPE.INSTALL, meta);
		jobs.Add(job);
		job.registerOnCompleteCallback((j) => {
			Tile job_tile = j.tile;
			job_tile.installFurniture(new Furniture(job_tile, type));
			if (cbTileChanged != null)
				cbTileChanged(job_tile);
		});
		if (cbJobCreated != null)
			cbJobCreated(job);
		return job;
	}

	public Job createMoveJobAt (int x, int y)
	{
		if (this.jobs.Count() >= this.workers.Count * 11) {
			return null;
		}
		Tile tile = tiles[x, y];
		if (tile.hasJob() || tile.isWalkable() == false)
			return null;
		Job job = new Job(tile, 0.01f, Job.TYPE.MOVE);
		jobs.Add(job);
		if (cbJobCreated != null)
			cbJobCreated(job);
		return job;
	}

	public Job createCustomJobAt (int x, int y, float workload, Job.TYPE type)
	{
		Tile tile = tiles[x, y];
		if (tile.hasJob() || tile.isWalkable() == false)
			return null;
		Job job = new Job(tile, workload, type);
		jobs.Add(job);
		if (cbJobCreated != null)
			cbJobCreated(job);
		return job;
	}

	public Job createUninstallJobAt (int x, int y)
	{
		Tile tile_with_furn = tiles[x, y];
		if (tile_with_furn.hasJob() || tile_with_furn.isInstalled() == false)
			return null;

		Tile tile_for_job = tile_with_furn;
		if (tile_for_job.isPassable() == false) {
			// We can not move TO the tile to accomplish job,
			// we must go to adjacent
			List<Tile> neighs = tile_for_job.getConnected();
			foreach (Tile neigh in neighs) {
				if (neigh.isWalkable()) {
					tile_for_job = neigh;
					break;
				}
			}
		}

		Job job = new Job(tile_for_job, Furniture.GetCost(tile_with_furn.furniture.type) * 0.5f, Job.TYPE.UNINSTALL); // TODO better placement of uninstall cost
		jobs.Add(job);

		job.registerOnCompleteCallback((j) => {
			// Specifically do not use job tile since furniture
			// may be installed on adjacent tile
			tile_with_furn.uninstallFurniture();
			if (cbTileChanged != null)
				cbTileChanged(tile_with_furn);
		});

		if (cbJobCreated != null)
			cbJobCreated(job);
		return job;
	}

	public void cancelJobAt (int x, int y)
	{
		Tile tile = tiles[x, y];
		if (tile.hasJob() == false)
			return;
		Job job = tile.job;
		tile.setJob(null);
		if (jobs.Contains(job) == false)
			return;
		jobs.remove(job);
		job.cancel();
	}

	public Job getNearestJob (int x, int y)
	{
		return jobs.getNearestJob(x, y);
	}

	public Job getFirstJob ()
	{
		return jobs.getFirstJob();
	}

	public void putJobBack (Job job)
	{
		this.jobs.Add(job);
	}

	public void RegisterOnAstroidCreated (Action<Astroid> cb)
	{
		cbAstroidCreated += cb;
	}

	public void registOnJobCreated (Action<Job> cb)
	{
		cbJobCreated += cb;
	}

	public void unregistOnJobCreated (Action<Job> cb)
	{
		cbJobCreated -= cb;
	}

	public void registerOnWorkerCreated (Action<Worker> cb)
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

	public void RegisterOnTick (Action<float> cb)
	{
		cbTick += cb;
	}

	public void UnRegisterOnTick (Action<float> cb)
	{
		cbTick -= cb;
	}
}
