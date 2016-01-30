using UnityEngine;

public class WorldController : MonoBehaviour {

	public static WorldController Instance;
	public World world { get; protected set; }

	public Tile.TYPE buildMode = Tile.TYPE.FLOOR;

	public Furniture.TYPE creatingFurniture;

	void OnEnable()
	{
		if (Instance == null)
			Instance = this;
		world = new World ();
	}

	void Start ()
	{
		GetComponentInParent<TileSpritesView> ().renderTiles (world);
		world.createWorkerAt (World.WIDTH / 2, World.HEIGHT / 2); // FIXME for test purposes
	}

	public void Update()
	{
		world.tick (Time.deltaTime);
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
