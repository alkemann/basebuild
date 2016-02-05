using System.Collections.Generic;
using UnityEngine;
using Path;

namespace Path {
	public class Finder 
	{

		World world;
		TileGraph tileGraph;

		public Finder(World world)
		{
			this.world = world;
			world.registerOnTileChanged (tileChanged); // listen to all tile changes to recreate graph
			// FIXME delay on so that many changes doesnt cause constant re-graphing
		}

		/**
		 * Be ready to add or remove tiles to possible traversable graph
		 */
		protected void tileChanged(Tile t)
		{
			tileGraph = null;
		}

		public List<Tile> findPath(Tile from, Tile to)
		{
			if (tileGraph == null)
				tileGraph = new TileGraph (world);
			return tileGraph.search(from, to);
		}

	}
}

