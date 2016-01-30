using UnityEngine;
using System.Collections.Generic;

public class JobQueue {

	Queue<Job> jobQueue;

	public JobQueue ()
	{
		jobQueue = new Queue<Job> (20);
	}

	public bool Enqueue(Job job)
	{
		jobQueue.Enqueue (job);
		return true; // TODO maybe return false if queue is full
	}

	public Job Dequeue()
	{
		if (jobQueue.Count == 0)
			return null;
		return jobQueue.Dequeue ();
	}

}
