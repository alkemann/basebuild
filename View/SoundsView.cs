using UnityEngine;
using System.Collections;

public class SoundsView : MonoBehaviour
{
	float soundCooldown = 0;

	void Start ()
	{
		WorldController.Instance.world.registerOnTileChanged (onTileChanged);
		WorldController.Instance.world.registOnJobCreated (onJobCreated);

	}

	void Update ()
	{
		soundCooldown -= Time.deltaTime;
	}

	void onJobCreated (Job job)
	{
		job.registerOnCompleteCallback ((j) => {
			switch (j.type) {
			case Job.TYPE.INSTALL:
				onFurnitureCreated(j.tile.furniture);
				break;
			default:
				// no sournd
				break;
			}
		});
		if (soundCooldown > 0)
			return;

		AudioClip ac;
		if (job.type == Job.TYPE.TERMINAL_WORK) {
			ac = Resources.Load<AudioClip> ("Sounds/terminal");
		} else {
			ac = Resources.Load<AudioClip> ("Sounds/job");
		}
		AudioSource.PlayClipAtPoint (ac, Camera.main.transform.position);
		soundCooldown = 0.1f;
	}

	void onTileChanged (Tile tile)
	{
		if (soundCooldown > 0)
			return;

		AudioClip ac;
		if (tile.type == Tile.TYPE.EMPTY) {
			ac = Resources.Load<AudioClip> ("Sounds/bulldoze");
		} else {
			ac = Resources.Load<AudioClip> ("Sounds/floor");
		}
		AudioSource.PlayClipAtPoint (ac, Camera.main.transform.position);
		soundCooldown = 0.1f;
	}

	void onFurnitureCreated (Furniture furn)
	{
		if (soundCooldown > 0)
			return;

		AudioClip ac = Resources.Load<AudioClip> ("Sounds/wall");
		AudioSource.PlayClipAtPoint (ac, Camera.main.transform.position);
		soundCooldown = 0.1f;
	}
}
