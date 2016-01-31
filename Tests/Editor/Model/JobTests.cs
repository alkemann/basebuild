using System.Collections;
using NUnit.Framework;

namespace BaseBuild.Tests
{
	public class JobTests
	{
		[Test]
		[Category("Construction Tests")]
		public void testInstallationJob()
		{
			World world = new World (20, 20);
			Tile tile = new Tile (world, 10, 10);
			Furniture furn = new Furniture (tile, Furniture.TYPE.WALL);
			Job job = new Job (tile, furn);
			Assert.That (job.tile == tile);
			Assert.That (job.furniture == furn);
		}

	}
}