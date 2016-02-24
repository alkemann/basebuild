using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

	public static bool active = false;

	public enum COMMANDS
	{
		NONE,
		BULLDOZE,
		CANCEL,
		FLOOR_TILE,
		INSTALL_FURNITURE,
		UNINSTALL_FURNITURE,
		MOVE
	}

	public void setCommandButton(string command_string)
	{
		if (MenuController.active == false)
			return;
		WorldController wc = WorldController.Instance;

		wc.installFurnitureType = Furniture.TYPE.NONE;

		switch (command_string) {
		case "none":
			wc.activity = COMMANDS.NONE;
			break;
		case "bulldoze":
			wc.activity = COMMANDS.BULLDOZE;
			break;
		case "cancel":
			wc.activity = COMMANDS.CANCEL;
			break;
		case "construct":
			wc.activity = COMMANDS.FLOOR_TILE;
			break;
		case "install":
			wc.activity = COMMANDS.INSTALL_FURNITURE;
			break;
		case "uninstall":
			wc.activity = COMMANDS.UNINSTALL_FURNITURE;
			break;
		case "move":
			wc.activity = COMMANDS.MOVE;
			break;
		}
	}

	public void setFurnitureTypeButton(string type_string)
	{
		if (MenuController.active == false)
			return;
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
		case "terminal":
			return Furniture.TYPE.TERMINAL;
		}
		throw new UnityException ("No such type: " + type_string);
	}

	public void addWorker()
	{
		if (MenuController.active == false)
			return;
		WorldController.Instance.world.createWorkerAt (WorldController.Instance.world.Width / 2, WorldController.Instance.world.Height / 2);
	}

	public void togglePlayPayse()
	{
		if (MenuController.active == false)
			return;
		WorldController.Instance.world.pause = !WorldController.Instance.world.pause;
	}

}
