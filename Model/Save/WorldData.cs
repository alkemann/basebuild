using System.Xml.Serialization;

public class WorldData
{

	[XmlAttribute]
	public int width;
	[XmlAttribute]
	public int height;

	public WorldData () { }

	public WorldData(World world)
	{
		width = world.Width;
		height = world.Height;
	}
}
