using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour {

	public static WorldController Instance;
	public World world { get; protected set; }

	public MenuController.COMMANDS activity = MenuController.COMMANDS.MOVE;
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
		Camera.main.transform.position = new Vector3 (WIDTH / 2, HEIGHT / 2, Camera.main.transform.position.z);
	}

	void Start ()
	{
		GetComponentInChildren<TileSpritesView> ().renderTiles (world);
	}

	public void Update()
	{
		world.tick (Time.deltaTime);
	}

	public GameObject jobsCounter;

	public void FixedUpdate()
	{
		jobsCounter.GetComponent<Text> ().text = ""+world.jobs.Count ();
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
