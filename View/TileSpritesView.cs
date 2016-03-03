using UnityEngine;
using System.Collections.Generic;

public class TileSpritesView : MonoBehaviour {

	public Sprite sprite_empty;
	public Sprite sprite_floor;

	public GameObject tiles_parent;

	Dictionary<Tile, GameObject> tileToGameObjectMap;

	public void RenderWorld()
	{
		World world = WorldController.Instance.world;
		tileToGameObjectMap = new Dictionary<Tile, GameObject>();
		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {
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

		if (tile.isInstalled()) {
			// Lets create the initial furniture as well
			GetComponent<FurnitureSpritesView>().onFurnitureInstalled(tile);
		}
	}
}
