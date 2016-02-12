using UnityEngine;
using System.Collections.Generic;

public class JobQueue {

	List<Job> jobQueue;

	public JobQueue ()
	{
		jobQueue = new List<Job> (20);
	}

	public int count ()
	{
		return jobQueue.Count;
	}

	public bool Add(Job job)
	{
		jobQueue.Add(job);
		return true; // TODO maybe return false if queue is full
	}

	public Job getNearestJob(int x, int y)
	{
		if (jobQueue.Count == 0)
			return null;
		Job nearest = null;
		float distance = float.MaxValue;
		foreach (Job job in jobQueue) {
			try {
				float how_far_is_this_job = distanceToTileFrom (job.tile, x, y);
				if (how_far_is_this_job < distance) {
					nearest = job;
					distance = how_far_is_this_job;
				}
			} catch (UnreachableLocationException e) {
				// Cant do  that job
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
		List<Tile> path = tile.world.findPath (tile, tile.world.getTileAt (x, y));
		if (path == null) {
			throw new UnreachableLocationException ();
		}
		return path.Count;
	}
}
