using UnityEngine;
using System.Collections.Generic;
using System;

public class StartControll : MonoBehaviour {

	public GameObject root;

	public void OnButtonClick () {
		World world = WorldController.Instance.world;
		int x = WorldController.Instance.HEIGHT / 2;
		int y = WorldController.Instance.WIDTH / 2;

		Action<Job> autoComplete = (Job j) => {
			Job job = world.getFirstJob();
			job.doWork(100f);
		};

		world.registOnJobCreated (autoComplete);

		world.buildTileAt (Tile.TYPE.FLOOR, x, y);
		world.createInstallJobAt (Furniture.TYPE.SPAWNER, x, y);
		world.buildTileAt (Tile.TYPE.FLOOR, x+1, y);
		world.buildTileAt (Tile.TYPE.FLOOR, x-1, y);

		world.buildTileAt (Tile.TYPE.FLOOR, x+1, y+1);
		world.buildTileAt (Tile.TYPE.FLOOR, x, y+1);
		world.buildTileAt (Tile.TYPE.FLOOR, x-1, y+1);

		world.buildTileAt (Tile.TYPE.FLOOR, x+1, y-1);
		world.buildTileAt (Tile.TYPE.FLOOR, x, y-1);
		world.buildTileAt (Tile.TYPE.FLOOR, x-1, y-1);

		world.getFirstJob();
		world.unregistOnJobCreated (autoComplete);
		MenuController.active = true;
		Destroy (root); // Wont need this any more
	}

}
