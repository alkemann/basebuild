
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

[XmlRoot("data")]
public class SaveGame
{

	[XmlElement("camera")]
	public Tuple3<float, float, float> camera;
	[XmlElement("world")]
	public WorldData world;

	[XmlArray("workers")]
	[XmlArrayItem("worker")]
	public List<WorkerData> workers;

	[XmlArray("jobs")]
	[XmlArrayItem("job")]
	public List<JobData> jobs;

	[XmlArray("tiles")]
	[XmlArrayItem("tile")]
	public List<TileData> tiles;

	public void Save (World world, string name = "SaveGameQuick")
	{
		float x = Camera.main.transform.position.x;
		float y = Camera.main.transform.position.y;
		float z = Camera.main.orthographicSize;
		this.camera = new Tuple3<float, float, float>(x, y, z);

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

		jobs = new List<JobData>();
		foreach (Job job in world.jobs) {
			jobs.Add(new JobData(job));
		}

		XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
		TextWriter writer = new StringWriter();
		serializer.Serialize(writer, this);
		writer.Close();

		string xml = writer.ToString();
		Debug.Log(xml);

		PlayerPrefs.SetString(name, xml);
	}

	public static SaveGame Load (string name = "SaveGameQuick")
	{
		string xml = PlayerPrefs.GetString(name);
		Debug.Log(xml);
		XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
		TextReader reader = new StringReader(xml);
		SaveGame data = (SaveGame)serializer.Deserialize(reader);
		reader.Close();
		return data;
	}
}
