using UnityEngine;
using System.Collections.Generic;

public class JobQueue {

	List<Job> jobQueue;

	public JobQueue ()
	{
		jobQueue = new List<Job> (20);
	}

	public int Count ()
	{
		return jobQueue.Count;
	}

	public bool Contains(Job job)
	{
		return jobQueue.Contains (job);
	}

	public bool Add(Job job)
	{
		jobQueue.Add(job);
		return true; // TODO maybe return false if queue is full
	}

	public void remove (Job job)
	{
		jobQueue.Remove (job);
	}

	/**
	 * By default prioritizes the job nearest to x,y that can be reached by a worker
	 * from x,y.
	 * 
	 * Added features could be:
	 *  - priority (high, medium, low or infite levels?
	 *  - categories (some jobs get natural higher priority?)
	 *  - profession ( only do certain jobs or priotize them differently )
	 */
	public Job getNearestJob(int x, int y)
	{
		if (jobQueue.Count == 0)
			return null;
		Job nearest = null;
		float distance = float.MaxValue;
		foreach (Job job in jobQueue) {
			if (job.tile.X == x && job.tile.Y == y) {
				nearest = job;
				break; // always do the job you are at first!
			}
			float how_far_is_this_job = distanceToTileFrom (job.tile, x, y);
			if (how_far_is_this_job != 0 && how_far_is_this_job < distance) {
				nearest = job;
				distance = how_far_is_this_job;
			}
		}
		if (nearest == null) {
			// None of the jobs in the queue was reachable from x,y	
			return null;
		}
		jobQueue.Remove (nearest);
		return nearest;
	}

	public Job getFirstJob()
	{
		if (jobQueue.Count == 0)
			return null;
		Job j = jobQueue [0];
		jobQueue.RemoveAt(0);
		return j;
	}

	float distanceToTileFrom (Tile tile, int x, int y)
	{
		Stack<Tile> path = tile.world.findPath (tile, tile.world.getTileAt (x, y));
		if (path == null) {
			return 0;
		}
		return path.Count;
	}
	public IEnumerator<Job> GetEnumerator()
	{
		return jobQueue.GetEnumerator();
	}
}
