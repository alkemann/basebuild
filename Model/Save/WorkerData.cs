using System.Xml.Serialization;

public class WorkerData
{

	[XmlAttribute]
	public float walk_speed;
	[XmlAttribute]
	public float work_speed;
	[XmlAttribute]
	public int x;
	[XmlAttribute]
	public int y;

	// TODO: Should we save job?
	// TODO: SHould we save current destination?

	public WorkerData () {}

	public WorkerData (Worker worker)
	{
		Tile t = worker.GetCurrentTileOfWorker();
		x = t.X;
		y = t.Y;
		walk_speed = worker.walk_speed;
		work_speed = worker.work_speed;
	}
}

