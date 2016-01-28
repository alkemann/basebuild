using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour {

	World world;

	void Start () {
		world = new World ();
		SpritesController sc = GetComponentInParent<SpritesController> ();
		sc.renderTiles (world);
	}

	void Update()
	{

	}
		
}
