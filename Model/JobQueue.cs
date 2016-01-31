using UnityEngine;
using System.Collections.Generic;

public class JobQueue {

	List<Job> jobQueue;

	public JobQueue ()
	{
		jobQueue = new List<Job> (20);
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
			float how_far_is_this_job = distanceToTileFrom (job.tile, x, y);
			if (how_far_is_this_job < distance) {
				nearest = job;
				distance = how_far_is_this_job;
			}
		}
		if (nearest == null) {
			Debug.LogError ("Nearest still null!");
			return null;
		}
		jobQueue.Remove (nearest);
		return nearest;
	}

	float distanceToTileFrom (Tile tile, int x, int y)
	{
		// TODO: Add pathing logic
		return Mathf.Sqrt (Mathf.Pow (tile.X - x, 2) + Mathf.Pow (tile.Y - y, 2));
	}
}
