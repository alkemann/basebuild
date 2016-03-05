using System.Collections.Generic;
using System.Xml.Serialization;

public class JobData
{
	[XmlAttribute]
	public int x;
	[XmlAttribute]
	public int y;
	[XmlAttribute]
	public int type;
	[XmlAttribute]
	public float work;

	[XmlArray("extra")]
	[XmlArrayItem("meta")]
	public List<Tuple2<string, object>> meta = null;

	public JobData () { }

	public JobData (Job job)
	{
		x = job.tile.X;
		y = job.tile.Y;
		work = job.WorkLeft();
		type = (int) job.type;
		if (job.meta != null && job.meta.Count > 0) {
			meta = new List<Tuple2<string, object>>();
			foreach (KeyValuePair<string, object> entry in job.meta) {
				meta.Add(new Tuple2<string, object>(entry.Key, entry.Value));
			}
		}
	}

	public object GetMeta(string key)
	{
		foreach (Tuple2<string, object> t in meta) {
			if (t.item1 == key) {
				return t.item2;
			}
		}
		return null;
	}
}
