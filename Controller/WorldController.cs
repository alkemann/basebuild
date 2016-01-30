using UnityEngine;

public class WorldController : MonoBehaviour {

	World world;

	void Start ()
	{
		world = new World ();
		GetComponentInParent<TileSpritesController> ().renderTiles (world);
	}

	public Tile getTileAt(int x, int y)
	{
		return world.getTileAt (x, y);
	}

	public Job createInstallJobAt(Furniture.TYPE type, int x, int y)
	{
		return world.createInstallJobAt (type, x, y);
	}
}
