using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

	public void SetBuildMode(string type)
	{
		WorldController.Instance.creatingFurniture = Furniture.TYPE.NONE;
		switch (type) {
		case "floor":
			WorldController.Instance.buildMode = Tile.TYPE.FLOOR;
			break;
		case "bulldoze":
			WorldController.Instance.buildMode = Tile.TYPE.EMPTY;
			break;

		default:
			break;
		}
	}

	public void SetCreateMode(string type)
	{
		WorldController.Instance.buildMode = Tile.TYPE.NONE;
		switch (type) {
		case "wall":
			WorldController.Instance.creatingFurniture = Furniture.TYPE.WALL;
			break;
		default:
			break;
		}
	}

}
