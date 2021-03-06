using UnityEngine;
using System.Collections.Generic;
using System;

namespace Path {

	public class Finder
	{
		World world;
		TileGraph tileGraph;

		public Finder(World world)
		{
			this.world = world;
			world.registerOnTileChanged ((t) => tileGraph = null); // listen to all tile changes to invalidate graph
		}

		public Stack<Tile> findPath(Tile from, Tile to)
		{
			if (tileGraph == null)
				tileGraph = new TileGraph (world);
			return tileGraph.search(from, to);
		}
	}

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
				List<Edge<Tile>> edges = new List<Edge<Tile>> (8);
				// get connected tiles
				foreach (Tile connected in tile.getConnected()) {
					// neighbor is not in space or not passable, add edge to it
					if (connected.isWalkable() && isNotCuttonCorner(tile, connected)) {
						edges.Add (
							new Edge<Tile> (
								nodes[connected],
								connected.costToEnterFrom (tile.X, tile.Y) + (connected.hasJob() ? 5f : 0f)  // add cost for pathfinding if tile has job
							)
						);
					}
				}
				node.edges = edges.ToArray ();
			}
		}

		public bool isNotCuttonCorner(Tile from, Tile to)
		{
			int dx = from.X - to.X;
			int dy = from.Y - to.Y;

			// Are we actually moving diagonally?
			if (Mathf.Abs(dx) + Mathf.Abs(dy) != 2)
				return true; // No? then not a problem

			// Are both the relevant N/S and E/W tiles open floors?
			Tile north_south = from.world.getTileAt(from.X, from.Y - dy);
			Tile east_west = from.world.getTileAt(from.X - dx, from.Y);

			return north_south.isWalkable() && east_west.isWalkable();
		}

		public override Stack<Tile> search(Tile from, Tile target)
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
