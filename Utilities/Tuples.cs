using System.Xml.Serialization;

public class Tuple2<T1, T2>
{
	[XmlAttribute]
	public T1 item1;
	[XmlAttribute]
	public T2 item2;

	public Tuple2 () { }

	public Tuple2 (T1 t1, T2 t2)
	{
		this.item1 = t1;
		this.item2 = t2;
	}
}

public class Tuple3<T1, T2, T3> : Tuple2<T1, T2>
{
	[XmlAttribute]
	public T3 item3;

	public Tuple3 () { }

	public Tuple3 (T1 t1, T2 t2, T3 t3)
	{
		this.item1 = t1;
		this.item2 = t2;
		this.item3 = t3;
	}
}

