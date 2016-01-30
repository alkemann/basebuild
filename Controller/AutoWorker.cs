using UnityEngine;
using System.Collections;

public class AutoWorker : MonoBehaviour
{

	public Job job;

	void Update ()
	{
		job.doWork (Time.deltaTime);
	}
}

