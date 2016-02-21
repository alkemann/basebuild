using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectWorker : MonoBehaviour {

	public static Worker selected = null;
	Worker worker;
	GameObject contextMenu;

	void Start()
	{
		
	}

	public static void DisbandSelectedWorker()
	{
		Debug.Log ("Removing this worker");
	}

	public void OnMouseDown()
	{
		contextMenu = GameObject.Find ("Context Menu").transform.Find("Worker Context").gameObject;
		contextMenu.SetActive (true);
		Transform tt = contextMenu.transform.Find ("Body Text");
		GameObject gg = tt.gameObject;
		Text t = gg.GetComponent<Text> ();
		if (t == null) {
			Debug.LogError ("text not found");
			return;
		}
			
		t.text = "Random Worker\n\n" +
			"Movement : " + worker.GetMovementSpeed() + "\n" +
			"Work : " + worker.GetWorkSpeed() ;
		SelectWorker.selected = worker;
	}

	public void SetWorker(Worker w)
	{
		Debug.Log ("Setting worker");
		this.worker = w;
	}

	public void DeSelect()
	{
		GameObject.Find("Worker Context").gameObject.SetActive (false);
		SelectWorker.selected = null;
	}
}
