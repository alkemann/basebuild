using UnityEngine;
using System.Collections.Generic;

namespace Path {


	public class RoomGraph
	{

	}

	public class TileGraph
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

		public List<Tile> search(Tile from, Tile target)
		{
			Node<Tile> start = nodes [from];
			Node<Tile> goal = nodes [target];
			PriorityQueue<Node<Tile>> frontier = new PriorityQueue<Node<Tile>>();
			frontier.put (start, 0);

			Dictionary<Node<Tile>, Node<Tile>> came_from = new Dictionary<Node<Tile>, Node<Tile>> (); // Assume we must travel half nodes
			Dictionary<Node<Tile>, float> cost_so_far = new Dictionary<Node<Tile>, float>();    // nodes.Count / 2

			came_from [start] = null;
			cost_so_far [start] = 0;

			while (frontier.Count > 0) {
				Node<Tile> current = frontier.get ();

				if (current == goal)
					break; // found our goal

				foreach (Edge<Tile> edge in current.edges) {
					Node<Tile> next = edge.node;
					float new_cost = cost_so_far [current] + edge.cost;
					if (cost_so_far.ContainsKey (next) == false || new_cost < cost_so_far [next]) {
						cost_so_far [next] = new_cost;
						int priority = (int)new_cost + heuristic_cost_estimate (next.data, target);
						frontier.put (next, priority);
						came_from [next] = current;
					}
				}
			}

			if (came_from [goal] == null) {
				Debug.Log ("No path found");
				return null;
			}

			List<Tile> path = new List<Tile> ();
			Node<Tile> iterator = goal;
			do {
				path.Add(iterator.data);
				iterator = came_from[iterator];
			} while (iterator != null);
			return path;
		}

		protected int heuristic_cost_estimate (Tile start, Tile goal)
		{
			return Mathf.Abs(start.X-goal.X) + Mathf.Abs(start.Y-goal.Y);
		}

	}

	class Node<T>
	{
		public T data;
		public List<Edge<T>> edges;

		public Node(T data)
		{
			this.data = data;
			edges = new List<Edge<T>> (8);
		}
	}

	class Edge<T>
	{
		public Node<T> node;
		public float cost;

		public Edge (Node<T> node, float cost)
		{
			this.node = node;
			this.cost = cost;
		}

	}

}
