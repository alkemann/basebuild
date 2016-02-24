using UnityEngine;
using System;
using System.Collections.Generic;

class AstroidSpriteView : MonoBehaviour
{
	public GameObject prefab;
	public GameObject parent;


	Dictionary<Astroid, GameObject> astroidToGameObjectMap;

	void Start ()
	{
		astroidToGameObjectMap = new Dictionary<Astroid, GameObject>();
		WorldController.Instance.world.RegisterOnAstroidCreated(astroidCreated);
	}

	public void astroidCreated (Astroid astroid)
	{
		Tile tile = astroid.tile;
		int x = tile.X;
		int y = tile.Y;

		GameObject go = (GameObject)Instantiate(prefab, new Vector3(x, y, -1f), Quaternion.identity);
		go.transform.SetParent(parent.transform, true);

		astroidToGameObjectMap[astroid] = go;

		astroid.RegisterOnChangeCallback(onAstroidChange);
	}

	public void onAstroidChange (Astroid astroid)
	{
		// FIXME: when mine furniture is installed on astroid, change visual display
	}
}
