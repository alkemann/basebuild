using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JobCounter : MonoBehaviour {

	void FixedUpdate () {
		GetComponent<Text>().text = "" + WorldController.Instance.world.jobs.Count();
	}
}
