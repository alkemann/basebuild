
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

[XmlRoot("data")]
public class SaveGame
{
	[XmlElement("world")]
	public WorldData world;

	[XmlArray("workers")]
	[XmlArrayItem("worker")]
	public List<WorkerData> workers;

	[XmlArray("tiles")]
	[XmlArrayItem("tile")]
	public List<TileData> tiles;

	// TODO Save job queue?

	public void Save (World world, string name = "SaveGameQuick")
	{
		this.world = new WorldData(world);
		workers = new List<WorkerData>(10);
		foreach (Worker worker in world.workers) {
			workers.Add(new WorkerData(worker));
		}

		tiles = new List<TileData>(10);
		foreach (Tile tile in world.tiles) {
			if (tile.isInstalled() || tile.isWalkable())
				tiles.Add(new TileData(tile));
		}

		XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
		TextWriter writer = new StringWriter();
		serializer.Serialize(writer, this);
		writer.Close();
		PlayerPrefs.SetString(name, writer.ToString());
	}

	public static SaveGame Load (string name = "SaveGameQuick")
	{
		string xml = PlayerPrefs.GetString(name);
		XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
		TextReader reader = new StringReader(xml);
		Debug.Log(reader.ToString());
		SaveGame data = (SaveGame)serializer.Deserialize(reader);
		reader.Close();
		return data;
	}
}
