using System.Xml.Serialization;

public class TileData
{
	[XmlAttribute]
	public int x;
	[XmlAttribute]
	public int y;
	[XmlAttribute]
	public int type;
	[XmlAttribute]
	public int furn = 0;
	[XmlElement]
	public AstroidData astroid;

	public TileData () { }

	public TileData (Tile tile)
	{
		x = tile.X;
		y = tile.Y;
		type = (int) tile.type;
		if (tile.isInstalled())
			furn = (int) tile.furniture.type;
		if (tile.astroid != null)
			astroid = new AstroidData(tile.astroid);

	}
}

