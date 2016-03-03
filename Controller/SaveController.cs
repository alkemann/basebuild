using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
	SaveGame saveGame;
	public void NewWorld ()
	{
		WorldController.Instance = null;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void QuickSave ()
	{
		SaveGame savegame = new SaveGame();
		savegame.Save(WorldController.Instance.world);
	}

	public void QuickLoad ()
	{
		WorldController.Instance = null;
		WorldController.saveGame = SaveGame.Load();
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
