using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

	public enum COMMANDS
	{
		NONE,
		BULLDOZE,
		CONSTRUCT_TILE,
		BUILD_FURNITURE,
		MOVE
	}

	public void setCommandButton(string command_string)
	{
		WorldController wc = WorldController.Instance;

		wc.constructTileType = Tile.TYPE.NONE;
		wc.installFurnitureType = Furniture.TYPE.NONE;

		switch (command_string) {
		case "none":
			wc.activity = COMMANDS.NONE;
			break;
		case "bulldoze":
			wc.activity = COMMANDS.BULLDOZE;
			break;
		case "construct":
			wc.activity = COMMANDS.CONSTRUCT_TILE;
			break;
		case "install":
			wc.activity = COMMANDS.BUILD_FURNITURE;
			break;
		case "move":
			wc.activity = COMMANDS.MOVE;
			break;
		}
	}
	public void setTileTypeButton(string type_string)
	{
		WorldController.Instance.constructTileType =getTypeFromString (type_string);
	}

	public void setFurnitureTypeButton(string type_string)
	{
		WorldController.Instance.installFurnitureType = getFurnitureTipeFromString (type_string);
	}

	private Tile.TYPE getTypeFromString (string type_string)
	{
		switch (type_string) {
		case "floor":
			return Tile.TYPE.FLOOR;
		}
		throw new UnityException ("No such type: " + type_string);
	}

	private Furniture.TYPE getFurnitureTipeFromString (string type_string)
	{
		switch (type_string) {
		case "door":
			return Furniture.TYPE.DOOR;
		case "wall":
			return Furniture.TYPE.WALL;
		}
		throw new UnityException ("No such type: " + type_string);
	}

	public void addWorker()
	{
		WorldController.Instance.world.createWorkerAt (World.WIDTH / 2, World.HEIGHT / 2);
	}
}
