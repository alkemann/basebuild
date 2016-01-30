using UnityEngine;
using System.Collections.Generic;

public class TileSpritesController : MonoBehaviour {

	public Sprite sprite_empty;
	public Sprite sprite_floor;
	public Sprite sprite_wall;

	public GameObject tiles_parent;

	Dictionary<Tile, GameObject> tileToGameObjectMap;

	void Start ()
	{
		tileToGameObjectMap = new Dictionary<Tile, GameObject> ();
	}

	public void renderTiles (World world)
	{
		for (int x = 0; x < World.WIDTH; x++) {
			for (int y = 0; y < World.HEIGHT; y++) {
				createTileSprite (world, x, y);
			}
		}
	}

	void onTileTypeChanged(Tile tile)
	{
		GameObject go = tileToGameObjectMap [tile];
		SpriteRenderer sr = go.GetComponent<SpriteRenderer> ();
		switch (tile.type) {
		case Tile.TYPE.EMPTY:
			sr.sprite = sprite_empty;
			break;
		case Tile.TYPE.FLOOR:
			sr.sprite = sprite_floor;
			break;
		default:
			Debug.LogError (tile);
			throw new System.Exception ("Uknown tile type or NONE");
		}
	}

	void createTileSprite (World world, int x, int y)
	{
		GameObject tile_go = new GameObject ();
		tile_go.name = "Tile_" + x + "_" + y;
		tile_go.transform.position = new Vector2 (x, y);
		tile_go.AddComponent<SpriteRenderer> ();
		tile_go.transform.SetParent (tiles_parent.transform, true);
		Tile tile = world.getTileAt (x, y);

		tileToGameObjectMap [tile] = tile_go;

		tile.registerOnChangeCallback(onTileTypeChanged);
		onTileTypeChanged(tile); // call it once to do the first sprite set
	}
}
