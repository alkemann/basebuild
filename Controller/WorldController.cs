using UnityEngine;

public class WorldController : MonoBehaviour {

	public static WorldController Instance;
	public World world { get; protected set; }

	public MenuController.COMMANDS activity = MenuController.COMMANDS.MOVE;
	public Tile.TYPE constructTileType = Tile.TYPE.FLOOR;
	public Furniture.TYPE installFurnitureType = Furniture.TYPE.NONE;

	public int HEIGHT;
	public int WIDTH;

	public WorldController ()
	{
		if (Instance == null)
			Instance = this;
	}

	void OnEnable()
	{
		world = new World (WIDTH, HEIGHT);
	}

	void Start ()
	{
		GetComponentInParent<TileSpritesView> ().renderTiles (world);
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
}
