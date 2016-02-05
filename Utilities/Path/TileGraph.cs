using UnityEngine;
using System.Collections.Generic;
using System;

namespace Path {

	class TileGraph : Graphs<Tile>
	{
		Dictionary<Tile, Node<Tile>> nodes;

		public TileGraph(World world)
		{
			nodes = new Dictionary<Tile, Node<Tile>> (10);

			// create nodes for tiles that are passable and are not in space
			for (int x = 0; x < world.Width; x++) {
				for (int y = 0; y < world.Height; y++) {
					Tile t = world.getTileAt (x, y);
					if (t.type != Tile.TYPE.EMPTY) { //  && t.isPassable ()
						nodes.Add (t, new Node<Tile> (t));
					}
				}
			}

			// foreach node
			foreach (Node<Tile> node in nodes.Values) {
				Tile tile = node.data;
				// get connected tiles
				foreach (Tile connected in tile.getConnected()) {
					// neighbor is not in space or not passable, add edge to it
					if (connected.type != Tile.TYPE.EMPTY ) { // && connected.isPassable ()
						node.edges.Add (new Edge<Tile> (nodes[connected], connected.costToEnterFrom (tile.X, tile.Y)));
					}
				}
			}
		}

		public override List<Tile> search(Tile from, Tile target)
		{
			return aStarSearch(
				nodes, // graph to search in
				from, // Tile to start in
				target,  // Destination tile
				(Tile one, Tile two) => { // heuristic method
					return one.heuristic_cost_estimate (two);
				}
			);
		}

	}
}
