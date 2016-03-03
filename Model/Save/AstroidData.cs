using System.Xml.Serialization;

public class AstroidData
{
	[XmlAttribute]
	public int x;
	[XmlAttribute]
	public int y;
	[XmlAttribute]
	public float val;

	public AstroidData () { }

	public AstroidData (Astroid astroid)
	{
		Tile tile = astroid.tile;
		x = tile.X;
		y = tile.Y;
		val = astroid.resources_left;
	}
}

