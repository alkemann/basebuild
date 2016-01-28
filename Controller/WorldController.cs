using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour {

	World world;

	void Start ()
	{
		world = new World ();
		GetComponentInParent<SpritesController> ().renderTiles (world);
	}

	public Tile getTileAt(int x, int y)
	{
		return world.getTileAt (x, y);
	}
}
