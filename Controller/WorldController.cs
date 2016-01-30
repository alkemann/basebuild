using UnityEngine;

public class WorldController : MonoBehaviour {

	World world;

	void Start ()
	{
		world = new World ();
		GetComponentInParent<TileSpritesView> ().renderTiles (world);
	}

	public Tile getTileAt(int x, int y)
	{
		return world.getTileAt (x, y);
	}

	public void buildTile (Tile.TYPE new_tile_type, int x, int y)
	{
		world.buildTileAt (new_tile_type, x, y);
	}

	public Job createInstallJobAt(Furniture.TYPE type, int x, int y)
	{
		return world.createInstallJobAt (type, x, y);
	}
}
