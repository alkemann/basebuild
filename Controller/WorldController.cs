using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour
{

	public static WorldController Instance;
	public static SaveGame saveGame;

	public World world { get; protected set; }

	public MenuController.COMMANDS activity = MenuController.COMMANDS.MOVE;
	public Furniture.TYPE installFurnitureType = Furniture.TYPE.NONE;

	public GameObject jobsCounter;
	public int HEIGHT;
	public int WIDTH;

	private bool worldSetupUp = false;

	public WorldController ()
	{
		if (Instance == null)
			Instance = this;
	}

	void OnEnable()
	{
		if (WorldController.saveGame != null) {
			WIDTH = saveGame.world.width;
			HEIGHT = saveGame.world.height;
		}
		world = new World(WIDTH, HEIGHT);
		Camera.main.transform.position = new Vector3 (WIDTH / 2, HEIGHT / 2, Camera.main.transform.position.z);
	}

	public void Update()
	{
		if (worldSetupUp == false)
			SetupWorld(); // Does this really cost enough to not do it this way?
		world.tick (Time.deltaTime);
	}

	protected void SetupWorld()
	{
		if (WorldController.saveGame == null) {
			world.SetupBlankWorld();
		} else {
			world.SetupWorldFromData(WorldController.saveGame);
			WorldController.saveGame = null;
		}
		GetComponentInChildren<TileSpritesView>().RenderWorld();
		worldSetupUp = true;
	}

	public Tile getTileAt(int x, int y)
	{
		return world.getTileAt (x, y);
	}

	public void floorTile (int x, int y)
	{
		world.getTileAt (x, y).floor();
	}

	public void bulldozeTile(int x, int y)
	{
		world.getTileAt (x, y).bulldoze();
	}
}
