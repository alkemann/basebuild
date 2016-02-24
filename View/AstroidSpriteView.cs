using UnityEngine;
using System;
using System.Collections.Generic;

class AstroidSpriteView : MonoBehaviour
{
	public GameObject prefab = null;
	public GameObject parent = null;
	public Sprite mining = null;
	Dictionary<Astroid, GameObject> astroidToGameObjectMap;

	void Start ()
	{
		astroidToGameObjectMap = new Dictionary<Astroid, GameObject>();
		WorldController.Instance.world.RegisterOnAstroidCreated(astroidCreated);
	}

	public void astroidCreated (Astroid astroid)
	{
		Tile tile = astroid.tile;
		GameObject go = (GameObject)Instantiate(prefab, new Vector3(tile.X, tile.Y, -1f), Quaternion.identity);
		go.transform.SetParent(parent.transform, true);
		astroidToGameObjectMap[astroid] = go;
		tile.registerOnFurnitureInstalled(OnAstroidTileChange);
	}

	public void OnAstroidTileChange (Tile tile)
	{
		// FIXME: when mine furniture is installed on astroid, change visual display
		if (tile.furniture != null && tile.furniture.type == Furniture.TYPE.MINER) {
			GameObject go = astroidToGameObjectMap[tile.astroid];
			SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
			sr.sprite = mining;
		}
	}
}
